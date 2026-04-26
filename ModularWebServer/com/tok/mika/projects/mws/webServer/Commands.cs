using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.tok.mika.libs.mws;
using com.tok.mika.libs.mws.console;

namespace com.tok.mika.projects.mws.webServer
{
    internal class Commands
    {
        internal Server WebServer { get; }
        private List<CommandRealisation> _realisationList;
        internal Commands(Server server)
        {
            WebServer = server;
            _realisationList = new List<CommandRealisation>();
            _realisationList.Add(new CommandRealisation("текст", (agent, args) =>
            {
                Console.WriteLine(args[0]);
                return true;
            }, "Это тестовая команад", [
                new CommandParameter("строка", "Введите строку для теста")
            ]));

            _realisationList.Add(new CommandRealisation("вывод", (agent, args) =>
            {
                return false;
            }, "Комманды для вывода информации"));
            _realisationList.Add(new CommandRealisation("сервер", (agent, args) =>
            {
                return false;
            }, "Команды для взаимодействия с сервером"));

            _realisationList.Add(new CommandRealisation("сервер запустить", (agent, args) =>
            {
                WebServer.Start(agent);
                return true;
            }, "Запустить сервер"));
            _realisationList.Add(new CommandRealisation("сервер остановить", (agent, args) =>
            {
                WebServer.Stop(agent);
                return true;
            }, "Остановить работу сервера"));
            _realisationList.Add(new CommandRealisation("вывод лога", (agent, args) =>
            {
                WebServer.main.getLog().show(agent);
                return true;
            }, "Вывести логи сервера"));
            _realisationList.Add(new CommandRealisation("вывод модулей", (agent, args) =>
            {
                WebServer.main.GetPluginLoader().ShowModules(agent);
                return true;
            }, "Вывести список модулей установленных на сервере"));


            foreach (var i in _realisationList) server.main.AddCommand(i);
        }
    }
}
