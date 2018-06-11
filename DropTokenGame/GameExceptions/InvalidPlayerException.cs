using System;

namespace DropTokenGame.GameExceptions
{
    public class InvalidPlayerException : Exception
    {
        public InvalidPlayerException()
        {
        }

        public InvalidPlayerException(string message) : base(message)
        {
        }

        public InvalidPlayerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
