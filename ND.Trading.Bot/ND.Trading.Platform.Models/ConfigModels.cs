using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models
{
    public interface IConfig
    {
    }
    public class ConfigRoot : IConfig
    {
        public List<ExchangeConfig> Exchanges { get; set; }
        public ConfigRoot()
        {
            Exchanges = new List<ExchangeConfig>();
        }
    }
    public class ExchangeConfig : IConfig
    {
        public string Name { get; set; }
        public string Market { get; set; }
        public string BaseUrl { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string Passphrase { get; set; }
        public string Token { get; set; }
        public int ApiDelay { get; set; }
        //public string UniqueStrategyId { get; set; }
        public string Tickers { get; set; }
        public string Currencies { get; set; }
        public bool ActiveStatus { get; set; }
        public List<StrategyConfig> Strategies { get; set; }
        public ExchangeConfig()
        {
            Strategies = new List<StrategyConfig>();
        }
    }
    public class StrategyConfig : IConfig
    {

        public string Name { get; set; }
        public string SubName { get; set; }
        public string ShortCode { get; set; }
        public string OrderType { get; set; }
        public bool ActiveStatus { get; set; }
        public int MinPositionLevelPercent { get; set; }
        public int MaxPositionCount { get; set; }
        public bool IsFullLongBackUpEnabled { get; set; }
        public bool IsFullShortBackUpEnabled { get; set; }
        public List<AccountConfig> Accounts { get; set; }
        public List<UserConfig> Users { get; set; }
    }
    public class AccountConfig : IConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
    public class UserConfig : IConfig
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string StartStatus { get; set; }
        public bool ActiveStatus { get; set; }

        public List<PositionConfig> BuyList { get; set; }
    }
    public class PositionConfig : IConfig
    {
        public string Symbol { get; set; }
        public string BuyMode { get; set; }
        public string SellMode { get; set; }
        public decimal FixedProfitValue { get; set; }
        public decimal FixedBuyPriceValue { get; set; }
        public decimal PriceDownPercent { get; set; }
        public decimal SellProfitPercent { get; set; }
        public string ThresholdMode { get; set; }
        public decimal Value { get; set; }
        public decimal InitialUnits { get; set; }
        public decimal ExtrapolateLevel { get; set; }
        public decimal ExtrapolateUnitFactor { get; set; }
        public decimal PriceRangeStart { get; set; }
        public decimal PriceRangeEnd { get; set; }
        public decimal MinQuantity { get; set; }
        public bool TradeProcess { get; set; }
        public string TradeProcessProfitMode { get; set; }
        public decimal TradeProcessProfitPercentage { get; set; }
        public decimal FixedTradeProcessProfitValue { get; set; }
        public bool OffsetTradeProcess { get; set; }
        public bool LongBackupActive { get; set; }
        public bool ShortBackupActive { get; set; }

    }
}
