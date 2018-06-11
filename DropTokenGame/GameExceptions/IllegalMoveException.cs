using System;

namespace DropTokenGame.GameExceptions
{
    public class IllegalMoveException : Exception
    {
        public IllegalMoveException()
        {
        }

        public IllegalMoveException(string message) : base(message)
        {
        }

        public IllegalMoveException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
