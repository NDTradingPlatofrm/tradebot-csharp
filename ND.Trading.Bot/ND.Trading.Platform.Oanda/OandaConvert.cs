using ND.Trading.Platform.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Oanda
{
    public class OandaConvert
    {


        public T DeserializeObject<T>(JToken jToken)
        {
            T obj = default(T);
            //lock (dzLock)
            //{
            try
            {
                Type type = typeof(T);
                if (type.Name == "Quote")
                {
                    //List<Quote> qList = new List<Quote>();
                    Quote q = new Quote();
                    q.Symbol = jToken["name"].Value<string>();
                    //qList.Add(q);
                    obj = (T)Convert.ChangeType(q, typeof(T));
                }
                else if (type.Name == "OandaAccount")
                {
                    JObject accObject = jToken["account"].Value<JObject>();
                    OandaAccount a = new OandaAccount();
                    a.Id = accObject["id"].Value<string>();
                    a.Balance = accObject["balance"].Value<decimal>();
                    a.MarginAvailable = accObject["marginAvailable"].Value<decimal>();

                    JArray posArray = accObject["positions"].Value<JArray>();
                    ForexPosition p = null;
                    foreach (JObject item in posArray)
                    {
                        JObject longObj = item["long"].Value<JObject>();
                        if (longObj["units"].Value<decimal>() > 0)
                        {
                            p = new ForexPosition();
                            p.Symbol = item["instrument"].Value<string>();
                            p.AccountId = a.Id;
                            p.TotalCount = longObj["units"].Value<decimal>();
                            p.AveragePrice = longObj["averagePrice"].Value<decimal>();
                            p.UnrealizedPL = longObj["unrealizedPL"].Value<decimal>();

                            JArray tradeArray = longObj["tradeIDs"].Value<JArray>();
                            foreach (JValue subItem in tradeArray)
                            {
                                p.TradeIdList.Add(subItem.ToString());
                            }
                            a.PositionList.Add(p);
                        }
                        JObject shortObj = item["short"].Value<JObject>();
                        if (shortObj["units"].Value<decimal>() < 0)
                        {
                            p = new ForexPosition();
                            p.Symbol = item["instrument"].Value<string>();
                            p.AccountId = a.Id;
                            p.TotalCount = shortObj["units"].Value<decimal>();
                            p.AveragePrice = shortObj["averagePrice"].Value<decimal>();
                            p.UnrealizedPL = shortObj["unrealizedPL"].Value<decimal>();

                            JArray tradeArray = shortObj["tradeIDs"].Value<JArray>();
                            foreach (JValue subItem in tradeArray)
                            {
                                p.TradeIdList.Add(subItem.ToString());
                            }
                            a.PositionList.Add(p);
                        }

                    }
                    JArray trdArray = accObject["trades"].Value<JArray>();
                    string sym = string.Empty;
                    ForexData fx = null;
                    foreach (JObject trdItem in trdArray)
                    {
                        fx = new ForexData();
                        if (sym != trdItem["instrument"].Value<string>())
                        {
                            sym = trdItem["instrument"].Value<string>();
                            p = (ForexPosition) a.PositionList.Find(pos => pos.Symbol == sym);
                        }
                        fx.AccountId = a.Id;
                        fx.Symbol = trdItem["instrument"].Value<string>();
                        fx.Count = trdItem["currentUnits"].Value<decimal>();
                        fx.PriceBuy = trdItem["price"].Value<decimal>();
                        fx.PurchaseTime = DateTime.Parse(trdItem["openTime"].Value<string>());
                        fx.UnrealizedPL = trdItem["unrealizedPL"].Value<decimal>();
                        fx.TradeId = trdItem["id"].Value<int>();
                        p.TradeList.Add(fx);
                    }
                    JArray ordArray = accObject["orders"].Value<JArray>();
                    foreach (JObject ordItem in ordArray)
                    {

                    }
                    a.LastTransactionId = jToken["lastTransactionID"].Value<string>();
                    obj = (T)Convert.ChangeType(a, typeof(T));
                }
                else if (type.Name == "Order")
                {
                    //decimal execVal = 0.0m;
                    //Order o = new Order();
                    //o.OrderId = jToken["id"].Value<string>();
                    //o.Price = jToken["price"].Value<decimal>();
                    //o.Count = jToken["size"].Value<decimal>();
                    //o.Brokerage = jToken["fill_fees"].Value<decimal>();
                    //o.Cost = jToken["executed_value"].Value<decimal>();
                    //if (jToken["status"].Value<string>() == "done")
                    //{
                    //    o.Status = "ACTIVE";
                    //    o.Settled = jToken["settled"].Value<bool>();
                    //    /* if (jToken["side"].Value<string>() == "buy")
                    //         o.Cost = execVal + o.Brokerage;
                    //     else if (jToken["side"].Value<string>() == "sell")
                    //         o.Cost = execVal - o.Brokerage;*/
                    //    o.DoneAt = jToken["done_at"].Value<DateTime>();
                    //}
                    //else
                    //    o.Status = "ORDER";
                    //obj = (T)Convert.ChangeType(o, typeof(T));
                }
                else if (type.Name == "ForexPosition")
                {
                    ForexPosition p = new ForexPosition();
                    p.Symbol = jToken["instrument"].Value<string>();
                    JObject longObj = jToken["long"].Value<JObject>();
                    JObject shortObj = jToken["short"].Value<JObject>();

                    if (longObj["units"].Value<decimal>() > 0)
                    {
                        p.TotalCount = longObj["units"].Value<decimal>();
                        p.AveragePrice = longObj["averagePrice"].Value<decimal>();
                        p.UnrealizedPL = longObj["unrealizedPL"].Value<decimal>();
                    }
                    if (shortObj["units"].Value<decimal>() < 0)
                    {
                        p.TotalCount = shortObj["units"].Value<decimal>();
                        p.AveragePrice = shortObj["averagePrice"].Value<decimal>();
                        p.UnrealizedPL = shortObj["unrealizedPL"].Value<decimal>();
                    }

                    JArray tradeArray = longObj["tradeIDs"].Value<JArray>();
                    foreach (JValue item in tradeArray)
                    {
                        p.TradeIdList.Add(item.ToString());
                    }

                    obj = (T)Convert.ChangeType(p, typeof(T));
                }
                else if (type.Name == "ForexData")
                {
                    ForexData fx = new ForexData();
                    fx.Symbol = jToken["instrument"].Value<string>();
                    fx.Count = jToken["currentUnits"].Value<decimal>();
                    fx.PriceBuy = jToken["price"].Value<decimal>();
                    fx.PurchaseTime = DateTime.Parse(jToken["openTime"].Value<string>());
                    fx.UnrealizedPL = jToken["unrealizedPL"].Value<decimal>();
                    fx.TradeId = jToken["id"].Value<int>();

                    obj = (T)Convert.ChangeType(fx, typeof(T));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            //}
            return obj;
        }
        public IEnumerable<string> DeserializeAccountId(JToken jToken)
        {
            List<string> accountIdList = new List<string>();
            string id = string.Empty;
            JArray actArray = jToken["accounts"].Value<JArray>();

            foreach (JObject item in actArray)
            {
                id = item["id"].Value<string>();
                accountIdList.Add(id);
            }

            return accountIdList;
        }
        public string SerializeObject<T>(T obj)
        {
            string rtnVal = string.Empty;
            //lock (zLock)
            //{
            try
            {
                Type type = typeof(T);
                string jsonBody = JsonConvert.SerializeObject(obj);
                if (type.Name == "OrderInfo")
                    rtnVal = "{  \"order\":" + jsonBody + "}";
            }
            catch (Exception ex)
            {
                throw;
            }
            // }
            return rtnVal;
        }
        public bool UpdateOrderDetails(JToken jToken, Order ord)
        {
            bool rtnVal = false;
            //lock (oLock)
            //{
            try
            {
                JObject ordCrtObject = jToken["orderCreateTransaction"].Value<JObject>();
                ord.OrderId = ordCrtObject["id"].Value<string>();

                JToken tkFill = jToken["orderFillTransaction"];

                if (tkFill == null)
                {
                    JObject ordCancelObject = jToken["orderCancelTransaction"].Value<JObject>() == null ? null : jToken["orderCancelTransaction"].Value<JObject>();
                    ord.Status = "CANCELLED";
                }
                else
                {
                    JObject ordFillObject = jToken["orderFillTransaction"].Value<JObject>();
                    if (ordFillObject["type"].Value<string>() == "ORDER_FILL")
                    {
                        ord.Count = ordFillObject["units"] == null ? 0.0m : ordFillObject["units"].Value<decimal>();
                        ord.Price = ordFillObject["price"] == null ? 0.0m : ordFillObject["price"].Value<decimal>();
                        ord.PnL = ordFillObject["pl"] == null ? 0.0m : ordFillObject["pl"].Value<decimal>();
                        ord.Brokerage = ordFillObject["financing"] == null ? 0.0m : ordFillObject["financing"].Value<decimal>();
                        ord.TradeId = ordFillObject["id"] == null ? string.Empty : ordFillObject["id"].Value<string>();
                        ord.DoneAt = DateTime.Parse(ordFillObject["time"].Value<string>());
                        ord.Status = "FILLED";
                    }
                    else
                        ord.Status = "PENDING";
                }
                rtnVal = true;
            }
            catch (Exception ex)
            {
                throw;
            }
            //}
            return rtnVal;
        }
        public bool UpdateFxStockDetails(JToken jToken, ForexData fxS)
        {
            bool rtnVal = false;
            //lock (fxLock)
            //{
            try
            {
                JObject ordCrtObject = jToken["orderCreateTransaction"].Value<JObject>();
                JToken tkFill = jToken["orderFillTransaction"];

                if (tkFill == null)
                {
                    JObject ordCancelObject = jToken["orderCancelTransaction"].Value<JObject>() == null ? null : jToken["orderCancelTransaction"].Value<JObject>();
                }
                else
                {
                    JObject ordFillObject = jToken["orderFillTransaction"].Value<JObject>();
                    if (ordFillObject["type"].Value<string>() == "ORDER_FILL")
                    {
                        fxS.Count = ordFillObject["units"].Value<decimal>();
                        fxS.PriceSell = ordFillObject["price"].Value<decimal>();
                        fxS.FeeSell = ordFillObject["financing"].Value<decimal>();
                        fxS.ProfitnLoss = ordFillObject["pl"].Value<decimal>();
                        fxS.ModifiedTime = DateTime.Parse(ordFillObject["time"].Value<string>());
                        //JArray clsTrades = ordFillObject["tradesClosed"].Value<JArray>();
                        //if (clsTrades!=null)
                        //{

                        //}
                        rtnVal = true;
                    }
                    //else
                    //    ord.Status = "PENDING";
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            // }
            return rtnVal;
        }
        public void UpdateAccountChanges(JToken jToken, OandaAccount act)
        {
            //lock (dzLock)
            //{
            ForexPosition pos = null;
            ForexData fx = null;
            try
            {
                if (act.LastTransactionId != jToken["lastTransactionID"].Value<string>())
                {
                    act.LastTransactionId = jToken["lastTransactionID"].Value<string>();
                    JObject chgObject = jToken["changes"].Value<JObject>();
                    JArray trdOpenArray = chgObject["tradesOpened"].Value<JArray>();
                    JArray trdCloseArray = chgObject["tradesClosed"].Value<JArray>();
                    JArray posArray = chgObject["positions"].Value<JArray>();
                    JArray filledOrdArray = chgObject["ordersFilled"].Value<JArray>();
                    JArray cancelOrdArray = chgObject["ordersCancelled"].Value<JArray>();
                    act.OpenedTradesJson = trdOpenArray.ToString();
                    foreach (JObject posItem in posArray)
                    {
                        pos = (ForexPosition) act.PositionList.Find(obj => obj.Symbol == posItem["instrument"].Value<string>());
                        if (pos == null) //New position added
                        {
                            pos = new ForexPosition();
                            pos.AccountId = act.Id;
                            act.PositionList.Add(pos);
                        }
                        pos.Symbol = posItem["instrument"].Value<string>();
                        JObject longObj = posItem["long"].Value<JObject>();
                        if (longObj["units"].Value<decimal>() != 0) //Existing long position updated
                        {
                            pos.TotalCount = longObj["units"].Value<decimal>();
                            pos.AveragePrice = longObj["averagePrice"].Value<decimal>();

                            JArray tradeArray = longObj["tradeIDs"].Value<JArray>();
                            pos.TradeIdList.Clear();
                            foreach (JValue item in tradeArray)
                            {
                                pos.TradeIdList.Add(item.ToString());
                            }
                        }
                        JObject shortObj = posItem["short"].Value<JObject>();
                        if (shortObj["units"].Value<decimal>() != 0) //Existing short position updated
                        {
                            pos.TotalCount = shortObj["units"].Value<decimal>();
                            pos.AveragePrice = shortObj["averagePrice"].Value<decimal>();

                            JArray tradeArray = shortObj["tradeIDs"].Value<JArray>();
                            pos.TradeIdList.Clear();
                            foreach (JValue item in tradeArray)
                            {
                                pos.TradeIdList.Add(item.ToString());
                            }
                        }
                        if (longObj["units"].Value<decimal>() == 0 && shortObj["units"].Value<decimal>() == 0) //Existing position closed
                            act.PositionList.Remove(pos);


                    }
                    foreach (JObject trdClsItem in trdCloseArray)
                    {
                        var trdId = trdClsItem["id"].Value<int>();
                        pos = (ForexPosition) act.PositionList.Find(obj => obj.Symbol == trdClsItem["instrument"].Value<string>());
                        if (pos != null)
                            pos.TradeList.Remove(pos.TradeList.Find(tr => tr.TradeId == trdId));
                    }

                    foreach (JObject trdOpnItem in trdOpenArray)
                    {
                        fx = new ForexData();
                        fx.Symbol = trdOpnItem["instrument"].Value<string>();
                        fx.AccountId = act.Id;
                        fx.Count = trdOpnItem["currentUnits"].Value<decimal>();
                        fx.PriceBuy = trdOpnItem["price"].Value<decimal>();
                        fx.PurchaseTime = DateTime.Parse(trdOpnItem["openTime"].Value<string>());
                        fx.TradeId = trdOpnItem["id"].Value<int>();
                        pos = (ForexPosition) act.PositionList.Find(obj => obj.Symbol == trdOpnItem["instrument"].Value<string>());
                        pos.TradeList.Add(fx);
                    }
                }


                JObject stObject = jToken["state"].Value<JObject>();
                JArray trdArray = stObject["trades"].Value<JArray>();
                JArray pArray = stObject["positions"].Value<JArray>();

                foreach (JObject psItem in pArray)
                {
                    pos = (ForexPosition) act.PositionList.Find(obj => obj.Symbol == psItem["instrument"].Value<string>());
                    pos.UnrealizedPL = psItem["netUnrealizedPL"].Value<decimal>();
                }
                foreach (JObject tdItem in trdArray)
                {
                    //pos = act.PositionList.Find(obj => obj.Symbol == tdItem["instrument"].Value<string>());
                    //fx = (ForexData) pos.TradeList.Find(obj => obj.Id == tdItem["id"].Value<string>());
                    fx = GetFxStockFromTradeId(act.PositionList, tdItem["id"].Value<int>());
                    if (fx != null)
                        fx.UnrealizedPL = tdItem["unrealizedPL"].Value<decimal>();
                    else
                        Console.WriteLine("Error while getting unrealized profit for trades");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            // }
        }
        private ForexData GetFxStockFromTradeId(List<Position> posList, int id)
        {
            ForexData fxSt = null;
            foreach (ForexPosition fxP in posList)
            {
                fxSt = (ForexData)fxP.TradeList.Find(obj => obj.TradeId == id);
                if (fxSt != null)
                    return fxSt;
            }
            return fxSt;
        }
    }
}
