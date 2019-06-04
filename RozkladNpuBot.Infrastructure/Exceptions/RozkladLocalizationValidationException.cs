using System;

namespace RozkladNpuBot.Infrastructure.Exceptions
{
    public class RozkladLocalizationValidationException : Exception
    {
        public RozkladLocalizationValidationException(string message)
            :base(message)
        {
            
        }
    }
}
