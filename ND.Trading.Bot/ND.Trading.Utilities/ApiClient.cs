using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Utilities
{
    public class ApiClient
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl = "";
        protected AuthInfo _authInfo = null;

        public ApiClient(string baseUrl, bool addDefaultHeaders)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient
              {
                 // BaseAddress = new Uri(_baseUrl)
              };

            if (addDefaultHeaders)
            {
                ConfigureHttpClient();
            }
        }

        public ApiClient(string baseUrl, HttpClientHandler handler, bool addDefaultHeaders)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient(handler)
            {
                //BaseAddress = new Uri(_baseUrl)
            };

            if (addDefaultHeaders)
            {
                ConfigureHttpClient();
            }

        }

        protected void ConfigureHttpClient()
        {
            _httpClient.DefaultRequestHeaders
                .Accept.Clear();
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "DJ's trade program");
        }

        private void AddRequestHeaders(Dictionary<string, string> hdrs)
        {

        }
    }
}
