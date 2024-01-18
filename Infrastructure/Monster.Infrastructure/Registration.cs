using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Monster.Application.Interfaces.RedisCache;
using Monster.Application.Interfaces.Tokens;
using Monster.Infrastructure.RedisCache;
using Monster.Infrastructure.Tokens;
 using System.Text;

namespace Monster.Infrastructure
{
    public static class Registration
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TokenSettings>(configuration.GetSection("JWT"));


            // ITokenService arayüzüne ve TokenService sınıfına bir servis ekler.
            services.AddTransient<ITokenService, TokenService>();

            services.Configure<RedisCacheSettings>(configuration.GetSection("RedisCacheSettings"));
            services.AddTransient<IRedisCacheService,RedisCacheService>();

            // Kimlik doğrulama yapılandırması eklenir.
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // JwtBearerAuthenticationScheme için yapılandırma eklenir.
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
            {
                // Token'in kaydedilip kaydedilmeyeceğini belirtir.
                opt.SaveToken = true;

                // Token doğrulama parametreleri ayarlanır.
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,  // İssuer doğrulaması devre dışı bırakılır.
                    ValidateAudience = false,  // Audience doğrulaması devre dışı bırakılır.
                    ValidateIssuerSigningKey = true,  // İmza anahtarı doğrulaması aktif edilir.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),  // İmza anahtarı belirlenir.
                    ValidateLifetime = false,  // Token süresi doğrulaması devre dışı bırakılır.
                    ValidIssuer = configuration["JWT:Issuer"],  // Geçerli Issuer belirlenir.
                    ValidAudience = configuration["JWT:Audience"],  // Geçerli Audience belirlenir.
                    ClockSkew = TimeSpan.Zero  // Saat kayması sıfır olarak ayarlanır.
                };
            });

            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = configuration["RedisCacheSettings:ConnectionString"];
                opt.InstanceName = configuration["RedisCacheSettings:InstanceName"];
            });


        }
    }
}
