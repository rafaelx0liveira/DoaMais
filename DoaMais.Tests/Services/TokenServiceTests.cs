using DoaMais.Application.Services.AuthService;
using DoaMais.Domain.Entities;
using DoaMais.Tests.Utils;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using VaultService.Interface;

namespace DoaMais.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IVaultClient> _mockVaultClient;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockVaultClient = new Mock<IVaultClient>();

            _mockConfiguration
                .Setup(x => x["KeyVaultSecrets:JwtSecret"])
                .Returns("JwtSecret");

            _mockVaultClient
                .Setup(x => x.GetSecret("JwtSecret"))
                .Returns("this_is_a_very_secure_key_12345678910");

            _tokenService = new TokenService(_mockConfiguration.Object, _mockVaultClient.Object);
        }

        [Fact]
        public void GenerateToken_ShouldReturnToken()
        {
            var user = TestsUtils.CreateMockedObject<Employee>();

            var token = _tokenService.GenerateToken(user);

            Assert.NotNull(token);
        }

        [Fact]
        public void GenerateToken_ShouldReturnTokenWithClaims()
        {
            var user = new Employee
            {
                Id = Guid.NewGuid(), 
                Email = "user@example.com",
                Role = Domain.Entities.Enums.EmployeeRole.Admin
            };

            var token = _tokenService.GenerateToken(user);

            Assert.NotNull(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenS = tokenHandler.ReadJwtToken(token);

            Assert.NotNull(tokenS);

            Assert.Equal(user.Id.ToString(), tokenS.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value);
            Assert.Equal(user.Email, tokenS.Claims.FirstOrDefault(x => x.Type == "email")?.Value);
            Assert.Equal(user.Role.ToString(), tokenS.Claims.FirstOrDefault(x => x.Type == "role")?.Value);
        }

    }
}
