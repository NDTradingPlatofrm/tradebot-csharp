using ND.Trading.Platform.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Etrade
{
    public class EtradeConvert
    {
        public T DeserializeObject<T>(JToken jToken)
        {
            T obj = default(T);
            try
            {
                Type type = typeof(T);
                if (type.Name == "Quote")
                {

                }
                else if (type.Name == "EtradeAccount")
                {
                    JObject respObject = jToken["json.accountBalanceResponse"].Value<JObject>();
                    JObject accObject = respObject["accountBalance"].Value<JObject>();
                    JObject accMarginObject = respObject["marginAccountBalance"].Value<JObject>();
                    EtradeAccount a = new EtradeAccount();
                    a.Id = respObject["accountId"].Value<string>();
                    //a.Type = respObject["accountType"].Value<string>();
                    //a.Balance = accObject["balance"].Value<decimal>();
                    a.MarginAvailable = accMarginObject["marginBalance"].Value<decimal>();

                    obj = (T)Convert.ChangeType(a, typeof(T));
                }
                else if (type.Name == "StockPosition")
                {
                    StockPosition p = new StockPosition();
                    JObject prodObject = jToken["productId"].Value<JObject>();
                    p.Symbol = prodObject["symbol"].Value<string>();
                    p.TotalCount = jToken["qty"].Value<decimal>();

                    obj = (T)Convert.ChangeType(p, typeof(T));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            //}
            return obj;
        }
    }
}
