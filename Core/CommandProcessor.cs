using SimpleCqrsMediator.Interface;
using SimpleCqrsMediator.Interface.Core;
using System;
using System.Threading.Tasks;

namespace SimpleCqrsMediator.Core
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly IProcessorExceptionHandler ProcessorExceptionHandler;

        public CommandProcessor(
            IServiceProvider serviceProvider,
            IProcessorExceptionHandler processorExceptionHandler)
        {
            ServiceProvider = serviceProvider;
            ProcessorExceptionHandler = processorExceptionHandler;
        }


        public Task ProcessAsync(ICommand command)
        {
            try
            {
                return TryProcessAsync(command);
            }
            catch (Exception ex)
            {
                ProcessorExceptionHandler.HandleException(ex);
                return default;
            }
        }

        private Task TryProcessAsync(ICommand command)
        {
            var commandType = command.GetType();
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            var handler = ServiceProvider.GetService(handlerType);

            if (handler is null)
            {
                throw new CqrsException($"Dependency {handlerType.FullName} for command type {commandType.FullName} cannot be resolved.");
            }

            var handleAsyncMethod = handlerType.GetMethod("HandleAsync");
            if (handleAsyncMethod == null)
            {
                throw new CqrsException($"HandleAsync method not found on handler {handlerType.FullName}.");
            }

            var task = handleAsyncMethod.Invoke(handler, new object[] { command }) as Task;
            if (task is null)
            {
                throw new CqrsException($"HandleAsync invocation returned null for handler {handlerType.FullName}.");
            }

            return task;
        }

        public Task<TResult> ProcessAsync<TResult>(ICommand<TResult> command)
        {
            try
            {
                return TryProcessAsync(command);
            }
            catch (Exception ex)
            {
                ProcessorExceptionHandler.HandleException(ex);
                return default;
            }
        }

        private async Task<TResult> TryProcessAsync<TResult>(ICommand<TResult> command)
        {
            var commandType = command.GetType();
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));
            var handler = ServiceProvider.GetService(handlerType);

            if (handler is null)
            {
                throw new CqrsException($"Dependency {handlerType.FullName} for command type {commandType.FullName} cannot be resolved.");
            }

            var handleAsyncMethod = handlerType.GetMethod("HandleAsync");
            if (handleAsyncMethod == null)
            {
                throw new CqrsException($"HandleAsync method not found on handler {handlerType.FullName}.");
            }

            var task = handleAsyncMethod.Invoke(handler, new object[] { command }) as Task<TResult>;
            if (task is null)
            {
                throw new CqrsException($"HandleAsync invocation returned null for handler {handlerType.FullName}.");
            }

            return await task;
        }
    }
}
