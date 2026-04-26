using com.tok.mika.libs.mws;
using com.tok.mika.libs.mws.console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace com.tok.mika.projects.mws.agents.telnet
{
    internal class TelNetAgent : ConsoleAgent
    {
        private bool _connect;
        private NetworkStream _stream;
        private string _prestring;
        public TelNetAgent(MainDataServer server, string name, NetworkStream stream, string presString) : base(server, name)
        {
            _stream = stream;
            _prestring = presString;
            _connect = true;
        }

        public string ReadLine()
        {
            try
            {
                string message = "\n\r" + _prestring;
                byte[] data = Encoding.UTF8.GetBytes(message);
                _stream.Write(data, 0, data.Length);
                string line = TelNetServer.ReadStrFromStream(_prestring, _stream);
                return line;
            }
            catch (Exception ex)
            {
                _connect = false;
                server.RemoveAgent(this);
                server.ShowError(ex.Message);
                return "";
            }
        }

        protected override void resultString(string result)
        {
            try
            {
                string message = "\n\r" + result;
                byte[] data = Encoding.UTF8.GetBytes(message);
                _stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                _connect = false;
                server.RemoveAgent(this);
                server.ShowError(ex.Message);
            }
        }

        public bool Connect()
        {
            return _connect;
        }
    }
}
