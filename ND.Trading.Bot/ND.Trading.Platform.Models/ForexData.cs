using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models
{
    public class ForexData : TradeData
    {
        public string AccountId { get; set; }
        public string AccountType { get; set; }
        public decimal UnrealizedPL { get; set; }

        public ForexData()
        { }

        public ForexData(decimal count, decimal cost, decimal price, decimal charge)
            :base(count, cost, price, charge)
        {
            
        }
    }

    public class ForexStock: TradeData
    {
        public string AccountId { get; set; }
        public string AccountType { get; set; }
        public decimal UnrealizedPL { get; set; }

        public ForexStock()
        { }

        public ForexStock(decimal count, decimal cost, decimal price, decimal charge)
            : base(count, cost, price, charge)
        {

        }
    }
}
