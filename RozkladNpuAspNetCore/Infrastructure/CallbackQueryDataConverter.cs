using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RozkladNpuAspNetCore.Infrastructure
{
    public static class CallbackQueryDataConverter
    {
        public static KeyValuePair<RozkladCallbackQueryType, List<string>> ConvertDataFromString(string dataString)
        {
            var resultValues = new List<string>();
            var parts = dataString.Split(';');
            if (parts.Length != 1)
            {
                foreach (var part in parts.Skip(1))
                {
                    resultValues.Add(part);
                }
            }
            
            return new 
                KeyValuePair<RozkladCallbackQueryType, List<string>>(
                    (RozkladCallbackQueryType) int.Parse(parts[0]), resultValues);
        }

        public static string ConvertToDataString(
            RozkladCallbackQueryType callbackQueryType,
            params string[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var result = new StringBuilder();
            result.Append((int)callbackQueryType);
            foreach (var value in values)
            {
                result.Append(';');
                result.Append(value);
            }

            return result.ToString();
        }
    }
}
