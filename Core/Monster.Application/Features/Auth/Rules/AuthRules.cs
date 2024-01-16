using Monster.Application.Bases;
using Monster.Application.Features.Auth.Exceptions;
using Monster.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monster.Application.Features.Auth.Rules
{
    public class AuthRules : BaseRules
    {
        public Task UserShouldNotBeExist(User? user)
        {
            // Eğer kullanıcı varsa UserAlreadyExistException istisnası fırlatılır.
            if (user is not null) throw new UserAlreadyExistException();

            // Kullanıcı yoksa, işlemi tamamlamış olarak Task.CompletedTask döndürülür.
            return Task.CompletedTask;
        }

        public Task EmailOrPasswordShouldNotBeInvalid(User? user, bool checkPassword)
        {
            // Eğer kullanıcı null (yok) ise veya şifre kontrolü geçerli değilse istisna fırlatılır.
            if (user is null || !checkPassword) throw new EmailOrPasswordShouldNotBeInvalidException();

            // Kullanıcı var ve şifre kontrolü geçerliyse, işlem tamamlandı olarak bildirilir.
            return Task.CompletedTask;
        }

        public Task RefreshTokenShouldNotBeExpired(DateTime? expiryDate)
        {
            if (expiryDate <= DateTime.Now) throw new RefreshTokenShouldNotBeExpiredException();
            return Task.CompletedTask;
        }
    }
}
