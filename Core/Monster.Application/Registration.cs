using Microsoft.Extensions.DependencyInjection;
using Monster.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Monster.Application
{
    public static class Registration
    {

        public static void AddApplication(this IServiceCollection services)
        {
            var assembly =Assembly.GetExecutingAssembly();
            services.AddTransient<ExceptionMiddleware>();
            services.AddMediatR(cfg=> cfg.RegisterServicesFromAssembly(assembly));
        }
    }
}
