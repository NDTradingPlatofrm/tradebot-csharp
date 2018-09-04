using ND.Trading.Bot.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ND.Trading.Bot.Engine
{
    public class TradeBot
    {
        private List<IStrategy> strategyList = null;

        public TradeBot()
        {

        }

        public void TriggerBot()
        {
            Thread newThread = null;
            try
            {
                //Get exchange list
                this.strategyList = ConfigManager.GetConfigurations();
                for (int i = 0; i < this.strategyList.Count; i++)
                {
                    Console.WriteLine("Market: " + this.strategyList[i].GetExchangeObject().GetMarketName() +
                        " | Exchange: " + this.strategyList[i].GetExchangeObject().GetExchangeName() +
                        " | Strategy: " + this.strategyList[i].GetStrategyName() + " Trade triggered");
                    //this.strategyList[i].TriggerStrategy();
                    newThread = new Thread(this.strategyList[i].TriggerStrategy);
                    newThread.Name = this.strategyList[i].GetStrategyId();
                    newThread.Start();
                    break; //TODO remove **********************
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
