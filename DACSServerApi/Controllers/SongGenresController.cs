
using DACSServerApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Entities;


namespace DACSServerApi.Controller
{
    [Route("api/song-genres")]
    [ApiController]
    public class SongGenresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SongGenresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/SongGenres
        [HttpGet]
        public async Task<ActionResult<List<SongGenre>>> GetAllSongGenres()
        {
            return await _context.SongsGenres.ToListAsync();

        }

        // GET: api/SongGenres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SongGenre>> GetSongGenre(int id)
        {
            var songGenre = await _context.SongsGenres.FindAsync(id);

            if (songGenre == null)
            {
                return NotFound("Thể loại bài hát không tồn tại!");
            }

            return songGenre;
        }

        // PUT: api/SongGenres/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<SongGenre>> PutSongGenre(int id,[FromForm] SongGenre songGenre)
        {
            var dbsongGenres = await _context.SongsGenres.FindAsync(id);
            if(dbsongGenres is null)
            {
                return NotFound("Thể loại bài hát không tồn tại!");
            }
            dbsongGenres.SongGenreName = songGenre.SongGenreName;
            await _context.SaveChangesAsync();
            return Ok(dbsongGenres);
        }
        // POST: api/SongGenres
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SongGenre>> PostSongGenre([FromForm] SongGenre songGenre)
        {
            _context.SongsGenres.Add(songGenre);
            await _context.SaveChangesAsync();
            return Ok(songGenre);
        }

        // DELETE: api/SongGenres/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SongGenre>> DeleteSongGenre(int id)
        {
            var songGenre = await _context.SongsGenres.FindAsync(id);
            if (songGenre == null)
            {
                return NotFound();
            }

            _context.SongsGenres.Remove(songGenre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SongGenreExists(int id)
        {
            return _context.SongsGenres.Any(e => e.SongGenreId == id);
        }
    }
}
