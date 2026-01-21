using ClassDataMRL.Interfaces;
using ClassDomainMRL.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPIMRL.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            // ⚠️ Por ahora: "hash" simple para que coincida con tus datos seed.
            // Luego lo cambiamos a BCrypt.
            var passwordHash = request.Password;

            var user = _authRepository.Login(request.Email, passwordHash);

            if (user == null) return Unauthorized("Credenciales inválidas.");

            var jwt = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!.Trim()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresMinutes = int.Parse(jwt["ExpiresMinutes"] ?? "60");
            var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var claims = new List<Claim>
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Value.IdUsuario.ToString()),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, user.Value.Email),
                new Claim(ClaimTypes.Name, user.Value.Nombre),
                new Claim(ClaimTypes.Role, user.Value.Rol),
                new Claim("IdRol", user.Value.IdRol.ToString())
            };

            
            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var response = new LoginResponseDto
            {
                IdUsuario = user.Value.IdUsuario,
                Nombre = user.Value.Nombre,
                Email = user.Value.Email,
                IdRol = user.Value.IdRol,
                Rol = user.Value.Rol,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expira = expires
            };

            return Ok(response);
        }
    }
}
