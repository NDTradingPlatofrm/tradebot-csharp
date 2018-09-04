using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Models
{
    public static class Constants
    {
        public static class ThreadProperties
        {
            public static readonly string THREADSLEEP = "ThreadSleep";
        }
        public static class Directories
        {
            public static readonly string BASEPATH = "BasePath";
            public static readonly string CONFIGPATH = "ConfigPath";
            public static readonly string CONFIGFILE = "config.json";
            public static readonly string CREDCONFIGFILE = "credential.json";
        }
        public static class Environment
        {
            public static readonly string ENVIRONMENT = "Environment";
            public static readonly string TEST = "Test";
            public static readonly string PROD = "Prod";
        }
        public static class Strategies
        {
            public static readonly string SECTIONNAME = "Strategies";
            public static readonly string EXTRAPOLATE = "Extrapolate";
            public static readonly string PUMPNDUMP = "PumpAndDump";
        }
        public static class Markets
        {
            public static readonly string SECTIONNAME = "Markets";
            public static readonly string STOCKS = "Stocks";
            public static readonly string COINS = "Coins";
        }
        public static class Exchanges
        {
            public static readonly string SECTIONNAME = "Exchanges";
            public static readonly string BINANCE = "Binance";
            public static readonly string CRYPTOPIA = "Cryptopia";
            public static readonly string ETRADE = "Etrade";

            public static class EndPoints
            {
                public static readonly string SECTIONNAME = "EndPoints";
            }
            public static class Currencies
            {
                public static readonly string SECTIONNAME = "Currencies";
            }
        }
        public class OrderState
        {
            public const string PENDING = "PENDING";
            public const string FILLED = "FILLED";
            public const string CANCELLED = "CANCELLED";
        }
        public class OrderAction
        {
            public const string BUY = "buy";
            public const string SELL = "sell";
        }
        public class OrderType
        {
            public const string MARKET = "Market";
            public const string LIMIT = "Limit";
        }
        public class TradeMode
        {
            public const string FIXED = "FIXED";
            public const string VARIABLE = "VARIABLE";
        }
        public class PositionState
        {
            public const string CLOSED = "CLOSED";
        }
    }
}
