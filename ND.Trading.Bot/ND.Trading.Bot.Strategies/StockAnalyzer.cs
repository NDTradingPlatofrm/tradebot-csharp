using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Bot.Core
{
    public class StockAnalyzer
    {
        public string Symbol { get; set; }
        public OffsetAnalyzer OffSetAnalyzer { get; set; }
        public QuickBuyAnalyzer QuickBuyAnalzer { get; set; }

        public StockAnalyzer()
        { }
        public StockAnalyzer(string symbol)
        {
            Symbol = symbol;
            OffSetAnalyzer = new OffsetAnalyzer();
            QuickBuyAnalzer = new QuickBuyAnalyzer();
        }
        public void ClearAnalyzer()
        {
            OffSetAnalyzer.ClearAnalyzer();
        }
    }
    public class QuickBuyAnalyzer
    {
        public decimal LastShortPrice { get; set; }
        public decimal LastLongPrice { get; set; }
        public QuickBuyAnalyzer()
        {
            LastShortPrice = 0.0m;
            LastLongPrice = 0.0m;
        }
    }
    public class OffsetAnalyzer
    {
        public bool IsBuyOffsetMet { get; set; }
        public bool IsSellOffsetMet { get; set; }
        public int LastOffsetMetIndex { get; set; }
        public decimal LastProfitValue { get; set; }
        public int OffsetMetDiffIndex { get; set; }
        public DateTime LastTradeCloseTime { get; set; }

        private TimeSpan lastTradeTimeDiff;

        public TimeSpan LastTradeTimeDiff
        {
            get
            {
                lastTradeTimeDiff = DateTime.Now.Subtract(LastTradeCloseTime);
                return lastTradeTimeDiff;
            }
            set
            {
                lastTradeTimeDiff = value;
            }
        }
        public OffsetAnalyzer()
        {
            IsBuyOffsetMet = false;
            IsSellOffsetMet = false;
            LastOffsetMetIndex = -1;
            OffsetMetDiffIndex = 0;
        }
        public void ClearAnalyzer()
        {
            IsBuyOffsetMet = false;
            IsSellOffsetMet = false;
            LastOffsetMetIndex = -1;
            OffsetMetDiffIndex = 0;
            LastTradeCloseTime = DateTime.Now;
        }
    }
}
