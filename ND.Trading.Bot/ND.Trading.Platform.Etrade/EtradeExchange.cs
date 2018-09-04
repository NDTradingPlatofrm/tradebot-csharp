using ND.Trading.Bot.Core;
using ND.Trading.Platform.Models;
using ND.Trading.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Etrade
{
    public class EtradeExchange : Exchange
    {
        protected EtradeConvert converter;

        public EtradeExchange(ExchangeConfig config)
           : base(config)
        {
            Dictionary<string, string> tokenUrls = new Dictionary<string, string>()
            {
                { "request_token_url", EndPoints.REQUEST_TOKEN},
                { "token_authorize_url", EndPoints.AUTHORIZE_TOKEN},
                { "access_token_url", EndPoints.ACCESS_TOKEN},
                { "renew_access_token_url", EndPoints.RENEW_ACCESS_TOKEN},
            };
            Dictionary<string, string> signParams = new Dictionary<string, string>()
            {

                { "callback", "oob"},
                { "consumer_key", this.AuthVal.Key},
                { "nonce", string.Empty},
                { "timestamp", string.Empty},
                { "signature_method", "HMAC-SHA1"},
                { "version", "1.0"},
                { "consumer_secret", this.AuthVal.Secret},
                { "token", string.Empty},
                { "token_secret", string.Empty},
                { "verifier", string.Empty}
            };

            ApiClient = new OauthClient(this.AuthVal, tokenUrls, signParams);
            converter = new EtradeConvert();
        }
        public override bool CloseTrade(TradeData trdData, string accountId)
        {
            throw new NotImplementedException();
        }

        public override void GetAccountChanges(IAccount acnt)
        {
            throw new NotImplementedException();
        }

        public override IAccount GetAccountDetails(string accountId)
        {
            IAccount result = null;

            string method = EndPoints.LIST_ACCOUNT_BALANCE;
            method = method.Replace("{accountId}", accountId);

            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET,
                         method, false, string.Empty).Result;
            if (jsonResult != string.Empty)
            {
                dynamic data = JObject.Parse(jsonResult);
                result = converter.DeserializeObject<EtradeAccount>(data);
            }

            return result;
        }

        public override decimal GetAvailablePrinciple(string currency)
        {
            throw new NotImplementedException();
        }

        public override decimal GetCurrentPrice(string symbol, string mode)
        {
            throw new NotImplementedException();
        }

        public override decimal GetLastTradePrice(string tradeId, string accountId)
        {
            throw new NotImplementedException();
        }

        public override Position GetOpenPosition(string symbol, string accountId)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Position> GetOpenPositions(string accountId)
        {
            List<Position> pList = null;
            Position pos = null;
            string method = EndPoints.OPEN_POSITIONS;
            method = method.Replace("{accountId}", accountId);

            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET,
                         method, false, string.Empty).Result;
            if (jsonResult != string.Empty)
            {
                JObject jsonObject = JObject.Parse(jsonResult);
                JObject posJsonObject = jsonObject["json.accountPositionsResponse"].Value<JObject>();
                JArray posArray = posJsonObject["response"].Value<JArray>();

                pList = new List<Position>();
                foreach (JObject item in posArray)
                {
                    pos = converter.DeserializeObject<StockPosition>(item);
                    pList.Add(pos);
                }
            }

            return pList;
        }

        public override IEnumerable<TradeData> GetOpenTrades(string symbol, string accountId)
        {
            throw new NotImplementedException();
        }

        public override Order GetOrderInfo(string orderId)
        {
            throw new NotImplementedException();
        }

        public override Quote GetQuote(string symbol)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Quote> GetQuoteList(string accountId, string mode)
        {
            throw new NotImplementedException();
        }

        public override List<TDMData> GetSymbolPriceList(string symbol, DateTime startDate)
        {
            throw new NotImplementedException();
        }

        public override bool PlaceOrder(Order order, string accountId)
        {
            throw new NotImplementedException();
        }
    }
}
