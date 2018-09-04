using ND.Trading.Platform.Models;
using ND.Trading.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ND.Trading.Bot.Core
{
    public class Forexpolate_Test : Strategy
    {
        private string LastTransId { get; set; }
        private ForexStock FxStock { get; set; }
        private List<StockAnalyzer> stAnalyzerList { get; set; }
        StockAnalyzer thisAnalyzer = null;
        private PositionConfig posConfig = null;

        private Account LongAccountInfo { get; set; }
        private Account ShortAccountInfo { get; set; }
        private Account BackupLongAccountInfo { get; set; }
        private Account BackupShortAccountInfo { get; set; }
        private Account QuickBuyLongAccountInfo { get; set; }
        private Account QuickBuyShortAccountInfo { get; set; }

        private Account CurrentAccount { get; set; }

        private bool QuickBuyLongFlag { get; set; }
        private bool QuickBuyShortFlag { get; set; }

        public Forexpolate_Test(StrategyConfig config)
            : base(config)
        { 
            QuickBuyLongFlag = false;
            QuickBuyShortFlag = false;
            tdmObj = new TDM_Manager(ExchangeObject);
        }
        public override void TriggerStrategy()
        {
            try
            {
                LongAccountInfo = tdmObj.GetAccountDetails(GetAccountId("Long"));
                ShortAccountInfo = tdmObj.GetAccountDetails(GetAccountId("Short"));
                //this.GetPositionConfig("EUR_USD").TradeProcess = false;
                //GlobalConfig.SerializeRootConfig(ConfigFile);
                do
                {
                    this.CurrentAccount = LongAccountInfo;
                    RunStrategy();
                    this.CurrentAccount = ShortAccountInfo;
                    RunStrategy();
                    if (QuickBuyLongFlag)
                    {
                        this.CurrentAccount = QuickBuyLongAccountInfo;
                        RunQuickBuyStrategy();
                    }
                    if (QuickBuyShortFlag)
                    {
                        this.CurrentAccount = QuickBuyShortAccountInfo;
                        RunQuickBuyStrategy();
                    }

                } while (this.ExitLoop == 0);
            }
            catch (Exception ex)
            {
                Common obj = new Common();
                obj.LogMessageToFile(ex.Message);
                obj.LogMessageToFile("------------------------------------------------------------------------");
                obj.LogMessageToFile(ex.StackTrace);
            }
        }
        private void RunStrategy()
        {
            //UpdateAccountDetails();
            if (IsEnoughPositionToTrade())
            {
                for (var iPos = ((OandaAccount)this.CurrentAccount).PositionList.Count - 1; iPos >= 0; --iPos)
                {
                    this.StockObject = ((OandaAccount)this.CurrentAccount).PositionList[iPos];
                    posConfig = this.GetPositionConfig(this.StockObject.Symbol);

                    if (IsReadyToSell())
                    {
                        if (PlaceSellOrder()) // Do we need to call account update here?
                        {
                            Console.WriteLine(this.StockObject.Symbol + " Sold at " + this.StockObject.SellPrice.ToString());
                        }
                    }
                    else
                    {
                        if (posConfig.TradeProcess)
                            ProcessTrade(); //Account update has an impact on position list becaus of closed positions
                        if (IsReadyToAddMore())
                            AddMoreStocks(); //Account update has an impact on position list becaus of closed positions
                    }
                }
            }
            else
                AddEnoughPositions();
        }
        private void RunQuickBuyStrategy()
        {
            UpdateAccountDetails();
        }

        private void UpdateAccountDetails()
        {
            DataAnalyzer dt = null;
            tdmObj.GetAccountChanges(this.CurrentAccount); //Service call 2 - Every Do-While loop delay thread sleep
            this.CurrentAccount.Type = GetAccountType(this.CurrentAccount.Id);
            if (((OandaAccount)this.CurrentAccount).OpenedTradesJson != null)
            {
                dt = new DataAnalyzer(string.Empty, this.CurrentAccount.Id, this.CurrentAccount.Type);
                dt.NewTrades = ((OandaAccount)this.CurrentAccount).OpenedTradesJson;
                Thread newThread = new Thread(dt.SaveNewTrades);
                newThread.Name = "AnalyzerExpSave";
                newThread.Start();
            }
        }
        private Account GetAccountDetails(string accountId)
        {
            Account accountInfo = null;
            accountInfo = tdmObj.GetAccountDetails(accountId); //Service call 1 - Once while starting app

            for (var iPos = ((OandaAccount)accountInfo).PositionList.Count - 1; iPos >= 0; --iPos)
            {
                DataAnalyzer dt = new DataAnalyzer(((OandaAccount)accountInfo).PositionList[iPos].Symbol, accountId, GetAccountType(accountId));
                Thread newThread = new Thread(dt.SaveActiveTicker);
                newThread.Name = "AnalyzerAdd";
                newThread.Start();

                Console.WriteLine("Bought " + ((OandaAccount)accountInfo).PositionList[iPos].TotalCount + " " +
                    ((OandaAccount)accountInfo).PositionList[iPos].Symbol);
            }

            return accountInfo;
        }

        protected bool IsEnoughPositionToTrade()
        {
            bool rtnVal = false;
            int expectedPositionLevel = (this.MaxPositionCount * this.MinPositionLevelPercent) / 100;
            if (((OandaAccount)this.CurrentAccount).PositionList.Count >= expectedPositionLevel)
                rtnVal = true;
            return rtnVal;
        }
        private void AddEnoughPositions()
        {
            Order orderObject = null;
            PositionConfig pConfig = null;
            try
            {
                IEnumerable<Quote> quoteList = tdmObj.GetQuoteList(tdmObj.GetTickerList(), this.CurrentAccount.Id);   //Service call 3  - Initially once and whenever new position added to account based on Max pos count
                foreach (Quote q in quoteList)
                {
                    if (!IsSymbolAlreadyAvailable(q.Symbol))
                    {
                        pConfig = this.GetPositionConfig(q.Symbol);
                        if (pConfig != null)
                        {
                            if (pConfig.InitialUnits > pConfig.MinQuantity)
                            {
                                if (this.CurrentAccount.Type == "Long")
                                    orderObject = new Order(q.Symbol, pConfig.InitialUnits, q.LastTradePrice, Constants.OrderAction.BUY, this.OrderType);
                                else if (this.CurrentAccount.Type == "Short")
                                    orderObject = new Order(q.Symbol, pConfig.InitialUnits, q.LastTradePrice, Constants.OrderAction.SELL, this.OrderType);

                                if (tdmObj.PlaceOrder(orderObject, this.CurrentAccount.Id))   //Service call 4
                                {
                                    if (orderObject.Status == Constants.OrderState.FILLED)
                                    {
                                        DataAnalyzer dt = new DataAnalyzer(orderObject.Symbol, this.CurrentAccount.Id, this.CurrentAccount.Type);
                                        Thread newThread = new Thread(dt.SaveActiveTicker);
                                        newThread.Name = "AnalyzerAdd";
                                        newThread.Start();
                                    }
                                    else if (orderObject.Status == Constants.OrderState.PENDING)
                                    {
                                        Console.WriteLine(DateTime.Now + " : Order status " + Constants.OrderState.PENDING);
                                    }
                                    else if (orderObject.Status == Constants.OrderState.CANCELLED)
                                    {
                                        Console.WriteLine(DateTime.Now + " : Order status " + Constants.OrderState.CANCELLED);
                                    }
                                    if (((OandaAccount)this.CurrentAccount).PositionList.Count >= this.MaxPositionCount)
                                        return;
                                }
                            }
                        }
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        protected bool IsSymbolAlreadyAvailable(string symbol)
        {
            bool rtnVal = false;
            ForexPosition fxPos = ((OandaAccount)this.CurrentAccount).PositionList.Find(x => x.Symbol == symbol);
            if (fxPos != null)
                rtnVal = true;

            return rtnVal;
        }
        private bool IsReadyToSell()
        {
            bool rtnVal = false;
            if (posConfig.SellMode == Constants.TradeMode.FIXED)
            {
                if (((ForexPosition)this.StockObject).UnrealizedPL >= this.posConfig.FixedProfitValue)
                    rtnVal = true;
            }
            else if (posConfig.SellMode == Constants.TradeMode.VARIABLE)
            {

            }
            return rtnVal;
        }
        private bool PlaceSellOrder()
        {
            bool rtnVal = false;
            Order orderObject = null;
            //Need short buy change
            /*if (this.OrderType == Constants.OrderType.MARKET)
                orderObject = new Order(this.StockObject.Symbol, this.StockObject.TotalCount, this.CurrentPrice, Constants.OrderAction.SELL, this.OrderType);
            else if (this.StockObject.StrategyData.OrderType == Constants.OrderType.LIMIT)
                orderObject = new Order(this.StockObject.Symbol, this.StockObject.TotalCount, this.CurrentPrice, Constants.OrderAction.SELL, this.OrderType);*/

            if (this.CurrentAccount.Type == "Long")
                orderObject = new Order(this.StockObject.Symbol, this.StockObject.TotalCount, this.CurrentPrice, Constants.OrderAction.SELL, this.OrderType);
            else if (this.CurrentAccount.Type == "Short")
                orderObject = new Order(this.StockObject.Symbol, (this.StockObject.TotalCount * -1), this.CurrentPrice, Constants.OrderAction.BUY, this.OrderType);

            if (tdmObj.PlaceOrder(orderObject, this.CurrentAccount.Id))   //Service call 5
            {
                if (orderObject.Status == Constants.OrderState.FILLED)
                {
                    //Do this in new thread..
                    this.StockObject.TotalCount = orderObject.Count;
                    this.StockObject.SellPrice = orderObject.Price;
                    this.StockObject.TotalRevenue = (orderObject.Count * orderObject.Price) + orderObject.Brokerage;
                    this.StockObject.TotalSellFee = orderObject.Brokerage;
                    this.StockObject.ProfitnLoss = orderObject.PnL;
                    this.StockObject.ModifiedOn = orderObject.DoneAt;
                    this.StockObject.UserName = GetUserConfig(this.StockObject.Symbol).Name;
                    this.StockObject.Status = Constants.PositionState.CLOSED;
                    if (UpdatePositionsForDataAnalysis())
                    {
                        DataAnalyzer dt = new DataAnalyzer(orderObject.Symbol, this.CurrentAccount.Id, this.CurrentAccount.Type);
                        Thread newThread = new Thread(dt.CloseActiveTicker);
                        newThread.Name = "AnalyzerAdd";
                        newThread.Start();
                    }
                    else
                        Console.WriteLine("Error while writing to DB");
                    rtnVal = true;
                }
                else if (orderObject.Status == Constants.OrderState.PENDING)
                {
                    Console.WriteLine(DateTime.Now + " : Order status " + Constants.OrderState.PENDING);
                }
                else if (orderObject.Status == Constants.OrderState.CANCELLED)
                {
                    Console.WriteLine(DateTime.Now + " : Order status " + Constants.OrderState.CANCELLED);
                }

            }
            return rtnVal;
        }
        private bool IsReadyToAddMore()
        {
            bool rtnVal = false;
            decimal priceDiff = 0.0m;
            decimal lastPrice = 0.0m;
            string type = this.CurrentAccount.Type;
            if (posConfig.SellMode == Constants.TradeMode.FIXED)
            {
                //Need short buy change
                if (type == "Long")
                    this.CurrentPrice = tdmObj.GetCurrentPrice(this.StockObject.Symbol, Constants.OrderAction.BUY);   //Service call 6 - every for loop
                else if (type == "Short")
                    this.CurrentPrice = tdmObj.GetCurrentPrice(this.StockObject.Symbol, Constants.OrderAction.SELL);   //Service call 6 - every for loop

                lastPrice = this.StockObject.TradeList[this.StockObject.TradeList.Count - 1].PriceBuy;
                //thisAnalyzer = GetStockAnalyzer(this.StockObject.Symbol); //Provision for quick buy when process trade false
                if (type == "Long")
                {
                    //if ((CurrentPrice-thisAnalyzer.QuickBuyAnalzer.LastLongPrice) > 0.00100m) //make it as quickbuythreshold in config
                    //{
                    //    //Provision for quick buy when process trade false
                    //}
                    priceDiff = lastPrice - CurrentPrice;
                }
                else if (type == "Short")
                {
                    //if ((thisAnalyzer.QuickBuyAnalzer.LastShortPrice - CurrentPrice) > 0.00100m) //make it as quickbuythreshold in config
                    //{
                    //    //Provision for quick buy process trade false
                    //}
                    priceDiff = CurrentPrice - lastPrice;
                }

                if (priceDiff > posConfig.FixedBuyPriceValue)
                    rtnVal = true;
            }
            else if (posConfig.SellMode == Constants.TradeMode.VARIABLE)
            {

            }
            return rtnVal;
        }
        private void AddMoreStocks()
        {
            int count = 0;
            Order orderObject = null;
            if (((ForexPosition)this.StockObject).TradeList.Count < this.posConfig.ExtrapolateLevel)
            {
                count = Math.Abs(Convert.ToInt32(this.StockObject.TradeList[this.StockObject.TradeList.Count - 1].Count)) + 1;
                //***************************************************************************************
                //Check enough margin available to buy this units
                decimal margin = ((OandaAccount)this.CurrentAccount).MarginAvailable;
                decimal expPrice = count * this.CurrentPrice;

                if (expPrice < margin)
                {
                    //Need short buy change
                    /*   if (this.OrderType == Constants.OrderType.MARKET)
                           orderObject = new Order(this.StockObject.Symbol, count, this.CurrentPrice, Constants.OrderAction.BUY, this.OrderType);
                       else if (this.StockObject.StrategyData.OrderType == Constants.OrderType.LIMIT)
                           orderObject = new Order(this.StockObject.Symbol, count, this.CurrentPrice, Constants.OrderAction.BUY, this.OrderType);*/

                    if (this.CurrentAccount.Type == "Long")
                        orderObject = new Order(this.StockObject.Symbol, count, this.CurrentPrice, Constants.OrderAction.BUY, this.OrderType);
                    else if (this.CurrentAccount.Type == "Short")
                        orderObject = new Order(this.StockObject.Symbol, count, this.CurrentPrice, Constants.OrderAction.SELL, this.OrderType);

                    if (tdmObj.PlaceOrder(orderObject, CurrentAccount.Id))   //Service call 7
                    {
                        if (orderObject.Status == "FILLED")
                        {
                            Console.WriteLine("Added More " + this.CurrentAccount.Type + " " + this.StockObject.Symbol + " at " + this.CurrentPrice.ToString());
                        }
                        else if (orderObject.Status == "PENDING")
                        {
                            Console.WriteLine(DateTime.Now + " : Order status " + Constants.OrderState.PENDING);
                        }
                        else if (orderObject.Status == "CANCELLED")
                        {
                            Console.WriteLine(DateTime.Now + " : Order status " + Constants.OrderState.CANCELLED);
                        }
                    }
                    else
                        Console.WriteLine("Buy more stocks failed - Please check");
                }
            }
        }
        private void ProcessTrade()
        {
            decimal profitVal = 0.0m;
            if (this.StockObject.TradeList.Count > 1)
            {
                for (var iPos = this.StockObject.TradeList.Count - 1; iPos > 0; iPos--)
                {
                    this.FxStock = (ForexStock)this.StockObject.TradeList[iPos];

                    if (posConfig.TradeProcessProfitMode == Constants.TradeMode.FIXED)
                        profitVal = posConfig.FixedTradeProcessProfitValue;
                    else if (posConfig.TradeProcessProfitMode == Constants.TradeMode.VARIABLE)
                        profitVal = posConfig.FixedTradeProcessProfitValue;

                    if (this.FxStock.UnrealizedPL >= (profitVal * 6)) //Move this to configuration
                    {
                        Thread newThread = null;
                        //Quick buy 10K number and try closing soon, if soon not closed, enable flag and monitor daily.
                        if (this.CurrentAccount.Type == "Long")
                        {
                            if (QuickBuyLongAccountInfo == null)
                                QuickBuyLongAccountInfo = GetAccountDetails(GetAccountId("Long"));
                            newThread = new Thread(() => this.TriggerQuickBuy(this.FxStock.Symbol));
                        }
                        else if (this.CurrentAccount.Type == "Short")
                        {
                            if (QuickBuyShortAccountInfo == null)
                                QuickBuyShortAccountInfo = GetAccountDetails(GetAccountId("Short"));
                            newThread = new Thread(() => this.TriggerQuickBuy(this.FxStock.Symbol));
                        }

                        newThread.Name = "QuickBuyTragger";
                        newThread.Start();
                    }
                    else if (this.FxStock.UnrealizedPL >= profitVal)
                    {
                        if (tdmObj.CloseTrade(this.FxStock, this.CurrentAccount.Id))
                        {
                            this.FxStock.Cost = (this.FxStock.Count * this.FxStock.PriceBuy);
                            this.FxStock.Revenue = (this.FxStock.Count * this.FxStock.PriceSell);
                            this.FxStock.UserName = GetUserConfig(this.FxStock.Symbol).Name;
                            if (!dbManager.InsertEntity<ForexStock>(this.FxStock))
                                Console.WriteLine("Error while writing to DB");
                            this.StockObject.TradeList.RemoveAt(iPos);
                        }
                    }
                    else
                        break;

                }
            }
        }
        private void ProcessOffsetTrade()
        {
            this.StockObject.TradeList = tdmObj.GetOpenTrades(this.StockObject.Symbol, CurrentAccount.Id).ToList();
            thisAnalyzer = GetStockAnalyzer(this.StockObject.Symbol);
            if (this.StockObject.TradeList.Count > 1)
            {
                if (thisAnalyzer.OffSetAnalyzer.IsSellOffsetMet)
                {
                    int previousIndex = thisAnalyzer.OffSetAnalyzer.LastOffsetMetIndex;
                    int currentIndex = GetOffsetIndexStatus(thisAnalyzer.OffSetAnalyzer.LastOffsetMetIndex);
                    if (currentIndex < previousIndex)
                    {
                        this.FxStock = (ForexStock)this.StockObject.TradeList[previousIndex];
                        if (this.FxStock.UnrealizedPL < 0.15m)
                            CloseTrades(thisAnalyzer.OffSetAnalyzer.LastOffsetMetIndex);
                    }
                }
                else
                    GetOffsetIndexStatus(0);

                this.FxStock = (ForexStock)this.StockObject.TradeList[0];
                if (thisAnalyzer.OffSetAnalyzer.LastProfitValue > 0.10m)
                {
                    if (this.FxStock.UnrealizedPL > 0.10m)
                    {
                        decimal diffIndex = thisAnalyzer.OffSetAnalyzer.LastProfitValue - this.FxStock.UnrealizedPL;
                        if (diffIndex > 0.03m)
                        {
                            if (thisAnalyzer.OffSetAnalyzer.OffsetMetDiffIndex > 2
                                || thisAnalyzer.OffSetAnalyzer.LastTradeTimeDiff.Minutes > 30
                                || thisAnalyzer.OffSetAnalyzer.LastTradeTimeDiff.Hours >= 1)
                                CloseTrade(0);
                            else
                                thisAnalyzer.OffSetAnalyzer.OffsetMetDiffIndex++;
                        }
                    }
                }
                thisAnalyzer.OffSetAnalyzer.LastProfitValue = this.FxStock.UnrealizedPL;
            }
        }
        private int GetOffsetIndexStatus(int index)
        {
            this.FxStock = (ForexStock)this.StockObject.TradeList[index];
            if (this.FxStock.UnrealizedPL >= 0.20m) //first or still it holds the profit margin
            {
                thisAnalyzer.OffSetAnalyzer.IsSellOffsetMet = true;
                thisAnalyzer.OffSetAnalyzer.LastOffsetMetIndex = index;
                for (var iPos = index + 1; iPos < this.StockObject.TradeList.Count - 1; iPos++) //check this is increased
                {
                    this.FxStock = (ForexStock)this.StockObject.TradeList[iPos];
                    if (this.FxStock.UnrealizedPL >= 0.20m)
                    {
                        thisAnalyzer.OffSetAnalyzer.IsSellOffsetMet = true;
                        thisAnalyzer.OffSetAnalyzer.LastOffsetMetIndex = iPos;
                        thisAnalyzer.OffSetAnalyzer.OffsetMetDiffIndex = 0;
                    }
                    else
                        break;
                }
            }
            else
            {
                for (var iPos = index - 1; iPos >= 0; iPos--)
                {
                    this.FxStock = (ForexStock)this.StockObject.TradeList[iPos];
                    if (this.FxStock.UnrealizedPL >= 0.20m)
                    {
                        thisAnalyzer.OffSetAnalyzer.IsSellOffsetMet = true;
                        thisAnalyzer.OffSetAnalyzer.LastOffsetMetIndex = iPos;
                        break;
                    }
                }
            }
            return thisAnalyzer.OffSetAnalyzer.LastOffsetMetIndex;
        }
        private void CloseTrades(int index)
        {
            Console.WriteLine("Closing trades.........");
            for (var iPos = index; iPos >= 0; iPos--)
            {
                this.FxStock = (ForexStock)this.StockObject.TradeList[iPos];
                if (this.FxStock.UnrealizedPL > 0.10m)
                {
                    if (tdmObj.CloseTrade(this.FxStock, this.CurrentAccount.Id))
                    {
                        this.FxStock.Cost = (this.FxStock.Count * this.FxStock.PriceBuy);
                        this.FxStock.Revenue = (this.FxStock.Count * this.FxStock.PriceSell);
                        this.FxStock.UserName = GetUserConfig(this.FxStock.Symbol).Name;
                        //this.FxStock.ProfitnLoss = this.FxStock.ProfitnLoss + FxStock.FeeSell;
                        Console.WriteLine("Sold " + this.FxStock.Count + " " + this.FxStock.Symbol + " with price " + this.FxStock.PriceSell + " and Profit = " + this.FxStock.ProfitnLoss + " at " + DateTime.Now);
                        if (!dbManager.InsertEntity<ForexStock>(this.FxStock))
                            Console.WriteLine("Error while writing to DB");
                        this.StockObject.TradeList.RemoveAt(iPos);
                        thisAnalyzer.ClearAnalyzer();
                    }
                }
            }
            Console.WriteLine("....................................");
        }
        private void CloseTrade(int iPos)
        {
            this.FxStock = (ForexStock)this.StockObject.TradeList[iPos];
            if (this.FxStock.UnrealizedPL > 0.10m)
            {
                if (tdmObj.CloseTrade(this.FxStock, this.CurrentAccount.Id))
                {
                    this.FxStock.Cost = (this.FxStock.Count * this.FxStock.PriceBuy);
                    this.FxStock.Revenue = (this.FxStock.Count * this.FxStock.PriceSell);
                    this.FxStock.UserName = GetUserConfig(this.FxStock.Symbol).Name;
                    //this.FxStock.ProfitnLoss = this.FxStock.ProfitnLoss;
                    Console.WriteLine("Sold " + this.FxStock.Count + " " + this.FxStock.Symbol + " with price " + this.FxStock.PriceSell + " and Profit = " + this.FxStock.ProfitnLoss + " at " + DateTime.Now);
                    if (!dbManager.InsertEntity<ForexStock>(this.FxStock))
                        Console.WriteLine("Error while writing to DB");
                    this.StockObject.TradeList.RemoveAt(iPos);
                    thisAnalyzer.ClearAnalyzer();
                }
            }
        }
        private StockAnalyzer GetStockAnalyzer(string symbol)
        {
            StockAnalyzer stAny = stAnalyzerList.Find(item => item.Symbol == symbol);
            if (stAny == null)
            {
                stAny = new StockAnalyzer(symbol);
                stAnalyzerList.Add(stAny);
            }
            return stAny;
        }
        private void TriggerQuickBuy(string symbol)
        {
            DateTime triggerTime = DateTime.Now;
            Order orderObject = null;

            if (this.CurrentAccount.Type == "Long")
                orderObject = new Order(this.StockObject.Symbol, 10000, this.CurrentPrice, Constants.OrderAction.BUY, this.OrderType);
            else if (this.CurrentAccount.Type == "Short")
                orderObject = new Order(this.StockObject.Symbol, 10000, this.CurrentPrice, Constants.OrderAction.SELL, this.OrderType);

            if (tdmObj.PlaceOrder(orderObject, this.CurrentAccount.Id))   //Service call 7
            {
                if (orderObject.Status == "FILLED")
                {
                    Console.WriteLine("Quick Buy " + this.CurrentAccount.Type + " -------- " + this.StockObject.Symbol + " at " + this.CurrentPrice.ToString());
                }
                else if (orderObject.Status == "PENDING")
                {
                    Console.WriteLine(DateTime.Now + " : Order status " + Constants.OrderState.PENDING);
                }
                else if (orderObject.Status == "CANCELLED")
                {
                    Console.WriteLine(DateTime.Now + " : Order status " + Constants.OrderState.CANCELLED);
                }
            }
            else
                Console.WriteLine("Buy more stocks failed - Please check");
        }
    }
}
