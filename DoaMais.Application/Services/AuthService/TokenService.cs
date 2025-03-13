using DoaMais.Application.Services.Interface;
using DoaMais.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VaultService.Interface;

namespace DoaMais.Application.Services.AuthService
{
    public class TokenService(IConfiguration configuration, IVaultClient vaultClient) : ITokenService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IVaultClient _vaultClient = vaultClient;

        public string GenerateToken(Employee user)
        {
            var jwtSecret = _configuration["KeyVaultSecrets:JwtSecret"] ?? throw new Exception();
            var secret = _vaultClient.GetSecret(jwtSecret);

            var key = Encoding.ASCII.GetBytes(secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
