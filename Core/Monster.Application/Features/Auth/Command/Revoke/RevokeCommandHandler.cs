﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Monster.Application.Bases;
using Monster.Application.Features.Auth.Rules;
using Monster.Application.Interfaces.AutoMapper;
using Monster.Application.Interfaces.UnitOfWorks;
using Monster.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monster.Application.Features.Auth.Command.Revoke
{
    public class RevokeCommandHandler : BaseHandler, IRequestHandler<RevokeCommandRequest, Unit>
    {
        private readonly UserManager<User> userManager;
        private readonly AuthRules authRules;

        public RevokeCommandHandler(UserManager<User> userManager, AuthRules authRules, IMapper mapper, IUnitOfWorks unitOfWork,
            IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.userManager = userManager;
            this.authRules = authRules;
        }

        public async Task<Unit> Handle(RevokeCommandRequest request, CancellationToken cancellationToken)
        {
            // Kullanıcıyı e-posta adresine göre bul.
            User user = await userManager.FindByEmailAsync(request.Email);

            // Kullanıcının e-posta adresinin geçerli olup olmadığını kontrol et.
            await authRules.EmailAddressShouldBeValid(user);

            // Kullanıcının yenileme tokenini null olarak ayarla ve güncelle.
            user.RefreshToken = null;
            await userManager.UpdateAsync(user);

            // Başarılı bir şekilde yenileme tokeni iptal edildiğini bildir.
            return Unit.Value;
        }
    }
}
