using AutoDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace SimpleCqrsMediator.Extensions
{
    public static class AddCqrsMediatorExtensions
    {
        public static void AddCqrsMediator(this IServiceCollection services, Assembly[] assemblies)
        {
            services.ScanAssembliesForTypes(assemblies)
                .Where(c =>
                        c.Name.EndsWith("Handler", StringComparison.InvariantCultureIgnoreCase) ||
                        c.Name.EndsWith("Processor", StringComparison.InvariantCultureIgnoreCase) ||
                        c.Name.EndsWith("Service", StringComparison.InvariantCultureIgnoreCase)
                    )
                .RegisterTypesViaInterface(ServiceLifetime.Scoped);
        }
    }
}
