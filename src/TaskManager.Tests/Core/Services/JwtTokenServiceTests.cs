using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoFixture;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using TaskManager.Core.Enums;
using TaskManager.Core.Infrastructure.Configuration;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskManager.Tests.Core.Services;

public class JwtTokenServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IOptions<JwtSettings>> _mockJwtSettings;
    private readonly IJwtTokenService _jwtTokenService;

    public JwtTokenServiceTests()
    {
        _fixture = new Fixture();

        // Удаляем ThrowingRecursionBehavior и добавляем OmitOnRecursionBehavior для обработки рекурсивных моделей
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Мокируем JwtSettings через IOptions
        _mockJwtSettings = new Mock<IOptions<JwtSettings>>();
        _mockJwtSettings.Setup(settings => settings.Value).Returns(new JwtSettings
        {
            Key = "VeryStrongSuperSecretKeyForJWT123456!",
            Issuer = "https://myissuer.com",
            Audience = "https://myaudience.com"
        });

        // Передаем IOptions в JwtTokenService
        _jwtTokenService = new JwtTokenService(_mockJwtSettings.Object);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtToken()
    {
        // Настраиваем Employee с нужными параметрами
        _fixture.Customize<Employee>(c => c.With(e => e.Role, Role.Admin));

        // Arrange
        var employee = _fixture.Build<Employee>()
            .With(e => e.Id, Guid.NewGuid())
            .With(e => e.Name, _fixture.Create<string>())
            .With(e => e.Email, "test@example.com")
            .Create();

        // Act
        var token = _jwtTokenService.GenerateToken(employee);

        // Assert
        Assert.NotNull(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("VeryStrongSuperSecretKeyForJWT123456!");

        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "https://myissuer.com",
            ValidAudience = "https://myaudience.com",
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        Assert.NotNull(validatedToken);
        Assert.IsType<JwtSecurityToken>(validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;

        Assert.Equal(employee.Id.ToString(), jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(employee.Name, jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
        Assert.Equal(employee.Email, jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value);
        Assert.Equal(employee.Role.ToString(), jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
    }
}