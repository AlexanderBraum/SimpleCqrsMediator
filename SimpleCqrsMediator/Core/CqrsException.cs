using System;

namespace SimpleCqrsMediator.Core
{
    public class CqrsException : Exception
    {
        public CqrsException(string message)
        : base(message)
        {
        }
    }
}
