using ND.Trading.Bot.Core;
using ND.Trading.Platform.Models;
using ND.Trading.Utilities;
using System;
using System.Collections.Generic;


namespace ND.Trading.Platform.TDM
{
    public abstract class TDManager : IExchange
    {
        private string Name { get; set; }
        private string MarketName { get; set; }
        private string[] TickerList { get; set; }
        private string[] Currencies { get; set; }
        protected AuthInfo AuthVal { get; set; }
        protected IApiClient ApiClient { get; set; }
        protected int ApiDelay { get; set; }

        protected TDManager(ExchangeConfig config)
        {
            Name = config.Name;
            MarketName = config.Market;
            ApiDelay = config.ApiDelay;
           // UniqueStrategyId = config.UniqueStrategyId;
            AuthVal = new AuthInfo(config.BaseUrl,
                config.Key,
                config.Secret,
                config.Passphrase,
                config.Token);
            if (config.Tickers != null)
                TickerList = config.Tickers.Split(',');
            if (config.Currencies != null)
                Currencies = config.Currencies.Split(',');
        }
        public string GetExchangeName()
        {
            return Name;
        }

        public string GetMarketName()
        {
            return MarketName;
        }

        public string[] GetTickerList()
        {
            return TickerList;
        }

        public string[] GetCurrencyList()
        {
            return Currencies;
        }

        public abstract decimal GetCurrentPrice(string symbol, string mode);
        public abstract Quote GetQuote(string symbol);
        public abstract IEnumerable<Quote> GetQuoteList(string accountId, string mode);
        public abstract bool PlaceOrder(Order order, string accountId);
        public abstract Order GetOrderInfo(string orderId);
        public abstract decimal GetAvailablePrinciple(string currency);
        public abstract IAccount GetAccountDetails(string accountId);
        public abstract void GetAccountChanges(IAccount acnt);
        public abstract IEnumerable<Position> GetOpenPositions(string accountId);
        public abstract Position GetOpenPosition(string symbol, string accountId);
        public abstract IEnumerable<TradeData> GetOpenTrades(string symbol, string accountId);
        public abstract decimal GetLastTradePrice(string tradeId, string accountId);
       // public abstract Transaction GetTransactionInfo(string transId, string accountId);
        //public abstract decimal GetAvailableMargin();
        public abstract bool CloseTrade(TradeData trdData, string accountId);
        //public abstract void SetAccountId(string accId);
        public abstract List<TDMData> GetSymbolPriceList(string symbol, DateTime startDate);
    }
}
