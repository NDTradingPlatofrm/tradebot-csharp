using ND.Trading.Platform.Models;
using ND.Trading.Platform.Models.Oanda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Bot.Core
{
    public class TDMExchange : Exchange
    {
        private Dictionary<string, List<TDM_Instrument>> InstrumentList { get; set; }
        private List<Account> AccountInfoList { get; set; }

        public TDMExchange(ExchangeConfig config)
            : base(config)
        {
            InstrumentList = new Dictionary<string, List<TDM_Instrument>>();
        }
        private Account GetAccount(string id)
        {
            Account rtnVal = null;
            foreach (Account act in AccountInfoList)
            {
                if (act.Id == id)
                    rtnVal = act;
            }
            return rtnVal;
        }
        private string GetLastOrderId(string id)
        {
            string orderId = "0";
            OandaAccount acct = ((OandaAccount)GetAccount(id));
            if (acct.OrderList.Count > 0)
                orderId = acct.OrderList[0].Id;
            return orderId;
        }
        private string GetLastTradeId(string id)
        {
            OandaAccount acct = ((OandaAccount)GetAccount(id));
            return acct.LastTransactionId;
        }
        private string[] GetTickerList()
        {
            return ExchangeObject.GetTickerList();
        }
        private List<TDM_Instrument> GetInstrumentList(string symbol)
        {
            List<TDM_Instrument> rtnVal = null;
            foreach (KeyValuePair<string, List<TDM_Instrument>> insLst in InstrumentList)
            {
                if (insLst.Key == symbol)
                    rtnVal = insLst.Value;
            }
            return rtnVal;
        }
        public override Account GetAccountDetails(string accountId)
        {
            Account accountInfo = null;
            accountInfo = GetAccount(accountId);
            if (accountInfo == null)
            {
                accountInfo = new Account();
                AccountInfoList.Add(accountInfo);
                accountInfo.Id = accountId;
            }
            return accountInfo;
        }
        public override void GetAccountChanges(Account act)
        {
            //Nothing to do with this function if this is TDM test
        }
        public override IEnumerable<Quote> GetQuoteList(string[] tickerList, string accountId)
        {
            List<TDM_Instrument> instrList = null;
            List<Quote> result = new List<Quote>();
            Quote q = null;
            if (tickerList == null)
                tickerList = this.GetTickerList();
            for (int i = 0; i < tickerList.Length; i++)
            {
                instrList = ExchangeObject.GetSymbolPriceList(tickerList[i], DateTime.Now);
                InstrumentList.Add(tickerList[i], instrList);
                q = new Quote();
                q.Symbol = tickerList[i];
                q.LastTradePrice = instrList[0].BuyPrice;
                result.Add(q);
            }
            return result;
        }
        public override bool PlaceOrder(Order order, string accountId)
        {
            bool rtnVal = false;
            OandaAccount acct = null;
            try
            {
                order.OrderId = (Convert.ToInt32(GetLastOrderId(accountId)) + 1).ToString();
                acct = ((OandaAccount)GetAccount(accountId));
                if (order.OrderType == Constants.OrderAction.BUY)
                {
                    if (acct.PositionList.Count == 0)
                        acct.PositionList.Add(new ForexPosition(order));
                    else
                    {
                        foreach (Position pos in acct.PositionList)
                        {
                            if (pos.Symbol == order.Symbol)
                                pos.AddMoreStocks(order);
                        }
                    }

                    order.Status = "FILLED";
                    order.Count = order.Count;
                    order.Price = order.Price;
                    order.TradeId = (Convert.ToInt32(GetLastTradeId(accountId)) + 1).ToString();
                    order.DoneAt = DateTime.Now;
                    // order.Status = "CANCELLED";
                    //order.PnL = ;
                    //order.Brokerage = ;
                }
                if (order.OrderType == Constants.OrderAction.SELL)
                {

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rtnVal;
        }
        public override decimal GetCurrentPrice(string symbol, string mode)
        {
            decimal result = 0.0m;
            try
            {
                if (mode == Constants.OrderAction.BUY)
                    result = GetCurrentBuyPrice(symbol);
                else if (mode == Constants.OrderAction.SELL)
                    result = GetCurrentSellPrice(symbol);
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }
        private decimal GetCurrentBuyPrice(string symbol)
        {
            decimal result = 0.0m;
           
            return result;
        }
        private decimal GetCurrentSellPrice(string symbol)
        {
            decimal result = 0.0m;
           
            return result;
        }

        public override Quote GetQuote(string symbol)
        {
            throw new NotImplementedException();
        }
        public override Order GetOrderInfo(string orderId)
        {
            throw new NotImplementedException();
        }
        public override decimal GetAvailablePrinciple(string currency)
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<Position> GetOpenPositions(string accountId)
        {
            throw new NotImplementedException();
        }
        public override Position GetOpenPosition(string symbol, string accountId)
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<StockData> GetOpenTrades(string symbol, string accountId)
        {
            throw new NotImplementedException();
        }
        public override decimal GetLastTradePrice(string tradeId, string accountId)
        {
            throw new NotImplementedException();
        }
        public override Transaction GetTransactionInfo(string transId, string accountId)
        {
            throw new NotImplementedException();
        }
        public override bool CloseTrade(ForexStock fxS, string accountId)
        {
            throw new NotImplementedException();
        }
        public override List<TDM_Instrument> GetSymbolPriceList(string symbol, DateTime startDate)
        {
            throw new NotImplementedException();
        }
    }
}