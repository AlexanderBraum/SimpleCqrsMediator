using System;

namespace SimpleCqrsMediator.Interface.Core
{
    public interface IProcessorExceptionHandler
    {
        public void HandleException(Exception ex);
    }
}
