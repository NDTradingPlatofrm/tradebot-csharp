using ND.Trading.Bot.Core;
using ND.Trading.Platform.Etrade;
using ND.Trading.Platform.Models;
//using ND.Trading.Platform.Binance;
//using ND.Trading.Platform.Gdax;
using ND.Trading.Platform.Oanda;
using ND.Trading.Platform.TDM;
//using ND.Trading.Platform.Robinhood;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Bot.ObjectFactory
{
    public class ExchangeFactory : IFactory
    {
        public T GetObject<T>(IConfig xChangeConfig)
        {
            T obj = default(T);
            if (ConfigurationManager.AppSettings[Constants.Environment.ENVIRONMENT].ToString() == Constants.Environment.PROD)
            {
                switch (((ExchangeConfig)xChangeConfig).Name)
                {
                    case "Oanda":
                        {
                            obj = (T)(object)new OandaExchange((ExchangeConfig)xChangeConfig);
                            break;
                        }
                    case "Etrade":
                        {
                            obj = (T)(object)new EtradeExchange((ExchangeConfig)xChangeConfig);
                            break;
                        }
                    default: throw new ArgumentException("Invalid type", "type");
                }
            }
            else if (ConfigurationManager.AppSettings[Constants.Environment.ENVIRONMENT].ToString() == Constants.Environment.TEST)
            {
                switch (((ExchangeConfig)xChangeConfig).Name)
                {
                    case "Oanda":
                        {
                            obj = (T)(object)new OandaTDM((ExchangeConfig)xChangeConfig);
                            break;
                        }
                    case "Etrade":
                        {
                            obj = (T)(object)new EtradeTDM((ExchangeConfig)xChangeConfig);
                            break;
                        }
                    default: throw new ArgumentException("Invalid type", "type");
                }
            }
            return obj;
        }

        /*    public T GetObject<T>(JObject config)
            {
                string type=config["Name"].ToString();
                switch (type)
                {
                    //case "Binance": return (T)(object)new BinanceExchange(config);
                    //case "Gdax": return (T)(object)new GdaxExchange(config);
                    //case "Robinhood": return (T)(object)new RhExchange(config);
                    case "Oanda": return (T)(object)new OandaExchange(config);
                    default: throw new ArgumentException("Invalid type", "type");
                }

            }*/
    }
}
