using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models
{
    public class StrategyInfo
    {
        public string StrategyId { get; set; }
        public string Name { get; set; }
        public string Market { get; set; }
        public string Exchange { get; set; }
        public decimal SellProfitPercent { get; set; }
        public string SellMode { get; set; }
        public decimal FixedProfitValue { get; set; }
        public decimal FixedDownPriceDiff { get; set; }
        public decimal PriceDownPercentage { get; set; }
        public int MinPositionLevel { get; set; }
        public int MaxPositionCount { get; set; }
        public int ExtrapolateCount { get; set; }
        public int ExtrapolateIndex { get; set; }
        public decimal PriceRangeStart { get; set; }
        public decimal PriceRangeEnd { get; set; }
        public decimal MinQuantity { get; set; }
        public string OrderType { get; set; }
        public bool IsProcessTrade { get; set; }
        public decimal FixedTradeProfitValue { get; set; }
        public decimal InitialPositionCount { get; set; }

        public StrategyInfo(string nam, decimal spp, string sm, decimal fpv, decimal fdpd, decimal pdp, int mpl, int mpc, 
            int ec, int ei, decimal prs, decimal pre, decimal mq, string typ, bool isTrdProcess, decimal ftpv, decimal ipc)
        {
            Name = nam;
            SellProfitPercent = spp;
            SellMode = sm;
            FixedProfitValue = fpv;
            FixedDownPriceDiff = fdpd;
            PriceDownPercentage = pdp;
            MinPositionLevel = mpl;
            MaxPositionCount = mpc;
            ExtrapolateCount = ec;
            ExtrapolateIndex = ei;
            PriceRangeStart = prs;
            PriceRangeEnd = pre;
            MinQuantity = mq;
            OrderType = typ;
            IsProcessTrade = isTrdProcess;
            FixedTradeProfitValue = ftpv;
            InitialPositionCount = ipc;
        }

    }
}
