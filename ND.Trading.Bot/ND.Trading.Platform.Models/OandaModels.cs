using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
//using OANDACom = ND.Trading.Platform.Models.Oanda.OANDACommon;

namespace ND.Trading.Platform.Models.Oanda
{
    public class AccountInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("alias")]
        public string Name { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("balance")]
        public decimal Balance { get; set; }
        [JsonProperty("marginAvailable")]
        public decimal MarginAvailable { get; set; }

    }
    public class InstrumentInfo
    {
        public string name { get; set; }
        public string type { get; set; }
        public string displayName { get; set; }
        public int pipLocation { get; set; }
        public int displayPrecision { get; set; }
        public int tradeUnitsPrecision { get; set; }
        public long minimumTradeSize { get; set; }
        public decimal maximumTrailingStopDistance { get; set; }
        public decimal minimumTrailingStopDistance { get; set; }
        public decimal maximumPositionSize { get; set; }
        public long maximumOrderUnits { get; set; }
        public decimal marginRate { get; set; }
    }

    public class PositionInfo
    {
        public string instrument { get; set; }
        public decimal pl { get; set; }
        public decimal unrealizedPL { get; set; }
        public decimal resettablePL { get; set; }
        public PositionSide @long { get; set; }
        public PositionSide @short { get; set; }
    }
    public class PositionSide
    {
        public long units { get; set; }
        public decimal averagePrice { get; set; }
        public List<long> tradeIDs { get; set; }
        public decimal pl { get; set; }
        public decimal unrealizedPL { get; set; }
        public decimal resettablePL { get; set; }
    }
    public class OrderInfo
    {
        public decimal units { get; set; }
        public string instrument { get; set; }
        public decimal price { get; set; }
        public string timeInForce { get; set; }
        public string type { get; set; }
        public string positionFill { get; set; }

        public OrderInfo(Order o)
        {
            this.units = Convert.ToInt64(o.Count);
            this.instrument = o.Symbol;
            this.timeInForce = "FOK";
            this.positionFill = "DEFAULT";
            if (o.Side == "sell")
                this.units *= -1;
            if (o.OrderType == "Limit")
                this.price = o.Price;
            this.type = o.OrderType.ToUpper();
        }
    }

    public class OrderPostResponse : Response
    {
        //[JsonConverter(typeof(TransactionConverter))]
        public ITransaction orderCreateTransaction { get; set; }
        //public OrderFillTransaction orderFillTransaction { get; set; }
        //public OrderCancelTransaction orderCancelTransaction { get; set; }
        //public Transaction.Transaction orderReissueTransaction { get; set; }
        //public Transaction.Transaction orderReissueRejectTransaction { get; set; }
    }
    public class Transaction : ITransaction
    {
        public long id { get; set; }
        public string type { get; set; }
        public string time { get; set; }
        public int userID { get; set; }
        public string accountID { get; set; }
        public long batchID { get; set; }
        public string requestID { get; set; }
    }

    public interface ITransaction
    {
        long id { get; set; }
        string type { get; set; }
        string time { get; set; }
        int userID { get; set; }
        string accountID { get; set; }
        long batchID { get; set; }
        string requestID { get; set; }
    }

    public class ClientExtensions
    {
        public string id { get; set; }
        public string tag { get; set; }
        public string comment { get; set; }
    }

    public class Response
    {
        public long lastTransactionID { get; set; }
        public List<long> relatedTransactionIDs { get; set; }

        public override string ToString()
        {
            // use reflection to display all the properties that have non default values
            StringBuilder result = new StringBuilder();
            var props = this.GetType().GetTypeInfo().DeclaredProperties;
            result.AppendLine("{");
            foreach (var prop in props)
            {
                if (prop.Name != "clientExtensions")
                {
                    object value = prop.GetValue(this);
                    bool valueIsNull = value == null;
                    object defaultValue = null;//OANDACom.GetDefault(prop.PropertyType);
                    bool defaultValueIsNull = defaultValue == null;
                    if ((valueIsNull != defaultValueIsNull) // one is null when the other isn't
                        || (!valueIsNull && (value.ToString() != defaultValue.ToString()))) // both aren't null, so compare as strings
                    {
                        result.AppendLine(prop.Name + " : " + prop.GetValue(this));
                    }
                }
            }
            result.AppendLine("}");
            return result.ToString();
        }
    }

    //public class OANDACommon
    //{
    //    public static object GetDefault(Type t)
    //    {
    //        return typeof(OANDACommon).GetTypeInfo().GetDeclaredMethod("GetDefaultGeneric").MakeGenericMethod(t).Invoke(null, null);
    //    }

    //    public static T GetDefaultGeneric<T>()
    //    {
    //        return default(T);
    //    }
    //}
}
