using ND.Trading.Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Bot.Core
{
    public interface IExchange
    {
        string GetExchangeName();
        string GetMarketName();
        string[] GetTickerList();
        decimal GetCurrentPrice(string symbol, string mode);
        decimal GetAvailablePrinciple(string currency);
        Quote GetQuote(string symbol);
        IEnumerable<Quote> GetQuoteList(string accountId, string mode);
        bool PlaceOrder(Order order, string accountId);
        Order GetOrderInfo(string orderId);
        IAccount GetAccountDetails(string accountId);
        void GetAccountChanges(IAccount acnt);
        IEnumerable<Position> GetOpenPositions(string accountId);
        Position GetOpenPosition(string symbol, string accountId);
        IEnumerable<TradeData> GetOpenTrades(string symbol, string accountId);
        decimal GetLastTradePrice(string tradeId, string accountId);
        bool CloseTrade(TradeData trdData, string accountId);
        List<TDMData> GetSymbolPriceList(string symbol, DateTime startDate);

    }
}
