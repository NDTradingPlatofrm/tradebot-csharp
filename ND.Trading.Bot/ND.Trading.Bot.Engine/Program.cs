using ND.Trading.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ND.Trading.Bot.Core;
using ND.Trading.Bot.ObjectFactory;
using ND.Trading.Platform.Models;
using System.Media;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ND.Trading.Bot.Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TradeBot bot = new TradeBot();
                bot.TriggerBot();
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }
    }
}