using System.Collections.Generic;
using System.Net;

namespace NpuRozklad.LessonsProvider.Fetcher
{
    public class NpuServerFetcherOptions
    {
        public string BaseAddress { get; set; }
        public string CallEndPoint { get; set; }
        public NpuServerFetcherRequestOptions RequestOptions { get; set; }

        public static NpuServerFetcherRequestOptions DefaultNpuRequestFetcherOptions()
        {
            return new NpuServerFetcherRequestOptions
            {
                Code = "code",
                Faculty = "faculty",
                Params = new KeyValuePair<string, string>("params", string.Empty),
                LoginPass = new KeyValuePair<string, string>("loginpass", string.Empty),
                Headers = new Dictionary<HttpRequestHeader, string>
                {
                    {HttpRequestHeader.ContentType, "application/x-www-form-urlencoded"},
                    {HttpRequestHeader.Accept, "application/json"}
                }
            };
        }
    }
}