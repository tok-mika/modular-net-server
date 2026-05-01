using com.tok.mika.libs.mws;
using com.tok.mika.libs.mws.console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
            /*_realisationList.Add(new CommandRealisation(WebServer, "текст", (agent, args) =>
            {
                Console.WriteLine(args[0]);
                return true;
            }, "Это тестовая команад.", [
                new CommandParameter("строка", "Введите строку для теста")
            ]));*/

            _realisationList.Add(new CommandRealisation(WebServer, "вывод", (agent, args) =>
            {
                return false;
            }, "Комманды для вывода информации."));
            _realisationList.Add(new CommandRealisation(WebServer, "сервер", (agent, args) =>
            {
                return false;
            }, "Команды для взаимодействия с сервером."));

            _realisationList.Add(new CommandRealisation(WebServer, "сервер запустить", (agent, args) =>
            {
                WebServer.Start(agent);
                return true;
            }, "Запустить сервер."));
            _realisationList.Add(new CommandRealisation(WebServer, "сервер остановить", (agent, args) =>
            {
                WebServer.Stop(agent);
                return true;
            }, "Остановить работу сервера."));
            _realisationList.Add(new CommandRealisation(WebServer, "вывод лога", (agent, args) =>
            {
                WebServer.main.getLog().show(agent);
                return true;
            }, "Вывести логи сервера."));
            _realisationList.Add(new CommandRealisation(WebServer, "вывод модулей", (agent, args) =>
            {
                WebServer.main.GetPluginLoader().ShowModules(agent);
                return true;
            }, "Вывести список модулей установленных на сервере."));
            _realisationList.Add(new CommandRealisation(WebServer, "сервер модули", (agent, args) =>
            {
                return true;
            }, "Команды управления модулями сервера."));
            _realisationList.Add(new CommandRealisation(WebServer, "сервер модули выгрузить", (agent, args) =>
            {
                WebServer.main.GetPluginLoader().unLoadAll(agent);
                return true;
            }, "Выгрузить все модули с сервера."));
            _realisationList.Add(new CommandRealisation(WebServer, "сервер модули загрузить", (agent, args) =>
            {
                WebServer.main.GetPluginLoader().Load(agent);
                return true;
            }, "Загрузить модули на сервер"));
            _realisationList.Add(new CommandRealisation(WebServer, "сервер модуль", (agent, args) =>
            {
                return false;
            }, "Команды управления модулем сервера."));
            _realisationList.Add(new CommandRealisation(WebServer, "сервер модуль загрузить", (agent, args) =>
            {
                string address = Path.Combine(this.WebServer.main.MainDir(), "modules", args[0] + ".dll");
                WebServer.main.GetPluginLoader().Load(agent, address);
                return true;
            }, "Загрузить модуль на сервер из файла.", [new CommandParameter("адрес", "Путь до файла модуля")]));
            _realisationList.Add(new CommandRealisation(WebServer, "сервер модуль выгрузить", (agent, args) =>
            {
                WebServer.main.GetPluginLoader().unLoad(agent, args[0]);
                return true;
            }, "Выгрузить модуль с сервера.", [new CommandParameter("имя", "Короткое имя модуля")]));
            _realisationList.Add(new CommandRealisation(WebServer, "сервер модуль уровень", (agent, args) =>
            {
                int level = 0;
                for(int i = 0; i < args[1].Length; i++)
                {
                    if(!(args[1][i] >= '0' &&  args[1][i] <= '9'))
                    {
                        agent.ShowError("Параметер <уровень> должен быть целым числом");
                        return true;
                    }
                }
                level = Convert.ToInt32(args[1]);
                WebServer.main.GetPluginLoader().SetLevelModule(agent, args[0], level);
                return true;
            }, "Установить приоритет загрузки. Чем больше значение, тем раньше будут исполняться функции модуля.", [new CommandParameter("имя", "Короткое имя модуля"),
            new CommandParameter("уровень", "Уровень приоритета загрузки. Чем больше значение, тем раньше будут исполняться функции модуля.")]));


            foreach (var i in _realisationList) server.main.AddCommand(i);
        }
    }
}
