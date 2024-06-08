
using DACSServerApi.Data;
using DACSServerApi.ModelFile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Entities;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace DACSServerApi.Controller
{
    [Route("api/Artists")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public ArtistsController(AppDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet("current")]
        public async Task<ActionResult<Artist>> GetArtistIdForCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var artist = await _context.Artists.Include(a => a.Songs).Include(a => a.Albums).FirstOrDefaultAsync(a => a.UserId == userId);
            if (artist == null)
            {
                return NotFound();
            }
            return Ok(artist);
        }
        [HttpGet("UnApproved")]
        public async Task<ActionResult<List<Artist>>> GetArtistsUnApproved()
        {
            return await _context.Artists.Where(a => a.IsApprovedArtist == false).ToListAsync();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("artistuser")]
        public async Task<ActionResult<List<Artist>>> GetArtistbyUser()
        {
            return await _context.Artists.Include(a => a.Songs).Include(a => a.User).ToListAsync();
        }
        // GET: api/Artists
        [HttpGet("artistwithsong")]
        public async Task<ActionResult<List<Artist>>> GetArtistWithSong()
        {
            var artist = await _context.Artists.Include(a => a.Songs).ToListAsync();
            return Ok(artist);
        }
        [HttpGet("artistwithsong/{id}")]
        public async Task<ActionResult<Artist>> GetArtistWithSongById(int id)
        {
            var artist = await _context.Artists.Include(a => a.Songs).FirstOrDefaultAsync(a => a.ArtistId == id);
            return Ok(artist);
        }
        [HttpGet]
        public async Task<ActionResult<List<Artist>>> GetArtists()
        {
            var artists = await _context.Artists.Include(a => a.Songs).Include(a => a.Albums).ToListAsync();
            return Ok(artists);
        }
        // GET: api/Artists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<List<Artist>>> GetArtist(int id)
        {
            var artist = await _context.Artists.Include(a => a.Songs).Include(a => a.Albums).FirstOrDefaultAsync(a => a.ArtistId == id);

            if (artist == null)
            {
                return NotFound();
            }

            return Ok(artist);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Artist>> PutArtist(int id, [FromForm] Artist artist, [FromForm] UploadImageFile? imageFile)
        {
            var dbArtist = await _context.Artists.FindAsync(id);
            if (dbArtist == null)
            {
                return NotFound("Song not Found");
            }
            if (imageFile.imageFile != null)
            {
                dbArtist.ArtistImage = await SaveFileAsync(imageFile.imageFile, dbArtist.ArtistName, "images");
            }
            dbArtist.ArtistName = artist.ArtistName;
            dbArtist.ArtistDescription = artist.ArtistDescription;
            dbArtist.Type = artist.Type;
            dbArtist.Genres = artist.Genres;
            await _context.SaveChangesAsync();
            return Ok(dbArtist);
        }

        // POST: api/Artists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Artist>> RegisterArtist([FromForm] Artist artist, [FromForm] UploadImageFile? imageFile)
        {
            if (imageFile.imageFile != null)
            {
                artist.ArtistImage = await SaveFileAsync(imageFile.imageFile, artist.ArtistName, "images");
            }
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
            return Ok(artist);
        }

        // DELETE: api/Artists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.ArtistId == id);
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

            // Trả về đường dẫn tương đối của tệp đã lưu
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
