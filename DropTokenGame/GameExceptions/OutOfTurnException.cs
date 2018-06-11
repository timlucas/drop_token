using System;

namespace DropTokenGame.GameExceptions
{
    public class OutOfTurnException : Exception
    {
        public OutOfTurnException()
        {
        }

        public OutOfTurnException(string message) : base(message)
        {
        }

        public OutOfTurnException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
