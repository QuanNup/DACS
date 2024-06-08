using DACSServerApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Entities;
using System.Security.Claims;

namespace DACSServerApi.Controllers
{
    
    [Route("api/likedsongs")]
    [ApiController]
    public class LikedSongsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LikedSongsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<LikedSong>>> GetListLikedSong()
        {
            var likedsongs = await _context.LikedSongs.Include(l => l.LikedSongDetails).ToListAsync();
            return Ok(likedsongs);
        }
        [HttpGet("GetLikedSongs")]
        public async Task<ActionResult<LikedSong>> GetListLikedSongs()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var likedsongs = await _context.LikedSongs.Include(l => l.LikedSongDetails).ThenInclude(lsd => lsd.Song).ThenInclude(s=>s.Artist).FirstOrDefaultAsync(l=>l.UserId == userId);
            if (likedsongs == null)
            {
                return NotFound("Danh sách bài hát yêu thích không tồn tại trong tài khoản này");
            }
            return Ok(likedsongs);
        }
        [HttpGet("{songId}")]
        public async Task<ActionResult<bool>> GetLikedSongs(int songId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingLikeSong = await _context.LikedSongs.AnyAsync(ls => ls.UserId == userId && ls.LikedSongDetails.Any(lsd => lsd.SongId == songId));
            return Ok(existingLikeSong);
        }
        [HttpPost("{songId}")]
        public async Task<ActionResult<LikedSong>> PostLikeSong( int songId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingLikeSong = await _context.LikedSongs.AnyAsync(ls => ls.UserId == userId && ls.LikedSongDetails.Any(lsd => lsd.SongId == songId));
            if (existingLikeSong)
            {
                return NotFound("Bài hát đã được người dùng này thích.");
            }
            var likedSong = await _context.LikedSongs.Include(ls => ls.LikedSongDetails).FirstOrDefaultAsync(ls => ls.UserId == userId);
            if (likedSong == null)
            {
                likedSong = new LikedSong
                {
                    UserId = userId
                };
                _context.LikedSongs.Add(likedSong);
            }
            if (likedSong.LikedSongDetails == null)
            {
                likedSong.LikedSongDetails = new List<LikedSongDetails>(); 
            }
            var likedSongDetail = new LikedSongDetails
            {
                SongId = songId,
                LikedSong = likedSong
            };
            likedSong.LikedSongDetails.Add(likedSongDetail);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{songId}")]
        public async Task<ActionResult<LikedSong>> DeleteLikedSong(int songId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var likedSong = await _context.LikedSongs.Include(ls => ls.LikedSongDetails).FirstOrDefaultAsync(ls => ls.UserId == userId);
            if (likedSong == null)
            {
                return NotFound("Danh sách bài hát yêu thích không tồn tại trong tài khoản này");
            }
            var likedSongDetail = likedSong.LikedSongDetails.FirstOrDefault(lsd => lsd.SongId == songId);
            if (likedSongDetail == null)
            {
                return NotFound("Danh sách bài hát yêu thích không tồn tại trong tài khoản này");
            }
            _context.LikedSongsDetails.Remove(likedSongDetail);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
