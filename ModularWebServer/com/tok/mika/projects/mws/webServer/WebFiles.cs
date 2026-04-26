using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.tok.mika.libs.mws.console;

namespace com.tok.mika.projects.mws.webServer
{
    internal class WebFiles
    {
        public Server server { get; }
        public WebFiles(Server server) 
        { 
            this.server = server;
        }


        public byte[]? getFile(ConsoleAgent agent ,string path)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/");
            filePath += "files" + path;
            if (File.Exists(filePath))
            {
                agent.ShowInfo("Открыт файл: " + filePath);
                FileStream fileStream = File.OpenRead(filePath);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);
                fileStream.Close();
                return buffer;
            }
            return null;
        }
    }
}
