using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.tok.mika.libs.mws;
using com.tok.mika.libs.mws.console;

namespace com.tok.mika.projects.mws
{
    internal class ConsoleAgentCMD : ConsoleAgent
    {
        public ConsoleAgentCMD(MainDataServer server, string name) : base(server, name)
        {

        }

        public string ReadLine()
        {
            string? line = Console.ReadLine();
            if (line == null) line = "";
            base.ReadLine(line);
            return line;
        }

        protected override void resultString(string result)
        {
            Console.WriteLine(result);
        }
    }
}
