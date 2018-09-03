using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models
{
    public class Order : Entity
    {
        public string OrderId { get; set; }
        public string OrderType { get; set; }
        public string Side { get; set; }
        public string Symbol { get; set; }
        public decimal Count { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public decimal Brokerage { get; set; }
        public decimal PnL { get; set; }
        public string Status { get; set; }
        public bool Settled { get; set; }
        public string TradeId { get; set; }
        public DateTime DoneAt { get; set; }

        public Order() { }

        public Order(string symbol, decimal count, decimal price, string side = "", string type = "")
        {
            OrderId = string.Empty;
            Symbol = symbol;
            Count = count;
            Price = price;
            Cost = count * price;
            Side = side;
            OrderType = type;
        }
    }
}
