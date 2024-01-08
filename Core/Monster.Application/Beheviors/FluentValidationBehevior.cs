using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monster.Application.Beheviors
{
    public class FluentValidationBehevior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validator;

        public FluentValidationBehevior(IEnumerable<IValidator<TRequest>>validator)
        {
            this.validator = validator;
        }
        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Doğrulama bağlamını oluşturur.
            var context = new ValidationContext<TRequest>(request);
            
            var failtures = validator
                .Select(v => v.Validate(context))  // Her bir validator için doğrulama yapılır.
                .SelectMany(result => result.Errors)  // Doğrulama sonuçlarındaki hatalar birleştirilir.
                .GroupBy(x => x.ErrorMessage)  // Aynı hata mesajına sahip hatalar gruplandırılır.
                .Select(x => x.First())  // Her grup içindeki ilk hata seçilir.
                .Where(f => f != null)  // Null olmayan hatalar seçilir.
                .ToList();  // Seçilen hatalar bir liste haline getirilir.

            // Doğrulama hataları varsa ValidationException fırlatır.
            if (failtures.Any())
                throw new ValidationException(failtures);

            return next();
        }
    }
}
