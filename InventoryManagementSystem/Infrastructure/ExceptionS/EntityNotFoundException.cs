using System;

namespace Infrastructure.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException() { }

        public EntityNotFoundException(string message) : base(message) { }

        // Constructor with custom message and inner exception
        public EntityNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
