using SimpleCqrsMediator.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SimpleCqrsMediator.Core
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly ILogger<CommandProcessor> logger;

        public CommandProcessor(
            IServiceProvider serviceProvider,
            ILogger<CommandProcessor> logger)
        {
            ServiceProvider = serviceProvider;
            this.logger = logger;
        }


        public Task ProcessAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            try
            {
                return TryProcessAsync(command);
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
        }

        private Task TryProcessAsync<TCommand>(TCommand command)
            where TCommand : ICommand
        {
            var targerHandler = ServiceProvider.GetService<ICommandHandler<TCommand>>();

            if (targerHandler == null)
            {
                throw new CqrsException($"Dependency {nameof(ICommandHandler<TCommand>)}<{nameof(command)}>, can not be resolved.");
            }

            return targerHandler.HandleAsync(command);
        }

        public Task<TResult> ProcessAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand
        {
            try
            {
                return TryProcessAsync<TCommand, TResult>(command);
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
        }

        private Task<TResult> TryProcessAsync<TCommand, TResult>(TCommand command)
            where TCommand : ICommand
        {
            var targerHandler = ServiceProvider.GetService<ICommandHandler<TCommand, TResult>>();

            if (targerHandler == null)
            {
                throw new CqrsException($"Dependency {nameof(ICommandHandler<TCommand, TResult>)}<{nameof(command)}>, can not be resolved.");
            }

            var result = targerHandler.HandleAsync(command);
            return result;
        }

        private void LogException(Exception ex)
        {
            var exceptionStr = ex.ToString();
            logger.LogError(ex, exceptionStr);
        }
    }
}
