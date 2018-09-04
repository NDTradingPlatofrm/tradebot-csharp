using ND.Trading.Platform.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Bot.Core
{
    public interface IStrategy
    {
        void SetExchangeDetails(IExchange xChange);
        void SetConfigPath(string val);
        void TriggerStrategy();
        IExchange GetExchangeObject();
        string GetStrategyName();
        string GetStrategyId();
    }
}
