using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ND.Trading.Utilities
{
    public class OauthClient : ApiClient, IApiClient
    {
        private static readonly object monitor = new object();
        private Common comnObject = null;
        private Dictionary<string, string> SignParams { get; set; }
        //private Dictionary<string, string> SignParams { get; set; }

        private string nonce = string.Empty;
        private string timestamp = string.Empty;

        public OauthClient(AuthInfo authInfo, Dictionary<string, string> tknUrls, Dictionary<string, string> signParams, bool addDefaultHeaders = true)
            : base(authInfo.BaseUrl, addDefaultHeaders)
        {
            _authInfo = authInfo;
            SignParams = signParams;
            comnObject = new Common();
            //This is request for token generation oAuth2.0
            GenerateOauthToken(tknUrls["request_token_url"], "POST");
            string authorize_request = tknUrls["token_authorize_url"] + SignParams["consumer_key"] + "&token=" + _authInfo.Token;
            System.Diagnostics.Process.Start(authorize_request);
            SignParams["verifier"] = Console.ReadLine();
            //This is actual token generation
            GenerateOauthToken(tknUrls["access_token_url"], "POST");
        }
        private Dictionary<String, String> OAuthResponse(string alltext)
        {
            Dictionary<String, String> _tkns = new Dictionary<String, String>();
            var kvpairs = alltext.Split('&');
            foreach (var pair in kvpairs)
            {
                var kv = pair.Split('=');
                _tkns.Add(kv[0], kv[1]);
            }
            return _tkns;
        }
        private void GenerateOauthToken(string tokenUrl, string method)
        {
            ClearHeaders();
            var authzHeader = GetAuthorizationHeader(tokenUrl, method, string.Empty);
            _httpClient.DefaultRequestHeaders.Add("Authorization", authzHeader);

            var req = new HttpRequestMessage
            {
                RequestUri = new Uri(tokenUrl),
                Method = HttpMethod.Post
            };

            var resp = _httpClient.SendAsync(req).Result;
            if (resp != null)
            {
                string tokenText = resp.Content.ReadAsStringAsync().Result;
                Dictionary<String, String> tokn = OAuthResponse(tokenText);

                _authInfo.Token = tokn["oauth_token"];
                SignParams["token"] = _authInfo.Token;

                // Sometimes the request_token URL gives us an access token,
                // with no user interaction required. Eg, when prior approval
                // has already been granted.
                try
                {
                    if (tokn["oauth_token_secret"] != null)
                    {
                        _authInfo.PassPhrase = tokn["oauth_token_secret"];
                        SignParams["token_secret"] = _authInfo.PassPhrase;
                    }
                }
                catch { }
            }
        }
        private string GetAuthorizationHeader(string uri, string method, string realm)
        {
            if (string.IsNullOrEmpty(SignParams["consumer_key"]))
                throw new ArgumentNullException("consumer_key");

            if (string.IsNullOrEmpty(SignParams["signature_method"]))
                throw new ArgumentNullException("signature_method");
            SignParams["signature"] = comnObject.GenerateSignature(uri, method, SignParams);

            var erp = comnObject.EncodeRequestParameters(SignParams);
            return (String.IsNullOrEmpty(realm))
                ? "OAuth " + erp
                : String.Format("OAuth realm=\"{0}\", ", realm) + erp;
        }
        private Dictionary<string, string> GetApiTokenParam(string consumerKey,
            string consumerSecret,
            string token,
            string tokenSecret)
        {
            return null;
        }
        private void ClearHeaders()
        {
            base._httpClient.DefaultRequestHeaders.Clear();
            base.ConfigureHttpClient();
            SignParams["nonce"] = comnObject.GenerateNonce();
            SignParams["timestamp"] = comnObject.GenerateTimeStamp(DateTime.Now);
        }

        public async Task<string> CallAsync(string xChangeName, Methods method, string endpoint, bool isSigned = false, string parameters = null, string body = null)
        {
            string rtnObject = string.Empty;

            try
            {
                ClearHeaders();
                var finalEndpoint = endpoint + (string.IsNullOrWhiteSpace(parameters) ? string.Empty : (parameters));
                var finalUri = new Uri(new Uri(this._baseUrl, UriKind.Absolute), finalEndpoint);

                var authzHeader = GetAuthorizationHeader(finalUri.ToString(), method.ToString(), string.Empty);
                _httpClient.DefaultRequestHeaders.Add("Authorization", authzHeader);


                var request = new HttpRequestMessage
                {
                    RequestUri = finalUri,
                    Method = comnObject.CreateHttpMethod(method.ToString()),
                    Content = String.IsNullOrEmpty(body) ? null : new StringContent(body, Encoding.UTF8, "application/json")
                };


                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, System.Threading.CancellationToken.None).ConfigureAwait(false);
                rtnObject = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    comnObject.LogMessageToFile(rtnObject);
                }
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
            }

            return rtnObject;
        }
    }
}
