using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Monster.Application.Interfaces.Tokens;
using Monster.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Monster.Infrastructure.Tokens
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<User> userManager;
        private readonly TokenSettings tokenSettings;

        public TokenService(IOptions<TokenSettings> options, UserManager<User> userManager)
        {
            tokenSettings = options.Value;
            this.userManager = userManager;
        }
        public async Task<JwtSecurityToken> CreateToken(User user, IList<string> roles)
        {
            // JWT içinde yer alacak talepleri temsil eden bir liste oluşturulur
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID (Jti) talebi eklenir.
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Kullanıcının benzersiz kimlik numarası talebi eklenir.
                new Claim(JwtRegisteredClaimNames.Email, user.Email) // Kullanıcının e-posta talebi eklenir.
            };
            // Kullanıcının sahip olduğu roller eklenir.
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // JWT için kullanılacak simetrik anahtar oluşturulur.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret));
            //JWT oluşturur
            var token = new JwtSecurityToken(
                issuer: tokenSettings.Issuer,// JWT'nin düzenleyeni (issuer) belirlenir.
                audience: tokenSettings.Audience,// JWT'nin kullanılacağı hedef kitle (audience) belirlenir.
                expires: DateTime.Now.AddMinutes(tokenSettings.TokenValidityInMunitues),// JWT'nin geçerlilik süresi belirlenir.
                claims: claims,// JWT içinde taşınacak talepler belirlenir
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)// JWT'nin imzalanması için gerekli bilgiler belirlenir.
                );

            // Kullanıcıya ait talepler veritabanına eklenir.
            await userManager.AddClaimsAsync(user, claims);
            //JWT döndürür
            return token;

        }

        public string GenerateRefreshToken()
        {

            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            TokenValidationParameters tokenValidationParamaters = new()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret)),
                ValidateLifetime = false
            };

            JwtSecurityTokenHandler tokenHandler = new();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParamaters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Token bulunamadı.");

            return principal;

        }
    }
}
