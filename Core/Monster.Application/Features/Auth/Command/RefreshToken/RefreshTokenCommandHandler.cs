﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Monster.Application.Features.Auth.Command.RefreshToken
{
    public class RefreshTokenCommandHandler : BaseHandler, IRequestHandler<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
    {
        private readonly AuthRules authRules;
        private readonly UserManager<User> userManager;
        private readonly ITokenService tokenService;
        public RefreshTokenCommandHandler(IMapper mapper, AuthRules authRules, IUnitOfWorks unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, ITokenService tokenService) : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.authRules = authRules;
            this.userManager = userManager;
            this.tokenService = tokenService;
        }

        public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
        {
            // Access token'dan kullanıcıya ait bilgileri al.
            ClaimsPrincipal? principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            string email = principal.FindFirstValue(ClaimTypes.Email);

            // Kullanıcıyı e-posta adresine göre bul.
            User? user = await userManager.FindByEmailAsync(email);

            // Kullanıcının rollerini al.
            IList<string> roles = await userManager.GetRolesAsync(user);

            // Yenileme tokeninin süresinin dolup dolmadığını kontrol et.
            await authRules.RefreshTokenShouldNotBeExpired(user.RefreshTokenExpiryTime);

            // Yeni bir JWT token oluştur.
            JwtSecurityToken newAccessToken = await tokenService.CreateToken(user, roles);

            // Yeni bir yenileme tokeni oluştur.
            string newRefreshToken = tokenService.GenerateRefreshToken();

            // Kullanıcıya yeni yenileme tokenini ata ve güncelle.
            user.RefreshToken = newRefreshToken;
            await userManager.UpdateAsync(user);

            // Yenilenen tokenları içeren bir yanıt nesnesi oluştur ve döndür.
            return new RefreshTokenCommandResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken,
            };
        }
    }
}
