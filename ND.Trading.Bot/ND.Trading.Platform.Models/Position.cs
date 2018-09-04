using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models
{
    public abstract class Position : Entity
    {
        public string Symbol { get; set; }
        public decimal TotalCount { get; set; }
        public decimal AveragePrice { get; set; }
        public short Index { get; set; }
        public string Status { get; set; }

        public decimal TotalBuyFee { get; set; }
        public decimal TotalCost { get; set; }
        public decimal SellPrice { get; set; }
        public decimal TotalSellFee { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal ProfitnLoss { get; set; }
        public decimal ProfitPercentage { get; set; }

        public DateTime ModifiedOn { get; set; }
        public List<TradeData> TradeList { get; set; }
        public StrategyInfo StrategyData { get; set; }
        public string OrderId { get; set; }
        public string UserName { get; set; }
        public string Identifier { get; set; }

        public List<string> TradeIdList { get; set; }
        public string AccountId { get; set; }
        public string AccountType { get; set; }

        public Position()
        {
            TradeList = new List<TradeData>();
        }

        public Position(Order order)
        {
            this.Symbol = order.Symbol;
            this.TotalCount = order.Count;
            this.TotalCost = order.Cost;
            this.AveragePrice = order.Price;
            this.TotalBuyFee = order.Brokerage;
            this.TotalSellFee = 0;
            //this.StrategyData = si;
            //this.UpdateExpectedPrice();
            this.ModifiedOn = DateTime.Now;
            this.Index = 0;
            this.OrderId = order.OrderId;
            this.Status = order.Status;
            //this.Identifier = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(si.StrategyId));
        }

        public abstract void UpdateExpectedPrice();


        public bool AddMoreStocks(Order order)
        {
            bool rtnVal = false;
            try
            {
                //this.TotalCount += order.Count;
                // this.TotalCost += order.Cost;
                // this.TotalBuyFee += order.Brokerage;
                this.TradeList.Add(new TradeData(order.Count, order.Cost, order.Price, order.Brokerage));
                //this.UpdateAveragePrice();
                // this.UpdateExpectedPrice();
                this.Index++;
                this.ModifiedOn = this.TradeList[this.Index].PurchaseTime;
                this.OrderId = order.OrderId;
                this.Status = order.Status;
                rtnVal = true;
            }
            catch (Exception ex)
            {
                throw;
            }
            return rtnVal;
        }

        protected void UpdateAveragePrice()
        {
            decimal totalPrice = 0.0m;

            foreach (TradeData dat in this.TradeList)
            {
                totalPrice += dat.PriceBuy;
            }
            AveragePrice = totalPrice / this.TradeList.Count;
        }
    }
}
