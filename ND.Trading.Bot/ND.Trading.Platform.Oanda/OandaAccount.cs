using ND.Trading.Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Oanda
{
    public class OandaAccount : IAccount
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public decimal Balance { get; set; }
        public decimal MarginAvailable { get; set; }
        public List<Position> PositionList { get; set; }
        public List<Order> OrderList { get; set; }

        public string LastTransactionId { get; set; }
        public string OpenedTradesJson { get; set; } //This is for saving data for analysis for oanda


        public OandaAccount()
        {
            PositionList = new List<Position>();
            OrderList = new List<Order>();
        }
        public ForexPosition GetForexPosition(string symbol)
        {
            ForexPosition p = null;
            foreach (ForexPosition pos in PositionList)
            {
                if (pos.Symbol == symbol)
                {
                    p = pos;
                    break;
                }
            }
            return p;
        }
    }
}
