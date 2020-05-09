using System;

namespace NpuRozklad.LessonsProvider.Exceptions
{
    public class DeserializationException : Exception
    {
        public DeserializationException(string message) : base(message)
        {
        }

        public DeserializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}