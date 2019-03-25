using System;

namespace RozkladNpuAspNetCore.Exceptions
{
    public class RozkladLocalizationValidationException : Exception
    {
        public RozkladLocalizationValidationException(string message)
            :base(message)
        {
            
        }
    }
}
