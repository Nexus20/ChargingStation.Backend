using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChargingStation.Common.Exceptions;
using ChargingStation.Infrastructure.Identity;
using ChargingStation.Common.Utility;
using UserManagement.API.Models.Requests;
using System.Data;
using ChargingStation.Domain.Entities;

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

    public string GenerateInviteToken(InviteRequest inviteRequest)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, inviteRequest.DepotId.ToString()),
            new Claim(ClaimTypes.Email, inviteRequest.Email),
            new Claim(ClaimTypes.Role, inviteRequest.Role)
        };

        if (inviteRequest.Role == CustomRoles.Administrator)
        {
            claims = claims.Append(new Claim(ClaimTypes.Role, CustomRoles.Employee)).ToArray();
        }

        return GenerateToken(claims, inviteRequest.Expiration);
    }

    public string GenerateToken(ApplicationUser user, string role, DateTime expires)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.MobilePhone, user.Phone),
            new Claim(ClaimTypes.Name, user.LastName + " " + user.LastName),
            new Claim(ClaimTypes.Role, role)
        };

        if (role == CustomRoles.Administrator)
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

