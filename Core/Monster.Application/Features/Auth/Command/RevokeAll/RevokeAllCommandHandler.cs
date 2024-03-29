﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Monster.Application.Bases;
using Monster.Application.Interfaces.AutoMapper;
using Monster.Application.Interfaces.UnitOfWorks;
using Monster.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monster.Application.Features.Auth.Command.RevokeAll
{
    public class RevokeAllCommandHandler : BaseHandler, IRequestHandler<RevokeAllCommandRequest, Unit>
    {
        private readonly UserManager<User> userManager;
        public RevokeAllCommandHandler(IMapper mapper, IUnitOfWorks unitOfWork, IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager) : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.userManager = userManager;
        }

        public async Task<Unit> Handle(RevokeAllCommandRequest request, CancellationToken cancellationToken)
        {
            // Tüm kullanıcıları al ve liste oluştur.
            List<User> users = await userManager.Users.ToListAsync(cancellationToken);

            // Her bir kullanıcının yenileme tokenini null olarak ayarla ve güncelle.
            foreach (User user in users)
            {
                user.RefreshToken = null;
                await userManager.UpdateAsync(user);
            }

            // Başarılı bir şekilde tüm kullanıcıların yenileme tokenleri iptal edildiğini bildir.
            return Unit.Value;
        }
    }
}
