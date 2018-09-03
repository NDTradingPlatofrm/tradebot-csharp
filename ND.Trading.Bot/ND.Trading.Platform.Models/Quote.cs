using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models
{
    public class Quote
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        [JsonProperty("last_trade_price")]
        public decimal LastBuyTradePrice { get; set; }
        public decimal LastSellTradePrice { get; set; }
    }
}
