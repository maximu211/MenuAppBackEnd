using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MenuApp.BLL.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.FileIO;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Newtonsoft.Json;

namespace MenuApp.BLL.Utils
{
    public interface IGenerateJwtToken
    {
        string GenerateNewJwtToken(string userId);
        string GenerateRefreshToken(string userId);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        ObjectId GetUserIdFromJwtToken(string token);
    }

    public class GenerateJwtToken : IGenerateJwtToken
    {
        private readonly IOptions<JwtSettings> _jwtSettings;

        public GenerateJwtToken(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateNewJwtToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Value.JwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] { new Claim("UserId", userId) }),
                Expires = DateTime.UtcNow.AddDays(1),
                IssuedAt = DateTime.UtcNow,
                Audience = _jwtSettings.Value.Audience,
                Issuer = _jwtSettings.Value.Issuer,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_jwtSettings.Value.JwtKey)
                ),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Value.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Value.Audience,
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(
                    token,
                    tokenValidationParameters,
                    out var securityToken
                );
                if (!IsJwtWithValidSecurityAlgorithm(securityToken))
                {
                    return new ClaimsPrincipal();
                }
                return principal;
            }
            catch
            {
                return new ClaimsPrincipal();
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken securityToken)
        {
            return securityToken is JwtSecurityToken jwtSecurityToken
                && jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase
                );
        }

        public ObjectId GetUserIdFromJwtToken(string token)
        {
            // Розділити токен на частини
            string[] tokenParts = token.Split('.');

            if (tokenParts.Length != 3)
            {
                throw new Exception("Invalid JWT token format.");
            }

            // Декодувати корисну інформацію (payload), яка знаходиться в другій частині токену
            string encodedPayload = tokenParts[1];
            string decodedPayload = Encoding.UTF8.GetString(
                Base64UrlEncoder.DecodeBytes(encodedPayload)
            );

            // Розпарсити декодований JSON, щоб отримати клейми
            JwtPayload payload = Newtonsoft.Json.JsonConvert.DeserializeObject<JwtPayload>(
                decodedPayload
            );

            // Отримати ідентифікатор користувача з клейма "UserId"
            if (payload.TryGetValue("UserId", out var userIdClaim) && userIdClaim != null)
            {
                if (ObjectId.TryParse(userIdClaim.ToString(), out ObjectId userId))
                {
                    return userId;
                }
                else
                {
                    throw new Exception("User id claim in JWT token is not in the correct format.");
                }
            }

            throw new Exception("User id claim not found in JWT token.");
        }
    }
}
