using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChargingStation.Common.Rbac;
using ChargingStation.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using UserManagement.API.Models.Requests;

namespace UserManagement.API.Utility;

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
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, inviteRequest.DepotId.ToString()),
            new(ClaimTypes.Email, inviteRequest.Email),
            new(ClaimTypes.Role, inviteRequest.Role)
        };

        if(inviteRequest.Role == CustomRoles.SuperAdministrator)
            claims.Add(new Claim(ClaimTypes.Role, CustomRoles.Administrator));
        else if (inviteRequest.Role == CustomRoles.Administrator)
            claims.Add(new Claim(ClaimTypes.Role, CustomRoles.Employee));

        return GenerateToken(claims, inviteRequest.Expiration);
    }

    public string GenerateAuthToken(ApplicationUser user, List<string> roles, DateTime expires)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Email, user.Email)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return GenerateToken(claims.ToArray(), expires);
    }

    private string GenerateToken(IEnumerable<Claim> claims, DateTime expires)
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

        return tokenHandler.ValidateToken(token, validationParameters, out _);
    }
}

