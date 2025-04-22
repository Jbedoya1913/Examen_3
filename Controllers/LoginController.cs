using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Universidad.Matriculas.Data;
using Universidad.Matriculas.Models;

namespace Universidad.Matriculas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Estudiantes
                .FirstOrDefault(u => u.Usuario == request.Usuario && u.Clave == request.Clave);

            if (user == null) return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("CLAVE_SUPER_SECRETA_SEGURA_123456789ABCDEFGHIJK");

            var token = new JwtSecurityToken(
                claims: new[] { new Claim(ClaimTypes.Name, user.Documento) },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
    }

    public class LoginRequest
    {
        public string? Usuario { get; set; }
        public string? Clave { get; set; }
    }
} 