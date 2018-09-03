using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Utilities
{
    public class CustomClient : IApiClient
    {

        public Task<string> CallAsync(string xChangeName, Methods method, string endpoint, bool isSigned = false, string parameters = null, string body = null)
        {
            throw new NotImplementedException();
        }
    }
}
