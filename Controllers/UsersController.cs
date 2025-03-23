using BookStoreApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using BookStoreApi.Data;
using BookStoreApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly BookStoreApiDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UsersController(BookStoreApiDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserRegisterDto userDto)
        {
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, userDto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);

            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userDto.Password) == PasswordVerificationResult.Failed)
                return Unauthorized("نام کاربری یا رمز عبور اشتباه است");

            // تولید توکن JWT (نیاز به پیاده سازی سرویس توکن)
            var token = GenerateJwtToken(user);

            return Ok(token);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            return user;
        }

        private string GenerateJwtToken(User user)
        {
            // پیاده سازی تولید توکن JWT
            // مثال:
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("کلید-امنیتی-شما"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "your-issuer",
                audience: "your-audience",
                claims: new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) },
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
