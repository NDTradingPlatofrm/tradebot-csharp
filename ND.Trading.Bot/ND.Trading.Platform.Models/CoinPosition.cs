using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models
{
    public class CoinPosition : Position
    {
        
        public decimal ExpectedSellPrice { get; set; }
       

        public CoinPosition(Order order, StrategyInfo si)
            :base(order)
        {
            this.TotalSellFee = 0;
            this.StrategyData = si;
            this.TradeList = new List<StockData>();
            this.TradeList.Add(new StockData(order.Count, order.Cost, order.Price, order.Brokerage));
            //this.CreatedOn = DateTime.Now;
            this.Index = 0;
            this.ModifiedOn = this.TradeList[this.Index].PurchaseTime;
            //this.OrderId = order.OrderId;
            this.Status = order.Status;
            //this.Identifier = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(si.StrategyId));
        }

        public override void UpdateExpectedPrice()
        {
            if (this.StrategyData.SellMode == "FIXED")
                this.ExpectedSellPrice = (this.StrategyData.FixedProfitValue + this.TotalCount) / this.TotalCount;
            else if (this.StrategyData.SellMode == "VARIABLE")
                this.ExpectedSellPrice = (this.TotalCost + (this.TotalCost * (this.StrategyData.SellProfitPercent / 100))) / this.TotalCount;
        }
    }
}
