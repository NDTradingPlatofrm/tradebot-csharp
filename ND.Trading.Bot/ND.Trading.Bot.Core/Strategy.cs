using MongoDB.Driver;
using ND.Trading.Platform.Models;
using ND.Trading.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Bot.Core
{
    public abstract class Strategy : IStrategy
    {
        private string Name { get; set; }
        private string SubName { get; set; }
        private string UniqueId { get; set; }
        private string ShortCode { get; set; }
        private bool ActiveStatus { get; set; }
        protected int MinPositionLevelPercent { get; set; }
        protected int MaxPositionCount { get; set; }
        protected string OrderType { get; set; }
        protected List<AccountConfig> AccountConfigList { get; set; }
        protected List<UserConfig> UserConfigList { get; set; }
        protected string MyStrategyJson { get; set; }
        protected string ConfigFile { get; set; }

        protected int ExitLoop { get; set; }
        protected decimal CurrentPrice { get; set; }
        protected IExchange ExchangeObject { get; set; }
        protected Position StockObject { get; set; }
        

        protected DBManager dbManager = null;

        public abstract void TriggerStrategy();

        protected Strategy(StrategyConfig config)
        {
            this.Name = config.Name;
            this.SubName = config.SubName;
            this.ShortCode = config.ShortCode;
            this.ActiveStatus = config.ActiveStatus;
            this.MinPositionLevelPercent = config.MinPositionLevelPercent;
            this.MaxPositionCount = config.MaxPositionCount;
            this.OrderType = config.OrderType;
            this.AccountConfigList = config.Accounts;
            this.UserConfigList = config.Users;
            this.MyStrategyJson = JsonConvert.SerializeObject(config);
            this.ExitLoop = 0;
            UniqueId = Guid.NewGuid().ToString();
            dbManager = new DBManager();
        }
        public void SetExchangeDetails(IExchange xChange)
        {
            this.ExchangeObject = xChange;
        }
        public IExchange GetExchangeObject()
        {
            return ExchangeObject;
        }
        public void SetConfigPath(string val)
        {
            ConfigFile=val;
        }
        protected bool UpdatePositionsForDataAnalysis()
        {
            bool rtnVal = false;
            if (dbManager.InsertEntity<Position>(this.StockObject))
            {
                rtnVal = true;
            }
            return rtnVal;
        }
        protected PositionConfig GetPositionConfig(string symbol)
        {
            PositionConfig pConfig = null;

            for (int i = 0; i < UserConfigList.Count(); i++)
            {
                PositionConfig pos = UserConfigList[i].BuyList.Find(x => x.Symbol == symbol);
                if (pos != null)
                    return pos;
            }
            return pConfig;
        }
        protected UserConfig GetUserConfig(string symbol)
        {
            UserConfig uConfig = null;
            for (int i = 0; i < UserConfigList.Count(); i++)
            {
                PositionConfig pos = UserConfigList[i].BuyList.Find(x => x.Symbol == symbol);
                if (pos != null)
                    return UserConfigList[i];
            }
            return uConfig;
        }
        protected string GetAccountId(string typeName)
        {
            string longAccountId = string.Empty;
            AccountConfig acConfig = AccountConfigList.Find(x => x.Type == typeName && x.Name == ShortCode + "_" + typeName);
            if (acConfig != null)
                longAccountId = acConfig.Id;

            return longAccountId;
        }
     
        protected string GetAccountType(string id)
        {
            string type = string.Empty;
            AccountConfig acConfig = AccountConfigList.Find(x => x.Id == id);
            if (acConfig != null)
                type = acConfig.Type;

            return type;
        }

        public string GetStrategyId()
        {
            return UniqueId;
        }
        public string GetStrategyName()
        {
            return Name;
        }
    }
}
