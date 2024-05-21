using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChargingStation.Common.Exceptions;
using ChargingStation.Infrastructure.Identity;
using UserManagement.API.Models.Requests;
using ChargingStation.Common.Utility;

namespace UserManagement.API.Persistence;

public class JwtHandler
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtHandler(string secret, string issuer, string audience)
    {
        _secret = secret;
        _issuer = issuer;
        _audience = audience;
    }

    public string GenerateToken(InfrastructureUser user, string role, DateTime expires)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, role)
        };

        if (role == CustomRoles.Administrator)
        {
            claims = claims.Append(new Claim(ClaimTypes.Role, CustomRoles.Employee)).ToArray();
        }

        return GenerateToken(claims, expires);
    }

    public string GenerateToken(RegisterRequest registerRequest, DateTime expires)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Email, registerRequest.Email),
            new Claim(ClaimTypes.Name, $"{registerRequest.FirstName} {registerRequest.LastName}"),
            new Claim(ClaimTypes.Role, registerRequest.Role)
        };

        if (registerRequest.Role == CustomRoles.Administrator)
        {
            claims = claims.Append(new Claim(ClaimTypes.Role, CustomRoles.Employee)).ToArray();
        }

        return GenerateToken(claims, expires);
    }

    private string GenerateToken(Claim[] claims, DateTime expires)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secret);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        return tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
    }

    public ClaimsPrincipal DecodeToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            throw new BadRequestException("Invalid token");

        var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
        var principal = new ClaimsPrincipal(identity);

        return principal;
    }
}

