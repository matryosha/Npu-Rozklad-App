using System;

namespace NpuRozklad.LessonsProvider.Exceptions
{
    public class NpuServerFetchException : Exception
    {
        public NpuServerFetchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NpuServerFetchException(string message) : base(message)
        {
        }
    }
}