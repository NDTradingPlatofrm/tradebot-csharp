using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models.Robinhood
{
    public class FundamentalInfo
    {
        [JsonProperty("open")]
        public decimal Open { get; set; }
        [JsonProperty("high")]
        public decimal High { get; set; }
        [JsonProperty("low")]
        public decimal Low { get; set; }
        [JsonProperty("volume")]
        public float Volume { get; set; }
        [JsonProperty("high_52_weeks")]
        public decimal High52Weeks { get; set; }
        [JsonProperty("low_52_weeks")]
        public decimal low52Weeks { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
