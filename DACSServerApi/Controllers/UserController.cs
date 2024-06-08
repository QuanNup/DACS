
using DACSServerApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.DTOs;
using SharedClassLibrary.Entities;
using System.Security.Claims;

namespace DACSServerApi.Controller
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        public UserController(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetUserById()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user =  await _userManager.FindByIdAsync(userId);
            var userDto = new UserDTO
            {
                UserName = user?.UserName,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                IsPremium = user.IsPremium
            };
            return Ok(userDto);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserDTO>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(); 
            }
            if (user.Id != userId)
            {
                return Forbid();
            }
            var userDto = new UserDTO
            {
                UserName = user?.UserName,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                IsPremium = user.IsPremium
            };
            return Ok(userDto);
        }
        [HttpPut("update-profile")]
        public async Task<ActionResult<UserDTO>> UpdateProfile([FromForm] ApplicationUser userProfile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (userProfile.Id != userId)
            {
                return Forbid();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            user.PhoneNumber = userProfile.PhoneNumber;
            user.UserName = userProfile.UserName;
            user.LastName = userProfile.LastName;
            user.FirstName = userProfile.FirstName;
            user.DateOfBirth = userProfile.DateOfBirth;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            await _context.SaveChangesAsync();
            return Ok(user);
        }

    }
}
