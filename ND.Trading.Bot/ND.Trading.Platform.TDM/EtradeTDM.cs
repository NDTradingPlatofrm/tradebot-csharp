using ND.Trading.Platform.Models;
using System;
using System.Collections.Generic;


namespace ND.Trading.Platform.TDM
{
    public class EtradeTDM : TDManager
    {
        public EtradeTDM(ExchangeConfig config)
           : base(config)
        {
            //Dictionary<string, string> tokenParams = new Dictionary<string, string>()
            //{
            //    { "request_token_url", EndPoints.REQUEST_TOKEN},
            //    { "token_authorize_url", EndPoints.AUTHORIZE_TOKEN},
            //    { "access_token_url", EndPoints.ACCESS_TOKEN},
            //    { "renew_access_token_url", EndPoints.RENEW_ACCESS_TOKEN},
            //    { "signature_method", "HMAC-SHA1"},
            //    { "callback", "oob"},
            //    { "version", "1.0"}
            //};

            //ApiClient = new OauthClient(this.AuthVal, tokenParams);
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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

        //public override Transaction GetTransactionInfo(string transId, string accountId)
        //{
        //    throw new NotImplementedException();
        //}

        public override bool PlaceOrder(Order order, string accountId)
        {
            throw new NotImplementedException();
        }
    }
}
