using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NpuRozklad.LessonsProvider.Exceptions;

namespace NpuRozklad.LessonsProvider.Fetcher
{
    internal class NpuServerFetcher : INpuServerFetcher
    {
        private readonly NpuServerFetcherOptions _options;
        private readonly HttpClient _httpClient;

        private string CallEndPoint => _options.CallEndPoint;

        public NpuServerFetcher(NpuServerFetcherOptions options)
        {
            CheckOptions(options);
            _options = options;
            _httpClient = new HttpClient();
            InitHttpClient();
        }

        public Task<string> FetchCalendar()
        {
            return SendPost(NpuRequestType.GetCalendar);
        }

        public Task<string> FetchSettings()
        {
            return SendPost(NpuRequestType.GetSettings);
        }

        public Task<string> FetchFaculties()
        {
            return SendPost(NpuRequestType.GetFaculties);
        }

        public Task<string> FetchLecturers(string facultyTypeid)
        {
            return SendPost(NpuRequestType.GetLecturers, facultyTypeid);
        }

        public Task<string> FetchClassroom(string facultyTypeid)
        {
            return SendPost(NpuRequestType.GetClassrooms, facultyTypeid);
        }

        public Task<string> FetchGroups(string facultyTypeid)
        {
            return SendPost(NpuRequestType.GetFacultyGroups, facultyTypeid);
        }

        private async Task<string> SendPost(NpuRequestType requestType, string faculty = "fi")
        {
            var content = GetDefaultContent(NpuRequestTypeToCode(requestType), faculty);
            HttpResponseMessage response;
            
            try
            {
                response = await _httpClient.PostAsync(CallEndPoint, content).ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                throw new NpuServerFetchException("Cannot fetch npu server", e);
            }

            if (!response.IsSuccessStatusCode)
                throw new NpuServerFetchException($"Fetch npu response status is {response.StatusCode}");
            
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private FormUrlEncodedContent GetDefaultContent(string code, string faculty)
        {
            return new FormUrlEncodedContent(new []
            {
                new KeyValuePair<string, string>(_options.RequestOptions.Params.Key, _options.RequestOptions.Params.Value),
                new KeyValuePair<string, string>(_options.RequestOptions.LoginPass.Key, _options.RequestOptions.LoginPass.Value),
                new KeyValuePair<string, string>(_options.RequestOptions.Code, code),
                new KeyValuePair<string, string>(_options.RequestOptions.Faculty, faculty)
            });
        }
        
        private static string NpuRequestTypeToCode(NpuRequestType requestType) =>
            requestType switch
            {
                NpuRequestType.GetCalendar => "get calendar",
                // ReSharper disable once StringLiteralTypo
                NpuRequestType.GetLecturers => "get lectors",
                // ReSharper disable once StringLiteralTypo
                NpuRequestType.GetClassrooms => "get auditories",
                NpuRequestType.GetFaculties => "get faculties",
                NpuRequestType.GetFacultyGroups => "get groups",
                NpuRequestType.GetSettings => "get settings",
                _ => ""
            };


        private void InitHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_options.BaseAddress);
            _httpClient.DefaultRequestHeaders.Add(HttpRequestHeader.Accept.ToString(),
                _options.RequestOptions.Headers[HttpRequestHeader.Accept]);
            _httpClient.DefaultRequestHeaders.Add(HttpRequestHeader.ContentType.ToString(),
                _options.RequestOptions.Headers[HttpRequestHeader.ContentType]);
        }

        private void CheckOptions(NpuServerFetcherOptions options)
        {
            // TODO
        }
    }
}