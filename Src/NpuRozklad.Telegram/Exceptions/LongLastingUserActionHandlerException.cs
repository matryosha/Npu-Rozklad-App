using System;

namespace NpuRozklad.Telegram.Exceptions
{
    public class LongLastingUserActionHandlerException : Exception
    {
        public LongLastingUserActionHandlerException(int telegramUserId, Exception innerException) 
            : base($"Error handling long lasting action. User telegram id: {telegramUserId}", innerException)
        {
        }
    }
}