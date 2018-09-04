using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models
{
    public interface IAccount
    {
        string Id { get; set; }
        string Type { get; set; }
        string Name { get; set; }
        string Currency { get; set; }
        decimal Balance { get; set; }
        decimal MarginAvailable { get; set; }
        List<Position> PositionList { get; set; }
        List<Order> OrderList { get; set; }

        string OpenedTradesJson { get; set; } //This is for saving data for analysis for Oanda
    }
}
