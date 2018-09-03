using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models
{
    public class TradeData : Entity
    {
        public int TradeId { get; set; }
        public string Symbol { get; set; }
        public decimal Count { get; set; }
        public decimal PriceBuy { get; set; }
        public decimal FeeBuy { get; set; }
        public decimal Cost { get; set; }
        public decimal PriceSell { get; set; }
        public decimal FeeSell { get; set; }
        public decimal Revenue { get; set; }
        public decimal ProfitnLoss { get; set; }
        public DateTime PurchaseTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string UserName { get; set; }

        public TradeData()
        {     }
        public TradeData(decimal count, decimal cost, decimal price, decimal charge)
        {
            this.Count = count;
            this.Cost = cost;
            this.PriceBuy = price;
            this.FeeBuy = charge;
            this.PurchaseTime = DateTime.Now;
        }
        public void UpdateTradeData(decimal count, decimal cost, decimal price, decimal charge, DateTime dt)
        {
            this.Count = count;
            this.Cost = cost;
            this.PriceBuy = price;
            this.FeeBuy = charge;
            this.ModifiedTime = dt;
        }
    }
}
