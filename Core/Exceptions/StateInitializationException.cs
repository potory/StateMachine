using System;

namespace StateMachine.Core.Exceptions
{
    public class StateInitializationException : Exception
    {
        public StateInitializationException() : base()
        {
            
        }

        public StateInitializationException(string message) : base(message)
        {
            
        }
    }
}