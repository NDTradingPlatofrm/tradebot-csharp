using ND.Trading.Bot.Core;
using ND.Trading.Platform.Models;
using ND.Trading.Utilities;
using System;
using System.Linq;
using System.Threading;

namespace ND.Trading.Bot.Strategies
{
    public class Stockpolate : Strategy
    {
        private IAccount LongAccountInfo { get; set; }
        private IAccount ShortAccountInfo { get; set; }
        private IAccount BackupLongAccountInfo { get; set; }
        private IAccount BackupShortAccountInfo { get; set; }
        private IAccount QuickBuyLongAccountInfo { get; set; }
        private IAccount QuickBuyShortAccountInfo { get; set; }

        private IAccount CurrentAccount { get; set; }

        public Stockpolate(StrategyConfig config)
            : base(config)
        {
           
        }

        public override void TriggerStrategy()
        {
            try
            {
                LongAccountInfo = GetAccountDetails(GetAccountId("Long"));
                ShortAccountInfo = GetAccountDetails(GetAccountId("Short"));

                do
                {
                    //this.CurrentAccount = LongAccountInfo;
                    //RunStrategy();
                    //this.CurrentAccount = ShortAccountInfo;
                    //RunStrategy();
                    //if (QuickBuyLongFlag)
                    //{
                    //    this.CurrentAccount = QuickBuyLongAccountInfo;
                    //    RunQuickBuyStrategy();
                    //}
                    //if (QuickBuyShortFlag)
                    //{
                    //    this.CurrentAccount = QuickBuyShortAccountInfo;
                    //    RunQuickBuyStrategy();
                    //}

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
        private IAccount GetAccountDetails(string accountId)
        {
            IAccount accountInfo = null;
            accountInfo = ExchangeObject.GetAccountDetails(accountId); //Service call 1 - Once while starting app
            accountInfo.PositionList = ExchangeObject.GetOpenPositions(accountId).ToList();

            for (var iPos = accountInfo.PositionList.Count - 1; iPos >= 0; --iPos)
            {
                DataAnalyzer dt = new DataAnalyzer(accountInfo.PositionList[iPos].Symbol, accountId, GetAccountType(accountId));
                Thread newThread = new Thread(dt.SaveActiveTicker);
                newThread.Name = "AnalyzerAdd";
                newThread.Start();

                Console.WriteLine("Bought " + accountInfo.PositionList[iPos].TotalCount + " " +
                    accountInfo.PositionList[iPos].Symbol);
            }

            return accountInfo;
        }
    }
}
