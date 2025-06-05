using SimpleCqrsMediator.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SimpleCqrsMediator.Core
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly ILogger<QueryProcessor> logger;

        public QueryProcessor(
            IServiceProvider serviceProvider,
            ILogger<QueryProcessor> logger)
        {
            ServiceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task<TResult> ProcessAsync<TQuery, TResult>(TQuery query)
            where TQuery : IQuery
        {
            try
            {
                return await TryProcessAsync<TQuery, TResult>(query);
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
        }

        private async Task<TResult> TryProcessAsync<TQuery, TResult>(TQuery query)
            where TQuery : IQuery
        {
            var targerHandler = ServiceProvider.GetService<IQueryHandler<TQuery, TResult>>();

            if (targerHandler == null)
            {
                throw new CqrsException($"Dependency {nameof(IQueryHandler<TQuery, TResult>)}<{nameof(query)}>, can not be resolved.");
            }

            var result = await targerHandler.HandleAsync(query);
            return result;
        }

        private void LogException(Exception ex)
        {
            var exceptionStr = ex.ToString();
            logger.LogError(exceptionStr);
        }
    }
}
