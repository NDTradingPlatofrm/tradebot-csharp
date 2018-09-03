using ND.Trading.Bot.Core;
using ND.Trading.Bot.Strategies;
using ND.Trading.Platform.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Bot.ObjectFactory
{
    public class StrategyFactory : IFactory
    {
         public T GetObject<T>(IConfig stratConfig)
        {
            switch (((StrategyConfig)stratConfig).Name)
            {
                case "Forexpolate": return (T)(object)new Forexpolate((StrategyConfig)stratConfig);
                case "Stockpolate": return (T)(object)new Stockpolate((StrategyConfig)stratConfig);
                default: throw new ArgumentException("Invalid type", "type");
            }
        }
    }
}
