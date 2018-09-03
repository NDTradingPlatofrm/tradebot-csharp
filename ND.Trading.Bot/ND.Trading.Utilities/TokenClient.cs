using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ND.Trading.Utilities
{
    public class TokenClient : ApiClient, IApiClient
    {
        private int apiDelay = 100;
        Dictionary<string, string> defaultHeaders = null;
        Common comnObject = null;
        private static readonly object monitor = new object();
        private static Dictionary<string, DateTime> LastRequestTimeDictionary = new Dictionary<string, DateTime>();

        public TokenClient(AuthInfo authInfo, Dictionary<string, string> headers, int delay, bool addDefaultHeaders = true)
            : base(authInfo.BaseUrl, addDefaultHeaders)
        {
            _authInfo = authInfo;
            defaultHeaders = headers;
            apiDelay = delay;
            comnObject = new Common();
            if (LastRequestTimeDictionary.ContainsKey(authInfo.Token) == false)
                LastRequestTimeDictionary.Add(authInfo.Token, DateTime.UtcNow);
        }
        private void ClearHeaders()
        {
            base._httpClient.DefaultRequestHeaders.Clear();
            base.ConfigureHttpClient();
            foreach (KeyValuePair<string, string> hdr in defaultHeaders)
            {
                base._httpClient.DefaultRequestHeaders.Add(hdr.Key, hdr.Value);
            }
        }
        public async Task<string> CallAsync(string xChangeName, Methods method, string endpoint, bool isSigned = false, string parameters = null, string body = null)
        {
            string result = string.Empty;
            try
            {
                ClearHeaders();

                lock (monitor)
                {
                    while (DateTime.UtcNow < LastRequestTimeDictionary[_authInfo.Token].AddMilliseconds(apiDelay))
                    { } // Oanda speed limit .. 100 requests/second 
                    LastRequestTimeDictionary[_authInfo.Token] = DateTime.UtcNow;
                    //Console.WriteLine(_authInfo.Token + "  -  " + LastRequestTimeDictionary[_authInfo.Token].Second + ":" + LastRequestTimeDictionary[_authInfo.Token].Millisecond);
                }

                var finalEndpoint = endpoint + (string.IsNullOrWhiteSpace(parameters) ? string.Empty : ("?" + parameters));

                //if (isSigned)
                //{
                //    parameters += (!string.IsNullOrWhiteSpace(parameters) ? "&timestamp=" : "timestamp=") + comnObject.GenerateTimeStamp(DateTime.Now);
                //}

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(new Uri(this._baseUrl, UriKind.Absolute), finalEndpoint),
                    Method = comnObject.CreateHttpMethod(method.ToString()),
                    Content = String.IsNullOrEmpty(body) ? null : new StringContent(body, Encoding.UTF8, "application/json")
                };

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    comnObject.LogMessageToFile(result);
                }
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            { }
            finally
            {

            }


            return result;
        }
    }
}
