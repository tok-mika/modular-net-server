using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tok.mika.libs.mws.console
{
    public abstract class ConsoleAgent
    {
        public MainDataServer server;
        private String name;                //Имя агента
        public ConsoleAgent(MainDataServer server, String name)
        { 
            this.server = server;
            this.name = name;
        }

        /// <summary>
        /// Вывести в консоль через агента
        /// </summary>
        /// <param name="info"></param>
        public void ShowInfo(String info)
        {
            DateTime now = DateTime.Now;
            if (server.getLog().getLog(info))
            {
                server.getLog().AddLine("[" + now.ToString("HH:mm:ss][dd.MM.yyyy") + "][out][" + name + "][info]вывод log...\r\n");
            }
            else server.getLog().AddLine("[" + now.ToString("HH:mm:ss][dd.MM.yyyy") + "][out][" + name + "][info]" + info + "\r\n");
            this.resultString(info);
        }
        /// <summary>
        /// Вывести в консоль через агента
        /// </summary>
        /// <param name="info"></param>
        public void ShowError(String info)
        {
            DateTime now = DateTime.Now;
            server.getLog().AddLine("[" + now.ToString("HH:mm:ss][dd.MM.yyyy") + "][out][" + name + "][error]" + info + "\r\n");
            this.resultString(info);
        }
        /// <summary>
        /// Вывести в консоль через агента
        /// </summary>
        /// <param name="info"></param>
        public void ShowWarning(String info)
        {
            DateTime now = DateTime.Now;
            server.getLog().AddLine("[" + now.ToString("HH:mm:ss][dd.MM.yyyy") + "][out][" + name + "][warning]" + info + "\r\n");
            this.resultString(info);
        }

        /// <summary>
        /// Нужна для чтения данных от агента
        /// </summary>
        /// <returns></returns>
        protected string ReadLine(string line)
        {
            DateTime now = DateTime.Now;
            server.getLog().AddLine("[" + now.ToString("HH:mm:ss][dd.MM.yyyy") + "][in][" + name + "]" + line + "\r\n");
            return line;
        }

        /// <summary>
        /// Возвращает ответ агенту в виде строки
        /// </summary>
        /// <param name="result">строка с результатом выполнения команды через данного агента</param>
        protected abstract void resultString(String result);
    }
}
