using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RetailCompare.API.Data;

namespace RetailCompare.API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var secretKey = config["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key configuration is missing.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds,
            Issuer = config["Jwt:Issuer"] ?? "RetailCompareAPI",
            Audience = config["Jwt:Audience"] ?? "RetailCompareApp"
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}