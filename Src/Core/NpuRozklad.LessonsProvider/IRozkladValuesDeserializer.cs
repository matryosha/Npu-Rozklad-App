using System.Collections.Generic;
using NpuRozklad.Parser.Entities;

namespace NpuRozklad.Parser
{
    /// <summary>
    /// Uses for deserializing values from npu server 
    /// </summary>
    interface IRozkladValuesDeserializer
    {
        /// <summary>
        /// Rozklad values deserializer
        /// </summary>
        /// <param name="rawString"></param>
        /// <returns></returns>
        List<T> DeserializeValues<T>(string rawString) where T: class, new();
    }
}