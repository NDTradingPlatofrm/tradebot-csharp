using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Utilities
{
    public class AuthInfo
    {
         public string Key { get; set; }
         public string Secret { get; set; }
         public string PassPhrase { get; set; }
         public string Token { get; set; }
         public string BaseUrl { get; set; }

        public AuthInfo(string baseUrl, string key="", string secret="", string passPhrase="", string token="")
         {
             this.Key = key;
             this.Secret = secret;
             this.PassPhrase = passPhrase;
             this.Token = token;
             this.BaseUrl = baseUrl;
         }
    }
}
