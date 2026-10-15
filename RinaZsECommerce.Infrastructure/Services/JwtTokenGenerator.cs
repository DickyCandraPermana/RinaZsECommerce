using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RinaZsECommerce.Application.Interfaces;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Infrastructure.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"] ?? "RinaZsECommerceSuperSecretKey2026SecureJwtTokenForFullstackStorefront!";
        var issuer = jwtSettings["Issuer"] ?? "RinaZsECommerceAPI";
        var audience = jwtSettings["Audience"] ?? "RinaZsECommerceClient";

        var key = Encoding.UTF8.GetBytes(secret);
        var signingKey = new SymmetricSecurityKey(key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = new Dictionary<string, object>
            {
                { "sub", user.Id.ToString() },
                { "name", user.Username },
                { "email", user.Email },
                { "role", user.Role },
                { "profileId", user.ProfileId.ToString() }
            },
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
