using System;

namespace NpuRozklad.Telegram.Exceptions
{
    public class CurrentUserInitializationException : Exception
    {
        private const string HeadMessage = "Can't initialize current telegram user:";

        public CurrentUserInitializationException(string message)
            : base($"{HeadMessage} {message}")
        { }

        public CurrentUserInitializationException(string message, Exception innerException) 
            : base($"{HeadMessage} {message}", innerException)
        {
        }
    }
}