using AutoDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SimpleCqrsMediator.Core;
using SimpleCqrsMediator.Interface;
using SimpleCqrsMediator.Interface.Core;
using System;
using System.Reflection;

namespace SimpleCqrsMediator.Extensions
{
    public static class AddCqrsMediatorExtensions
    {
        public static void AddCqrsMediator(this IServiceCollection services, Assembly[] assemblies)
        {
            services.AddScoped<IProcessorExceptionHandler, DefaultProcessorExceptionHandler>();
            services.AddScoped<ICommandProcessor, CommandProcessor>();
            services.AddScoped<IQueryProcessor, QueryProcessor>();
            services.ScanAssembliesForTypes(assemblies)
                .Where(c =>
                        c.Name.EndsWith("Handler", StringComparison.InvariantCultureIgnoreCase) ||
                        c.Name.EndsWith("Processor", StringComparison.InvariantCultureIgnoreCase) ||
                        c.Name.EndsWith("Service", StringComparison.InvariantCultureIgnoreCase)
                    )
                .RegisterTypesViaInterface(ServiceLifetime.Scoped);
        }

        public static void AddCqrsMediatorExceptionHandler<TExceptionHandler>(this IServiceCollection services)
            where TExceptionHandler : IProcessorExceptionHandler
        {
            services.AddScoped(typeof(IProcessorExceptionHandler), typeof(TExceptionHandler));
        }
    }
}
