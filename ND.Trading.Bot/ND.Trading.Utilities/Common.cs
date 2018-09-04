using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Utilities
{
    public class Common
    {
        private static readonly object logLock = new object();
        public Random random;
        public static string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        public static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public Common()
        { random = new Random(); }

        #region "Oauth - Etrade"
        #region "Public methods"
        public string GenerateTimeStamp(DateTime baseDateTime)
        {
            //DateTimeOffset dtOffset = new DateTimeOffset(baseDateTime);
            //return dtOffset.ToUnixTimeMilliseconds().ToString();

            TimeSpan ts = DateTime.UtcNow - _epoch;
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        public HttpMethod CreateHttpMethod(string method)
        {
            switch (method.ToUpper())
            {
                case "DELETE":
                    return HttpMethod.Delete;
                case "POST":
                    return HttpMethod.Post;
                case "PUT":
                    return HttpMethod.Put;
                case "GET":
                    return HttpMethod.Get;
                default:
                    throw new NotImplementedException();
            }
        }
        public string GenerateNonce()
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                int g = random.Next(3);
                switch (g)
                {
                    case 0:
                        // lowercase alpha
                        sb.Append((char)(random.Next(26) + 97), 1);
                        break;
                    default:
                        // numeric digits
                        sb.Append((char)(random.Next(10) + 48), 1);
                        break;
                }
            }
            return sb.ToString();
        }
        public string GenerateSignature(string uri, string method, Dictionary<string, string> _params)
        {
            var signatureBase = GetSignatureBase(uri, method, _params);
            var hash = GetHash(_params);

            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(signatureBase);
            byte[] hashBytes = hash.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }
        public string EncodeRequestParameters(ICollection<KeyValuePair<String, String>> p)
        {
            var sb = new System.Text.StringBuilder();
            foreach (KeyValuePair<String, String> item in p.OrderBy(x => x.Key))
            {
                if (!String.IsNullOrEmpty(item.Value) &&
                    !item.Key.EndsWith("secret") && !item.Key.EndsWith("token"))
                    {
                    sb.AppendFormat("oauth_{0}=\"{1}\", ",
                        item.Key,
                        UrlEncode(item.Value));
                }
                else if (!String.IsNullOrEmpty(item.Value) &&
                        item.Key.EndsWith("token"))
                {
                    sb.AppendFormat("oauth_{0}=\"{1}\", ",
                        item.Key, item.Value);
                }
            }

            return sb.ToString().TrimEnd(' ').TrimEnd(',');
        }
        #endregion
        #region "Private methods"
        private HashAlgorithm GetHash(Dictionary<string, string> _params)
        {
            if (_params["signature_method"] != "HMAC-SHA1")
                throw new NotImplementedException();

            string keystring = string.Format("{0}&{1}",
                UrlEncode(_params["consumer_secret"]),
                _params["token_secret"]);
            var hmacsha1 = new HMACSHA1
            {
                Key = System.Text.Encoding.ASCII.GetBytes(keystring)
            };
            return hmacsha1;
        }
        private string GetSignatureBase(string url, string method, Dictionary<string, string> _params)
        {
            // normalize the URI
            var uri = new Uri(url);
            var normUrl = string.Format("{0}://{1}", uri.Scheme, uri.Host);
            if (!((uri.Scheme == "http" && uri.Port == 80) ||
                  (uri.Scheme == "https" && uri.Port == 443)))
                normUrl += ":" + uri.Port;

            normUrl += uri.AbsolutePath;

            // the sigbase starts with the method and the encoded URI
            var sb = new System.Text.StringBuilder();
            sb.Append(method)
                .Append('&')
                .Append(UrlEncode(normUrl))
                .Append('&');

            // the parameters follow - all oauth params plus any params on
            // the uri
            // each uri may have a distinct set of query params
            var p = ExtractQueryParameters(uri.Query);
            // add all non-empty params to the "current" params
            foreach (var p1 in _params)
            {
                // Exclude all oauth params that are secret or
                // signatures; any secrets should be kept to ourselves,
                // and any existing signature will be invalid.

                if (!String.IsNullOrEmpty(_params[p1.Key]) &&
                    !p1.Key.EndsWith("secret")
                    && !p1.Key.EndsWith("signature"))
                    p.Add("oauth_" + p1.Key, p1.Value);
                //if (!String.IsNullOrEmpty(p1.Value) && !p1.Key.EndsWith("secret") && !p1.Key.EndsWith("token"))
                //{
                //    p.Add(p1.Key, p1.Value);
                //}

            }

            // concat+format all those params
            var sb1 = new System.Text.StringBuilder();
            foreach (KeyValuePair<String, String> item in p.OrderBy(x => x.Key))
            {
                // even "empty" params need to be encoded this way.
                sb1.AppendFormat("{0}={1}&", item.Key, item.Value);
            }

            // append the UrlEncoded version of that string to the sigbase
            sb.Append(UrlEncode(sb1.ToString().TrimEnd('&')));
            var result = sb.ToString();
            return result;
        }
        private string UrlEncode(string value)
        {
            var result = new System.Text.StringBuilder();
            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                    result.Append(symbol);
                else
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
            }
            return result.ToString();
        }
        private Dictionary<String, String> ExtractQueryParameters(string queryString)
        {
            if (queryString.StartsWith("?"))
                queryString = queryString.Remove(0, 1);

            var result = new Dictionary<String, String>();

            if (string.IsNullOrEmpty(queryString))
                return result;

            foreach (string s in queryString.Split('&'))
            {
                if (!string.IsNullOrEmpty(s) && !s.StartsWith("oauth_"))
                {
                    if (s.IndexOf('=') > -1)
                    {
                        string[] temp = s.Split('=');
                        result.Add(temp[0], temp[1]);
                    }
                    else
                        result.Add(s, string.Empty);
                }
            }

            return result;
        }
        #endregion
        #endregion

        #region "Common - Log method"
        public void LogMessageToFile(string msg)
        {
            System.IO.StreamWriter sw = null;

            try
            {
                lock (logLock)
                {
                    sw = System.IO.File.AppendText(
                    System.IO.Directory.GetCurrentDirectory().ToString() + "Botlog.txt");
                    string logLine = System.String.Format(
                        "{0:G}: {1}.", System.DateTime.Now, msg);
                    sw.WriteLine(logLine);
                }
            }
            finally
            {
                sw.Close();
            }
        }
        public void SendMail(string message)
        {
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential("dinu.john@outlook.com", "Ch!nju123");
            client.EnableSsl = true;
            client.Credentials = credentials;

            try
            {
                var mail = new MailMessage("dinu.john@outlook.com", "dinu.john@outlook.com");
                mail.Subject = "********* QUICK BUY TRIGGERED *******";
                mail.Body = message;
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
        #endregion
    }
}
