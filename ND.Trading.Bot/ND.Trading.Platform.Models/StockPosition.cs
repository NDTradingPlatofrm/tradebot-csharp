using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models
{
    public class StockPosition : Position
    {
       
        //public decimal ExpectedSellPrice { get; set; }
       

        public StockPosition(Order order, StrategyInfo si)
            : base(order)
        {
            this.TradeList = new List<TradeData>();
            this.TradeList.Add(new StockData(order.Count, order.Cost, order.Price, order.Brokerage));
            this.ModifiedOn = this.TradeList[this.Index].PurchaseTime;
        }
        public StockPosition()
        {
            TradeIdList = new List<string>();
        }

        public override void UpdateExpectedPrice()
        {
       
        }
    }
}
