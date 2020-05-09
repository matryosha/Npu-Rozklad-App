using System;
using System.Collections.Generic;
using System.Text;

namespace NpuRozklad.Exceptions
{
    public class RozkladLocalizationValidationException : Exception
    {
        public static RozkladLocalizationValidationException Create(
            IEnumerable<string> missingKeys,
            IEnumerable<string> emptyKeys,
            string localizationName)
        {
            var resultMessage = new StringBuilder();
            foreach (var missingKey in missingKeys)
            {
                resultMessage.AppendLine($"Key {missingKey} is missing in {localizationName}");

            }
            foreach (var emptyKey in emptyKeys)
            {
                resultMessage.AppendLine($"Key {emptyKey} is defined but empty in {localizationName}");
            }

            return new RozkladLocalizationValidationException(resultMessage.ToString());
        }
        
        protected RozkladLocalizationValidationException(string message): base(message) {}
    }
}
