//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models
{
    public class Account
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public decimal Balance { get; set; }
        public decimal MarginAvailable { get; set; }
    }

    //public class OandaAccount : Account
    //{
    //    public List<ForexPosition> PositionList { get; set; }
    //    public List<Order> OrderList { get; set; }
    //    public string LastTransactionId { get; set; }
    //    public string OpenedTradesJson { get; set; } //This is for saving data for analysis
    //    //public string Type { get; set; } //To understand type specific to strategy like Long, Short, backup etc

    //    public OandaAccount()
    //    {
    //        PositionList = new List<ForexPosition>();
    //        OrderList = new List<Order>();
    //    }
    //    public ForexPosition GetForexPosition(string symbol)
    //    {
    //        ForexPosition p = null;
    //        foreach (ForexPosition pos in PositionList)
    //        {
    //            if (pos.Symbol == symbol)
    //            {
    //                p = pos;
    //                break;
    //            }
    //        }
    //        return p;
    //    }
    //}
}
