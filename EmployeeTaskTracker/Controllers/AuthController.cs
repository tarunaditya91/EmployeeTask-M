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

        //  Register endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Employee employee)
        {
            // Hash the password before saving
            employee.Password = _passwordHasher.HashPassword(employee, employee.Password);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return Ok("User registered successfully");
        }

        //  Login endpoint
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var user = _context.Employees.FirstOrDefault(e => e.FullName == login.Username);
            if (user == null) return Unauthorized("Invalid credentials");

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, login.Password);
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

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}