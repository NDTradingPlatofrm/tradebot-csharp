using ND.Trading.Platform.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Bot.Core
{
    //Not used - please remove after checking
    public static class GlobalConfig
    {
        private static ConfigRoot rootObject = null;
        private static readonly object zLock = new object();

        public static void SetRootConfig(ConfigRoot root)
        {
            GlobalConfig.rootObject = root;
        }

        public static ConfigRoot GetRootConfig()
        {
            return GlobalConfig.rootObject;
        }

        //Use Lock for synchronisation
        public static void SerializeRootConfig(string path)
        {
            lock (zLock)
            {
                // serialize JSON to a string and then write string to a file
                File.WriteAllText(path, JsonConvert.SerializeObject(GlobalConfig.rootObject));
            }
        }
    }
}
