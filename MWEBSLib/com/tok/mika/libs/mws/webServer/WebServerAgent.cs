using com.tok.mika.libs.mws;
using com.tok.mika.libs.mws.console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tok.mika.libs.mws.webServer
{
    public class WebServerAgent : ConsoleAgent
    {
        public WebServerAgent(MainDataServer server, string name) : base(server, name)
        {
            return;
        }

        protected override void resultString(string result)
        {
            return;
        }
    }
}
