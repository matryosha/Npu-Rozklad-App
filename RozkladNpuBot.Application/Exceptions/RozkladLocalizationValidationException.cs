using System;

namespace RozkladNpuBot.Application.Exceptions
{
    public class RozkladLocalizationValidationException : Exception
    {
        public RozkladLocalizationValidationException(string message)
            :base(message)
        {
            
        }
    }
}
