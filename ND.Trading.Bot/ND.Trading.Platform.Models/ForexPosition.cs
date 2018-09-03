using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models
{
    public class ForexPosition : Position
    {
        public decimal UnrealizedPL { get; set; }
       

        public ForexPosition(Order order)
            : base(order)
        {
            this.TradeList = new List<TradeData>();
            this.TradeList.Add(new ForexData(order.Count, order.Cost, order.Price, order.Brokerage));
            this.ModifiedOn = this.TradeList[this.Index].PurchaseTime;
        }
        public ForexPosition()
        {
            TradeIdList = new List<string>();
        }

        public override void UpdateExpectedPrice()
        {

        }
    }

}
