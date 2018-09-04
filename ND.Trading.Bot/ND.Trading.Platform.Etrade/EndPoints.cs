using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Etrade
{
    public static class EndPoints
    {
        #region Authetication Endpoints
        public static readonly string REQUEST_TOKEN = "https://etws.etrade.com/oauth/request_token";
        public static readonly string AUTHORIZE_TOKEN = "https://us.etrade.com/e/t/etws/authorize?key=";
        public static readonly string ACCESS_TOKEN = "https://etws.etrade.com/oauth/access_token";
        public static readonly string RENEW_ACCESS_TOKEN = "https://etws.etrade.com/oauth/renew_access_token";
        #endregion

        #region Account Endpoints
        public static readonly string LIST_ACCOUNTS = "/accounts/sandbox/rest/accountlist";
        public static readonly string LIST_ACCOUNT_BALANCE = "/accounts/sandbox/rest/accountbalance/{accountId}";
        public static readonly string OPEN_POSITIONS = "/accounts/sandbox/rest/accountpositions/{accountId}";
        #endregion
    }
}
