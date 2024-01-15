using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Monster.Application.Bases;
using Monster.Application.Features.Auth.Rules;
using Monster.Application.Interfaces.AutoMapper;
using Monster.Application.Interfaces.Tokens;
using Monster.Application.Interfaces.UnitOfWorks;
using Monster.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monster.Application.Features.Auth.Command.Login
{
    public class LoginCommandHandler : BaseHandler, IRequestHandler<LoginCommandRequest, LoginCommandResponse>
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly ITokenService tokenService;
        private readonly AuthRules authRules;

        public LoginCommandHandler(UserManager<User> userManager, IConfiguration configuration, ITokenService tokenService, AuthRules authRules,
            IMapper mapper, IUnitOfWorks unitOfWork, IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.tokenService = tokenService;
            this.authRules = authRules;
        }
        public async Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        {
            // Kullanıcıyı e-posta adresine göre bul.
            User user = await userManager.FindByEmailAsync(request.Email);

            // Kullanıcının şifresini kontrol et.
            bool checkPassword = await userManager.CheckPasswordAsync(user, request.Password);

            // E-posta veya şifre geçersizse ilgili kurala göre istisna fırlat.
            await authRules.EmailOrPasswordShouldNotBeInvalid(user, checkPassword);

            // Kullanıcının rollerini al.
            IList<string> roles = await userManager.GetRolesAsync(user);

            // Yeni bir JWT token oluştur.
            JwtSecurityToken token = await tokenService.CreateToken(user, roles);

            // Yenileme tokeni oluştur.
            string refreshToken = tokenService.GenerateRefreshToken();

            // Konfigürasyondan yenileme tokeni geçerlilik süresini al ve ayarla.
            _ = int.TryParse(configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);


            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
            // Kullanıcıyı güncelle.
            await userManager.UpdateAsync(user);

            // Kullanıcının UpdateSecurityStampAsync güncelle.
            await userManager.UpdateSecurityStampAsync(user);

            // Yeni oluşturulan JWT tokenini string olarak al.
            string _token = new JwtSecurityTokenHandler().WriteToken(token);

            // Kullanıcıya ait AccessToken'ı ayarla.
            await userManager.SetAuthenticationTokenAsync(user, "Default", "AccessToken", _token);

            // Başarılı giriş için yanıt oluştur ve döndür.
            return new()
            {
                Token = _token,
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            };

        }
    }
}
