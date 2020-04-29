using System.Collections.Generic;
using Newtonsoft.Json;

namespace NpuRozklad.Services.Localization
{
    public class RozkladLocalization
    {   
        [JsonProperty("language-short-name")]
        public string ShortName { get; set; }
        [JsonProperty("language-full-name")]
        public string FullName { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }
}
