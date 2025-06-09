using Microsoft.Extensions.Logging;
using SimpleCqrsMediator.Interface.Core;
using System;

namespace SimpleCqrsMediator.Core
{
    public class DefaultProcessorExceptionHandler: IProcessorExceptionHandler
    {
        private readonly ILogger<QueryProcessor> Logger;
        public DefaultProcessorExceptionHandler(ILogger<QueryProcessor> logger)
        {
            Logger = logger;
        }

        public void HandleException(Exception ex)
        {
            var exceptionStr = ex.ToString();
            Logger.LogError(exceptionStr);
            throw ex;
        }
    }
}
