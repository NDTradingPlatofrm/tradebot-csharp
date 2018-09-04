using ND.Trading.Bot.Core;
using ND.Trading.Platform.Models;
using ND.Trading.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Oanda
{
    public class OandaExchange : Exchange
    {

        // private string accountId = string.Empty;
        protected OandaConvert converter;
        // private static DateTime XDtTest = DateTime.UtcNow;

        public OandaExchange(ExchangeConfig config)
            : base(config)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer "  + this.AuthVal.Token}/*,
                { "Accept-Encoding", "gzip, deflate" },*/
            };

            ApiClient = new TokenClient(this.AuthVal, headers, this.ApiDelay);
            converter = new OandaConvert();
        }

        #region "Account Services"
        private IEnumerable<string> GetAccountIdList()
        {

            IEnumerable<string> result = null;

            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET,
                         EndPoints.LIST_ACCOUNTS, false, string.Empty).Result;
            if (jsonResult != string.Empty)
            {
                dynamic data = JObject.Parse(jsonResult);
                result = converter.DeserializeAccountId(data);
            }
            return result;
        }
        private IAccount GetAccountSummary(string accountId)
        {
            IAccount result = null;
            string method = EndPoints.ACCOUNT_SUMMARY;
            method = method.Replace("{accountId}", accountId);

            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET,
                         method, false, string.Empty).Result;
            if (jsonResult != string.Empty)
            {
                dynamic data = JObject.Parse(jsonResult);
                result = converter.DeserializeObject<IAccount>(data);
            }
            return result;
        }
        public override IAccount GetAccountDetails(string acctId)
        {
            IAccount result = null;

            string method = EndPoints.ACCOUNT_INFO;
            method = method.Replace("{accountId}", acctId);

            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET,
                         method, false, string.Empty).Result;
            if (jsonResult != string.Empty)
            {
                dynamic data = JObject.Parse(jsonResult);
                result = converter.DeserializeObject<OandaAccount>(data);
            }

            return result;
        }
        public override void GetAccountChanges(IAccount acnt)
        {
            string method = EndPoints.ACCOUNT_CHANGES;
            method = method.Replace("{accountId}", acnt.Id);
            string param = "sinceTransactionID=" + ((OandaAccount)acnt).LastTransactionId;
 
            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET,
                         method, false, param).Result;
            if (jsonResult != string.Empty)
            {
                dynamic data = JObject.Parse(jsonResult);
                converter.UpdateAccountChanges(data, (OandaAccount)acnt);
            }
        }
        public override decimal GetAvailablePrinciple(string currency)
        {
            // GetCurrentPrice("AUD_USD");

            // GetAllTrades();
            //GetOpenPositions();
            //GetInstrumentTrade("AUD_USD");
            //Account a = GetAccountDetails(accountId);
            return 10;
        }
        //public override decimal GetAvailableMargin()
        //{
        //    // GetCurrentPrice("AUD_USD");

        //    // GetAllTrades();
        //    //GetOpenPositions();
        //    //GetInstrumentTrade("AUD_USD");
        //    Account a = GetAccountSummary();
        //    return a.MarginAvailable;
        //}
        #endregion

        #region "Price Services"
        public override List<TDMData> GetSymbolPriceList(string symbol, DateTime startDate)
        {
            List<TDMData> instrList = null;
            TDMData instr = null;
            string method = EndPoints.GET_CANDLES;
            method = method.Replace("{instrument}", symbol);
            string dtval = startDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffff'Z'");
            var argsParam = "count=5000&price=BA&granularity=M1&from=" + dtval;
            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET, method,
                false, argsParam).Result;
            if (jsonResult != string.Empty)
            {
                instrList = new List<TDMData>();
                JToken data = JObject.Parse(jsonResult);
                JArray actArray = data["candles"].Value<JArray>();
                foreach (JObject item in actArray)
                {
                    instr = new TDMData();
                    instr.Symbol = symbol;
                    instr.DTValue = item["time"].Value<DateTime>();
                    var cnd = item["ask"].Value<JObject>();
                    instr.BuyPrice = cnd["c"].Value<decimal>();
                    cnd = item["bid"].Value<JObject>();
                    instr.SellPrice = cnd["c"].Value<decimal>();
                    instrList.Add(instr);
                }
            }
            return instrList;
        }
        public override decimal GetCurrentPrice(string symbol, string mode)
        {
            decimal result = 0.0m;
            try
            {
                /* GetOpenPositions();
                 while (true)
                 {
                     decimal d;

                     d = GetCurrentSellPrice(symbol);
                     Console.Write("Sell:" + d);
                     d = GetCurrentBuyPrice(symbol);
                     Console.WriteLine(" ----- Buy:" + d);
                     Thread.Sleep(3000);
                 }*/

                if (mode == Constants.OrderAction.BUY)
                    result = GetCurrentBuyPrice(symbol);
                else if (mode == Constants.OrderAction.SELL)
                    result = GetCurrentSellPrice(symbol);
            }
            catch (Exception ex)
            {
                Common obj = new Common();
                obj.LogMessageToFile(DateTime.Now.ToString() + ex.GetType().ToString() + " here is the exception....");
                throw;
            }
            return result;
        }
        public override decimal GetLastTradePrice(string tradeId, string accountId)
        {
            return GetTradeInfo(tradeId, accountId);
        }
        private decimal GetCurrentBuyPrice(string symbol)
        {
            decimal result = 0.0m;
            string method = EndPoints.GET_CANDLES;
            method = method.Replace("{instrument}", symbol);
            var argsParam = "count=1&price=A&granularity=S5";
            //var argsParam = "price=M&granularity=M1&from=2005-10-17T15%3A00%3A00.000000000Z&to=2005-10-20T15%3A00%3A00.000000000Z";
            //var argsParam = "count=5000&price=M&granularity=M1&from=2005-10-17T15%3A00%3A00.000000000Z";
            ////https://api-fxtrade.oanda.com/v3/instruments/USD_CAD/candles?price=BA&from=2016-10-17T15%3A00%3A00.000000000Z&granularity=M1
            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET, method,
                false, argsParam).Result;
            if (jsonResult != string.Empty)
            {
                JToken data = JObject.Parse(jsonResult);
                JArray actArray = data["candles"].Value<JArray>();
                foreach (JObject item in actArray)
                {
                    var cnd = item["ask"].Value<JObject>();
                    result = cnd["c"].Value<decimal>();
                }

            }
            return result;
        }
        private decimal GetCurrentSellPrice(string symbol)
        {
            decimal result = 0.0m;
            string method = EndPoints.GET_CANDLES;
            method = method.Replace("{instrument}", symbol);
            var argsParam = "count=1&price=B&granularity=S5";
            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET, method,
                false, argsParam).Result;
            if (jsonResult != string.Empty)
            {
                JToken data = JObject.Parse(jsonResult);
                JArray actArray = data["candles"].Value<JArray>();
                foreach (JObject item in actArray)
                {
                    var cnd = item["bid"].Value<JObject>();
                    result = cnd["c"].Value<decimal>();
                }
            }
            return result;
        }
        #endregion

        #region "Order Services"
        public override Order GetOrderInfo(string orderId)
        {
            Order result = null;
            try
            {
                var argsParam = orderId;
                string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET, EndPoints.GET_ORDER,
                    false, argsParam).Result;
                if (jsonResult != string.Empty)
                {
                    dynamic data = JObject.Parse(jsonResult);
                    result = converter.DeserializeObject<Order>(data);
                }
            }
            catch (Exception ex)
            {
                Common obj = new Common();
                obj.LogMessageToFile(DateTime.Now.ToString() + ex.GetType().ToString() + " here is the exception....");
                throw;
            }
            return result;
        }
        public override Quote GetQuote(string symbol)
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<Quote> GetQuoteList(string accountId, string mode)
        {
            //This need to be changed to PRICE API
            //TODO *******************************************************


            List<Quote> result = null;

            string method = EndPoints.ACCOUNT_INSTRUMENTS;
            method = method.Replace("{accountId}", accountId);
            string param = "instruments=" + String.Join(",", this.GetTickerList());
            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET,
                         method, false, param).Result;
            if (jsonResult != string.Empty)
            {
                JObject jsonObject = JObject.Parse(jsonResult);
                JArray qArray = jsonObject["instruments"].Value<JArray>();
                result = new List<Quote>();
                foreach (JObject item in qArray)
                {
                    Quote q = converter.DeserializeObject<Quote>(item);
                    if (mode == Constants.OrderAction.BUY)
                        q.LastBuyTradePrice = GetCurrentBuyPrice(q.Symbol);
                    else if (mode == Constants.OrderAction.SELL)
                        q.LastSellTradePrice = GetCurrentSellPrice(q.Symbol);
                    result.Add(q);
                }
            }
            return result;
        }
        public override bool PlaceOrder(Order order, string accountId)
        {
            bool rtnVal = false;
            try
            {
                string method = EndPoints.POST_ORDER;
                method = method.Replace("{accountId}", accountId);

                string body = converter.SerializeObject<OrderInfo>(new OrderInfo(order));
                string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.POST,
                        method, false, string.Empty, body).Result;
                if (jsonResult != string.Empty)
                {
                    dynamic data = JObject.Parse(jsonResult);
                    rtnVal = converter.UpdateOrderDetails(data, order);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rtnVal;
        }
        #endregion

        #region "Position Services"
        public override IEnumerable<Position> GetOpenPositions(string accountId)
        {
            List<Position> result = null;
            Position pos = null;
            string method = EndPoints.OPEN_POSITIONS;
            method = method.Replace("{accountId}", accountId);

            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET,
                         method, false, string.Empty).Result;
            if (jsonResult != string.Empty)
            {
                JObject jsonObject = JObject.Parse(jsonResult);
                JArray posArray = jsonObject["positions"].Value<JArray>();

                result = new List<Position>();
                foreach (JObject item in posArray)
                {
                    pos = converter.DeserializeObject<ForexPosition>(item);
                    result.Add(pos);
                }
            }
            return result;
        }
        public override Position GetOpenPosition(string symbol, string accountId)
        {
            Position result = null;
            string method = EndPoints.OPEN_INSTRUMENT_POSITIONS;
            method = method.Replace("{accountId}", accountId);
            method = method + @"/" + symbol;

            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET,
                         method, false, string.Empty).Result;
            if (jsonResult != string.Empty)
            {
                JObject jsonObject = JObject.Parse(jsonResult);
                JObject posObject = jsonObject["position"].Value<JObject>();

                //result = new List<Position>();
                //foreach (JObject item in posArray)
                // {
                JObject longObj = posObject["long"].Value<JObject>();
                if (longObj["units"].Value<decimal>() > 0)
                    result = converter.DeserializeObject<ForexPosition>(posObject);
                //result.Add(pos);
                //}
            }
            return result;
        }
        #endregion

        #region "Trade Services"
        public override IEnumerable<TradeData> GetOpenTrades(string symbol, string accountId)
        {
            string method = string.Empty;
            List<ForexData> result = null;
            ForexData fxStock = null;

            if (symbol != string.Empty)
                method = EndPoints.LIST_OPEN_TRADES;
            else
                method = EndPoints.LIST_ALL_OPEN_TRADES;

            var argsParam = "instrument=" + symbol + "&state=OPEN";

            method = method.Replace("{accountId}", accountId);

            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET,
                         method, false, argsParam).Result;
            if (jsonResult != string.Empty)
            {

                JObject jsonObject = JObject.Parse(jsonResult);
                JArray trdArray = jsonObject["trades"].Value<JArray>();

                result = new List<ForexData>();
                foreach (JObject item in trdArray)
                {
                    fxStock = converter.DeserializeObject<ForexData>(item);
                    result.Add(fxStock);
                }
            }
            return result;
        }
        public override bool CloseTrade(TradeData fxS, string accountId)
        {
            bool rtnVal = false;
            string method = EndPoints.CLOSE_TRADE;
            method = method.Replace("{accountId}", accountId);
            method = method.Replace("{tradeId}", fxS.TradeId.ToString());
            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.PUT,
                        method, false, string.Empty).Result;
            if (jsonResult != string.Empty)
            {
                dynamic data = JObject.Parse(jsonResult);
                rtnVal = converter.UpdateFxStockDetails(data, (ForexData)fxS);
            }
            return rtnVal;
        }
        private decimal GetTradeInfo(string tradeId, string accountId)
        {
            decimal rtnVal = 0.0m;
            string method = EndPoints.LIST_TRADES;
            method = method.Replace("{accountId}", accountId);
            if (tradeId != string.Empty)
                method = method + @"/" + tradeId;

            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET,
                         method, false, string.Empty).Result;
            if (jsonResult != string.Empty)
            {
                JObject data = JObject.Parse(jsonResult);
                JObject trdObject = data["trade"].Value<JObject>();
                rtnVal = trdObject["price"].Value<decimal>();
            }
            return rtnVal;
        }
        private void GetInstrumentTrade(string symbol, string accountId)
        {
            string method = EndPoints.LIST_TRADES;
            method = method.Replace("{accountId}", accountId);
            var argsParam = "instrument=" + symbol;

            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET,
                         method, false, argsParam).Result;
            if (jsonResult != string.Empty)
            {
                dynamic data = JObject.Parse(jsonResult);
            }
        }
        #endregion

        #region "Transaction Services"
        public Transaction GetTransactionInfo(string transId, string accountId)
        {
            Transaction rtnVal = new Transaction();
            string method = EndPoints.LIST_TRANSACTIONS;
            method = method.Replace("{accountId}", accountId);

            if (transId != string.Empty)
                method = method + @"/" + transId;

            string jsonResult = ApiClient.CallAsync(this.GetExchangeName(), Methods.GET,
                         method, false, string.Empty).Result;
            if (jsonResult != string.Empty)
            {
                JObject data = JObject.Parse(jsonResult);
                //JObject trdObject = data["trade"].Value<JObject>();
                //rtnVal = trdObject["price"].Value<decimal>();
                rtnVal.requestID = jsonResult;
            }
            return rtnVal;
        }
        #endregion

        //public override void SetAccountId(string accId)
        //{
        //    this.accountId = accId;
        //}
    }
}
