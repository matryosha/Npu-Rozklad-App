using System.Collections.Generic;
using Newtonsoft.Json;

namespace RozkladNpuBot.Application.Localization
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
