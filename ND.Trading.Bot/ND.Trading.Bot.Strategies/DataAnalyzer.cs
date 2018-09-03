using MongoDB.Driver;
using ND.Trading.Platform.Models;
using ND.Trading.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Bot.Core
{
    public class DataAnalyzer
    {
        private DBManager dbManager = null;
        private string AccountID { get; set; }
        private string AccountType { get; set; }
        private string Symbol { get; set; }
        public string NewTrades { get; set; }
        private static int recurCount = 0;

        public DataAnalyzer(string symbol, string id, string type)
        {
            Symbol = symbol;
            AccountID = id;
            AccountType = type;
            dbManager = new DBManager();
        }

        public void SaveNewTrades()
        {
            ForexData fx = null;
            JArray trdOpenArray = JArray.Parse(NewTrades);
            foreach (JObject trdOpnItem in trdOpenArray)
            {
                fx = new ForexData();
                fx.Symbol = trdOpnItem["instrument"].Value<string>();
                fx.AccountId = AccountID;
                fx.AccountType = AccountType;
                fx.Count = trdOpnItem["currentUnits"].Value<decimal>();
                fx.PriceBuy = trdOpnItem["price"].Value<decimal>();
                fx.PurchaseTime = DateTime.Parse(trdOpnItem["openTime"].Value<string>());
                fx.TradeId = trdOpnItem["id"].Value<int>();
                SaveExtrapolateBuyData(fx);
            }
        }
        public IEnumerable<ExpBuyInterval> GetExpBuyDataList(string refId)
        {
            IEnumerable<ExpBuyInterval> expBuyList = null;
            FilterDefinition<ExpBuyInterval> filterExp = null;
            if (refId == string.Empty)
                filterExp = Builders<ExpBuyInterval>.Filter.Where(_ => true);
            else
                filterExp = Builders<ExpBuyInterval>.Filter.Where(item => item.ReferanceId == refId);

            expBuyList = dbManager.GetEntityList<ExpBuyInterval>(filterExp);
            return expBuyList;
        }
        public void SaveExtrapolateBuyData(ForexData fx)
        {
            ExpBuyInterval expData;
            string refId = string.Empty;
            if (ConfigurationManager.AppSettings[Constants.Environment.ENVIRONMENT].ToString() == Constants.Environment.PROD)
            {
                FilterDefinition<ActiveTicker> filter = null;
                filter = Builders<ActiveTicker>.Filter.Where(sp => sp.Symbol == fx.Symbol && sp.AccountId == AccountID);
                ActiveTicker aTic = dbManager.GetEntity(filter);

                if (aTic != null)
                {
                    refId = aTic.Id;

                    FilterDefinition<ExpBuyInterval> filterExp = null;
                    filterExp = Builders<ExpBuyInterval>.Filter.Where(sp => sp.ReferanceId == refId && sp.TradeId == fx.TradeId);

                    if (dbManager.GetEntity(filterExp) == null)
                    {
                        expData = new ExpBuyInterval(refId, fx.TradeId, fx.Symbol, fx.PriceBuy, fx.Count, fx.PurchaseTime, fx.AccountId);
                        dbManager.InsertEntity<ExpBuyInterval>(expData);
                    }
                }
                else
                {
                    Console.WriteLine("Active ticker missing, error in logic");
                }
            }
        }
        private DataTable SetLevels(DataTable dt, int levelId)
        {
            DataTable rtnTable = null;
            //int levelId = 0;
            int prevRowVal = 0;
            int curRowVal = 0;
            int cntr = 0;

            recurCount++;
            //levelId = levelNum - (levelNum - recurCount);
            foreach (DataRow row in dt.Rows)
            {
                if (row["Units"].ToString() != String.Empty)
                {
                    curRowVal = Convert.ToInt32(row["Units"].ToString().Trim());
                    if (cntr != 0)
                        row["Level" + levelId] = curRowVal - prevRowVal;
                    else
                        row["Level" + levelId] = 1;
                    prevRowVal = Convert.ToInt32(row["Units"].ToString().Trim());
                    cntr++;
                }
                else
                {
                    Console.WriteLine("Issue in this logic");
                }
            }
            rtnTable = dt.Select("Level" + levelId + " = 1").CopyToDataTable();
            if (levelId != recurCount)
                rtnTable = SetLevels(rtnTable, levelId);
            return rtnTable;
        }
        public void SaveActiveTicker()
        {
            if (ConfigurationManager.AppSettings[Constants.Environment.ENVIRONMENT].ToString() == Constants.Environment.PROD)
            {
                FilterDefinition<ActiveTicker> filter = null;
                filter = Builders<ActiveTicker>.Filter.Where(sp => sp.Symbol == Symbol && sp.AccountId == AccountID);
                if (dbManager.GetEntity(filter) == null)
                    dbManager.InsertEntity<ActiveTicker>(new ActiveTicker(Symbol, AccountID));
            }
        }
        public void CloseActiveTicker()
        {
            FilterDefinition<ActiveTicker> filter = null;
            filter = Builders<ActiveTicker>.Filter.Where(sp => sp.Symbol == Symbol && sp.AccountId == AccountID);
            dbManager.DeleteEntity(filter);
        }
        public class ActiveTicker : Entity
        {
            public string Symbol { get; set; }
            public string AccountId { get; set; }
            public ActiveTicker(string sym, string actId)
            {
                this.Symbol = sym;
                this.AccountId = actId;
            }
        }
        public class ExpBuyInterval : Entity
        {
            public string ReferanceId { get; set; }
            public string AccountId { get; set; }
            public int TradeId { get; set; }
            public string Symbol { get; set; }
            public decimal Price { get; set; }
            public decimal Units { get; set; }
            public DateTime PurchaseTime { get; set; }

            public ExpBuyInterval(string rId, int id, string s, decimal pr, decimal u, DateTime pt, string actId)
            {
                ReferanceId = rId;
                AccountId = actId;
                TradeId = id;
                Symbol = s;
                Price = pr;
                Units = u;
                PurchaseTime = pt;
            }
        }
    }
}


/*
                //for (int i = fxList.Count - 1; i >= 0; i--)
                //{
                //    item = (ForexStock)fxList[i];
                //    if (i == (fxList.Count - 1))
                //    {
                //        expData = new ExpBuyInterval(refId, item.TradeId, item.Symbol, item.PriceBuy, item.PriceBuy, item.Count, item.PurchaseTime, item.PurchaseTime);
                //        DBManager.InsertEntity<ExpBuyInterval>(expData);
                //    }
                //    else
                //    {
                //        expData = new ExpBuyInterval(refId, CurFxStock.TradeId, CurFxStock.Symbol, CurFxStock.PriceBuy, PreFxStock.PriceBuy, CurFxStock.Count, CurFxStock.PurchaseTime, PreFxStock.PurchaseTime);
                //        DBManager.InsertEntity<ExpBuyInterval>(expData);
                //    }
                //}

 //public ForexStock CurFxStock { get; set; }
        //public ForexStock PreFxStock { get; set; }
        //public void SaveDataForAnalysisTemp<T>(T obj, string uniqueId = "")
        //{
        //    Type type = typeof(T);


        //    //Create Dat analyze object here and supply values




        //    if (uniqueId != string.Empty)
        //    {
        //        //Update
        //        if (type.Name == "")
        //        {
        //            DBManager.InsertEntity<Order>((Order)Convert.ChangeType(obj, typeof(Order)));
        //        }
        //    }
        //    else
        //        DBManager.InsertEntity<Order>((Order)Convert.ChangeType(obj, typeof(Order)));
        //}


   

*/