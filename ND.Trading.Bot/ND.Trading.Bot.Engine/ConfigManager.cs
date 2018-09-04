using ND.Trading.Bot.Core;
using ND.Trading.Bot.ObjectFactory;
using ND.Trading.Platform.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Bot.Engine
{
    public static class ConfigManager
    {
        public static JObject jsonObject = null;
        public static string basePath = System.IO.Directory.GetCurrentDirectory().ToString();
        public static string configPath = ConfigurationManager.AppSettings[Constants.Directories.CONFIGPATH].ToString();
        public static string configFile = basePath + configPath + Constants.Directories.CONFIGFILE;
        public static int threadSleep = Convert.ToInt32(ConfigurationManager.AppSettings[Constants.ThreadProperties.THREADSLEEP].ToString());
        


        public static List<IStrategy> GetConfigurations()
        {
            List<IStrategy> strategyList = new List<IStrategy>();
            IFactory xChangeObjectFactory = null;
            IFactory strategyObjectFactory = null;
            IStrategy stratObj = null;
          //  IExchange xChangeObj = null;

            using (StreamReader file = File.OpenText(configFile))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                jsonObject = (JObject)JToken.ReadFrom(reader);
            }

          
            var rootObject = JsonConvert.DeserializeObject<ConfigRoot>(jsonObject.ToString());
            GlobalConfig.SetRootConfig(rootObject);

            //Do Validations here.....  ************************************ NEED CONFIG VALIDATIONS HERE  *********************

            //NEED CONFIG VALIDATIONS HERE 

            //******************************************************************************************************************

            foreach (ExchangeConfig xChng in rootObject.Exchanges)
            {
                if (xChng.ActiveStatus)
                {
                    xChangeObjectFactory = new ExchangeFactory();
                    //xChangeObj = xChangeObjectFactory.GetObject<IExchange>(xChng);
                    foreach (StrategyConfig strgyItem in xChng.Strategies)
                    {
                        strategyObjectFactory = new StrategyFactory();
                        if (strgyItem.ActiveStatus)
                        {
                            stratObj = strategyObjectFactory.GetObject<IStrategy>(strgyItem);
                            //xChng.UniqueStrategyId = stratObj.GetStrategyId();
                            stratObj.SetExchangeDetails(xChangeObjectFactory.GetObject<IExchange>(xChng));
                            stratObj.SetConfigPath(configFile);
                            //stratObj.SetStrategyId();
                            strategyList.Add(stratObj);
                        }
                    }
                }
            }
            return strategyList;
        }
    }
}
