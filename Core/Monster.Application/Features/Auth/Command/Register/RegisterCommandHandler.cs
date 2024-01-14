using MediatR;
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

namespace Monster.Application.Features.Auth.Command.Register
{
    public class RegisterCommandHandler : BaseHandler, IRequestHandler<RegisterCommandRequest, Unit>
    {
        private readonly AuthRules authRules;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;

        public RegisterCommandHandler(AuthRules authRules, UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper,
            IUnitOfWorks unitOfWork, IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.authRules = authRules;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task<Unit> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
        {
            // Kullanıcının var olmamasını kontrol eden kurala uygunluk kontrolü yapılır.
            await authRules.UserShouldNotBeExist(await userManager.FindByEmailAsync(request.Email));

            // Kullanıcı verisi oluşturulur ve haritalama yapılır.
            User user = mapper.Map<User, RegisterCommandRequest>(request);
            user.UserName = request.Email;
            user.SecurityStamp = Guid.NewGuid().ToString();

            // Kullanıcı oluşturma işlemi yapılır.
            IdentityResult result = await userManager.CreateAsync(user, request.Password);

            // Kullanıcı oluşturma başarılıysa ilgili roller oluşturulur ve kullanıcıya atanır.
            if (result.Succeeded)
            {
                // "user" rolü var mı diye kontrol edilir, yoksa oluşturulur.
                if (!await roleManager.RoleExistsAsync("user"))
                    await roleManager.CreateAsync(new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = "user",
                        NormalizedName = "USER",
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                    });

                // Kullanıcıya "user" rolü atanır.
                await userManager.AddToRoleAsync(user, "user");
            }

            return Unit.Value;
        }
    }
}
