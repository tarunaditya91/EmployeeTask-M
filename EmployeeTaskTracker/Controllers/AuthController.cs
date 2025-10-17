using EmployeeTaskTracker.Data;
using EmployeeTaskTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeTaskTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDBcontext _context;
        private readonly PasswordHasher<Employee> _passwordHasher;

        public AuthController(IConfiguration config, ApplicationDBcontext context)
        {
            _config = config;
            _context = context;
            _passwordHasher = new PasswordHasher<Employee>();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Employee employee)
        {
            if (_context.Employees.Any(e => e.FullName == employee.FullName))
                return BadRequest("User already exists");

            // Hash the password (raw password is in PasswordHash property)
            employee.PasswordHash = _passwordHasher.HashPassword(employee, employee.PasswordHash);

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var user = _context.Employees.FirstOrDefault(e => e.FullName == login.Username);
            if (user == null) return Unauthorized("Invalid credentials");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, login.Password);
            if (result == PasswordVerificationResult.Success)
            {
                var token = GenerateJwtToken(user.FullName, user.Role);
                return Ok(new { token });
            }

            return Unauthorized("Invalid credentials");
        }

        private string GenerateJwtToken(string username, string role)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}