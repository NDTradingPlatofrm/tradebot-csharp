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

        public bool IsGoodToBuy()
        {
            bool rtnVal = false;
            List<decimal> lastUnitFactorList = new List<decimal>(); //persist
            string lastUnitFactorId = ""; //Get last unit factor id, keep in seperate class and persist object in strategy class
            decimal lastUnitFactor = 0.0m;

            FilterDefinition<ForexStock> filterExp = null;
            filterExp = Builders<ForexStock>.Filter.Where(sp => sp.Symbol == Symbol && sp.AccountId == AccountID);
            SortDefinition<ForexStock> sortDef = null;
            sortDef = Builders<ForexStock>.Sort.Descending(sp => sp.ModifiedTime);
            List<ForexStock> fxList = dbManager.GetLastEntityList(filterExp, sortDef, 11).ToList();
            if (fxList != null)
            {
                //DateTime dt = fxList[0].ModifiedTime.ToLocalTime();
                if (lastUnitFactorId == string.Empty)
                {
                    lastUnitFactor = fxList[0].Count - fxList[fxList.Count - 1].Count; //persist unit factor if need
                    lastUnitFactorList.Add(lastUnitFactor);
                }
                else if (lastUnitFactorId != fxList[0].Id)
                {

                    lastUnitFactorId = fxList[0].Id;
                    lastUnitFactor = fxList[0].Count - fxList[fxList.Count - 1].Count;
                    decimal min = lastUnitFactorList.Min();
                    decimal buyFactor = min - lastUnitFactor;
                    if (lastUnitFactorList.Count == 10)
                        lastUnitFactorList.RemoveAt(10);
                    else
                        lastUnitFactorList.Add(lastUnitFactor);
                    if (buyFactor >= 4)
                        rtnVal = true;
                }
            }
            return rtnVal;

        }
        public bool IsGoodToSell()
        {
            bool rtnVal = false;

            return rtnVal;
        }
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