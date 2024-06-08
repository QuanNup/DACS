
using DACSServerApi.Data;
using DACSServerApi.ModelFile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Entities;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace DACSServerApi.Controller
{
    [Route("api/songs")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SongsController(AppDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet("UnApproved")]
        public async Task<ActionResult<List<Song>>> GetSongNotApproved()
        {
            var song = await _context.Songs.Where(s => s.IsApproved == false).Include(s => s.Artist).Include(s => s.Album).ToListAsync();
            return Ok(song);
        }
        [HttpGet("IsApproved")]
        public async Task<ActionResult<List<Song>>> GetApprovedSongs()
        {
            var approvedSongs = await _context.Songs.Where(s => s.IsApproved == true).Include(s=>s.Artist).Include(s=>s.Album).ToListAsync();
            return Ok(approvedSongs);
        }

        [HttpGet("Search/{searchText}")]
        public async Task<ActionResult<List<Song>>> searchSong(string searchText)
        {
            return await _context.Songs.Include(s=>s.Artist).Where(s => s.SongName.Contains(searchText) || s.Artist.ArtistName.Contains(searchText) || s.SongGenre.SongGenreName.Contains(searchText)).ToListAsync();
        }
            

        [HttpGet("{id}")]
        public async Task<ActionResult<Song>> GetSongByIdAsync(int id)
        {
            try
            {
                var result = await _context.Songs.Include(s => s.Artist).FirstOrDefaultAsync(s => s.SongId == id);
                if (result == null)
                {
                    return NotFound("Song not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("file")]
        public IActionResult GetMusic([FromQuery] string filePath)
        {
            try
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return new FileStreamResult(fileStream, "audio/mpeg")
                {
                    EnableRangeProcessing = true
                };
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("file-images")]
        public IActionResult GetImage(string imageName)
        {
            var imagePath = Path.Combine(imageName);
            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return File(imageBytes, "image/jpeg");
        }

        [HttpGet]
        public async Task<ActionResult<List<Song>>> GetSongs()
        {
            var approvedSongs = await _context.Songs.Include(s=>s.SongGenre).Include(s=>s.Artist).ToListAsync();
            return Ok(approvedSongs);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Song>> DeleteSongAsync(int id)
        {
            var result = await _context.Songs.FindAsync(id);
            if (result == null)
            {
                return NotFound("Song not Found");
            }
            _context.Songs.Remove(result);
            await _context.SaveChangesAsync();

            return Ok(result);
        }
        [HttpPut("{id}")]

        public async Task<ActionResult<Song>> UpdateSongAsync(int id, [FromForm] Song updateSong, [FromForm] UploadImageFile? imageFile, [FromForm] UploadSongFile? songFile)
        {

            var dbSong = await _context.Songs.FindAsync(id);
            if (dbSong == null)
            {
                return NotFound("Bài hát không tồn tại!");
            }

            if (imageFile.imageFile != null)
            {
                dbSong.SongImage = await SaveFileAsync(imageFile.imageFile, dbSong.SongName, "images");
            }
            if (songFile.songFile != null)
            {
                dbSong.SongFile = await SaveFileAsync(songFile.songFile, dbSong.SongName, "songs");
            }
            dbSong.ArtistId = updateSong.ArtistId;
            dbSong.SongGenreId = updateSong.SongGenreId;
            dbSong.SongName = updateSong.SongName;
            dbSong.SongDescription = updateSong.SongDescription;
            await _context.SaveChangesAsync();
            return Ok(dbSong);
        }
        [HttpPost]
        public async Task<ActionResult<Song>> AddSongAsync([FromForm] Song newSong, [FromForm] UploadImageFile? imageFile, [FromForm] UploadSongFile? songFile)
        {
            if (imageFile.imageFile != null)
            {
                newSong.SongImage = await SaveFileAsync(imageFile.imageFile, newSong.SongName, "images");
            }

            if (songFile.songFile != null)
            {
                newSong.SongFile = await SaveFileAsync(songFile.songFile, newSong.SongName, "songs");
            }

            newSong.IsApproved = true;
            _context.Songs.Add(newSong);
            await _context.SaveChangesAsync();
            return Ok(newSong);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("AddSongByAdmin")]
        public async Task<ActionResult<Song>> AddSongByAdminAsync([FromForm] Song newSong, [FromForm] UploadImageFile? imageFile, [FromForm] UploadSongFile? songFile)
        {
            if (imageFile.imageFile != null)
            {
                newSong.SongImage = await SaveFileAsync(imageFile.imageFile, newSong.SongName, "images");
            }

            if (songFile.songFile != null)
            {
                newSong.SongFile = await SaveFileAsync(songFile.songFile, newSong.SongName, "songs");
            }
            newSong.IsApproved = true;
            _context.Songs.Add(newSong);
            await _context.SaveChangesAsync();
            return Ok(newSong);
        }
        private async Task<string> SaveFileAsync(IFormFile file, string title, string subfolder)
        {
            var uploadsFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads", subfolder);
            var fileName = $"{ToValidFileName(title)}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
            var savePath = Path.Combine(uploadsFolder, fileName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Path.Combine("uploads", subfolder, fileName).Replace("\\", "/");
        }

        private string ToValidFileName(string text)
        {
            for (int i = 32; i < 48; i++)
            {
                text = text.Replace(((char)i).ToString(), "-");
            }
            text = text.Replace(".", "-");
            text = text.Replace(" ", "-");
            text = text.Replace(",", "-");
            text = text.Replace(";", "-");
            text = text.Replace(":", "-");

            Regex regex = new Regex(@"p{IsCombiningDiacriticalMarks}+");
            string temp = text.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty)
                        .Replace("u0111", "d").Replace("u0110", "D");
        }

    }
}
