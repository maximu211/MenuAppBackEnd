using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MenuApp.BLL.Configuration;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

namespace MenuApp.BLL.Utils.Authorization
{
    public interface IGenerateJwtToken
    {
        string GenerateNewJwtToken(string userId);
        string GenerateRefreshToken(string userId);
        ObjectId GetUserIdFromJwtToken(string token);
    }

    public class GenerateJwtToken : IGenerateJwtToken
    {
        private readonly IOptions<JwtSettings> _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GenerateJwtToken(
            IOptions<JwtSettings> jwtSettings,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _jwtSettings = jwtSettings;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateNewJwtToken(string userId)
        {
            Claim[] claims = [new("userId", userId)];

            var jwt = new JwtSecurityToken(
                issuer: _jwtSettings.Value.Issuer,
                audience: _jwtSettings.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromHours(3)),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.JwtKey)),
                    SecurityAlgorithms.HmacSha256
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public string GenerateRefreshToken(string userId)
        {
            string combinedData = userId + Guid.NewGuid().ToString();

            byte[] bytes = Encoding.UTF8.GetBytes(combinedData);
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public ObjectId GetUserIdFromJwtToken(string claim)
        {
            if (string.IsNullOrEmpty(claim))
            {
                return ObjectId.Empty;
            }

            if (ObjectId.TryParse(claim, out ObjectId userId))
            {
                return userId;
            }
            else
            {
                throw new ArgumentException("Invalid userId claim value");
            }
        }
    }
}
