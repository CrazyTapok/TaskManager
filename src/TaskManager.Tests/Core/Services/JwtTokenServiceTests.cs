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

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _mockJwtSettings = new Mock<IOptions<JwtSettings>>();
        _mockJwtSettings.Setup(settings => settings.Value).Returns(new JwtSettings
        {
            Key = "VeryStrongSuperSecretKeyForJWT123456!",
            Issuer = "https://myissuer.com",
            Audience = "https://myaudience.com"
        });

        _jwtTokenService = new JwtTokenService(_mockJwtSettings.Object);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtToken()
    {
        _fixture.Customize<Employee>(employee => employee.With(employee => employee.Role, Role.Admin));

        // Arrange
        var employee = _fixture.Build<Employee>()
            .With(employee => employee.Id, Guid.NewGuid())
            .With(employee => employee.Name, _fixture.Create<string>())
            .With(employee => employee.Email, "test@example.com")
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

        Assert.Equal(employee.Id.ToString(), jwtToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(employee.Name, jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value);
        Assert.Equal(employee.Email, jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Email).Value);
        Assert.Equal(employee.Role.ToString(), jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Role).Value);
    }
}