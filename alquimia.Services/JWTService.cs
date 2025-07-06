using alquimia.Data.Entities;
using alquimia.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace alquimia.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>
{
new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
new Claim("name", user.Name ?? string.Empty)
};

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var keyString = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(keyString))
            {
                throw new InvalidOperationException("JWT signing key not configured");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
