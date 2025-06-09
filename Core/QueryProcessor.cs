using Microsoft.Extensions.Logging;
using SimpleCqrsMediator.Interface;
using SimpleCqrsMediator.Interface.Core;
using System;
using System.Threading.Tasks;

namespace SimpleCqrsMediator.Core
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly ILogger<QueryProcessor> logger;
        private readonly IProcessorExceptionHandler ProcessorExceptionHandler;

        public QueryProcessor(
            IServiceProvider serviceProvider,
            IProcessorExceptionHandler processorExceptionHandler,
            ILogger<QueryProcessor> logger)
        {
            ServiceProvider = serviceProvider;
            ProcessorExceptionHandler = processorExceptionHandler;
            this.logger = logger;
        }

        public async Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query)
        {
            try
            {
                return await TryProcessAsync(query);
            }
            catch (Exception ex)
            {
                ProcessorExceptionHandler.HandleException(ex);
                return default;
            }
        }

        public async Task<TResult> TryProcessAsync<TResult>(IQuery<TResult> query)
        {
            var queryType = query.GetType();
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));
            var handler = ServiceProvider.GetService(handlerType);

            if (handler is null)
            {
                throw new CqrsException($"Dependency {handlerType.FullName} for query type {queryType.FullName} cannot be resolved.");
            }

            var handleAsyncMethod = handlerType.GetMethod("HandleAsync");
            if (handleAsyncMethod == null)
            {
                throw new CqrsException($"HandleAsync method not found on handler {handlerType.FullName}.");
            }

            var task = handleAsyncMethod.Invoke(handler, new object[] { query }) as Task<TResult>;
            if (task is null)
            {
                throw new CqrsException($"HandleAsync invocation returned null for handler {handlerType.FullName}.");
            }

            return await task;
        }
    }
}
