using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models.Gdax
{
    public class Currency
    {
        [JsonProperty("currency")]
        public string Curren { get; set; }
        [JsonProperty("id")]
        public string AccountId { get; set; }

        public Currency(string curr)
        {
            Curren = curr;
            AccountId = string.Empty;
        }
    }
    public class AccountInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("balance")]
        public decimal Balance { get; set; }
        [JsonProperty("available")]
        public decimal Available { get; set; }
        [JsonProperty("hold")]
        public decimal Hold { get; set; }
        [JsonProperty("profile_id")]
        public string ProfileId { get; set; }
    }

    public class LimitOrderInfo
    {
        [JsonProperty("price")]
        public string Price { get; set; }
        [JsonProperty("size")]
        public string Size { get; set; }
        [JsonProperty("product_id")]
        public string ProductId { get; set; }
        [JsonProperty("side")]
        public string Side { get; set; }

        public LimitOrderInfo(Order o)
        {
            Size = Math.Round(o.Count, 4).ToString();
            ProductId = o.Symbol;
            Side = o.Side;
            Price = o.Price.ToString();
        }
    }

    public class OrderInfo
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("size")]
        public decimal Size { get; set; }
        [JsonProperty("product_id")]
        public string ProductId { get; set; }
        [JsonProperty("side")]
        public string Side { get; set; }
        [JsonProperty("stp")]
        public string Stp { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("time_in_force")]
        public string TimeInForce { get; set; }
        [JsonProperty("post_only")]
        public string PostOnly { get; set; }
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
        [JsonProperty("fill_fees")]
        public decimal FillFees { get; set; }
        [JsonProperty("filled_size")]
        public decimal FilledSize { get; set; }
        [JsonProperty("executed_value")]
        public decimal ExecutedValue { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("settled")]
        public bool Settled { get; set; }


    }
}
