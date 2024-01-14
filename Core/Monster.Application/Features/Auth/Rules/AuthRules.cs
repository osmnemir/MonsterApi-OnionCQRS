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
    }
}
