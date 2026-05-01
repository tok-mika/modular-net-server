using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.tok.mika.libs.mws;
using com.tok.mika.libs.mws.console;
using com.tok.mika.libs.mws.webServer;
using com.tok.mika.projects.mws.agents.telnet;

namespace com.tok.mika.projects.mws
{
    public class MainData : MainDataServer
    {
        private bool _off;
        private ConsoleLog _log;
        private ConsoleAgentCMD _agent; //agent CMD
        //private List<WebServerCommands> commandsEvents;
        private List<CommandRealisation> _commands;
        private List<ConsoleAgent> _agents;
        private webServer.Server server;
        private PluginsLoader pluginsLoader;
        private WebServerAgent webServerAgent;
        public MainData()
        {
            _agents = new List<ConsoleAgent>();
            _log = new ConsoleLog(this);
            _agent = new ConsoleAgentCMD(this, "cmd");
            webServerAgent = new WebServerAgent(this, "server");
            _agents.Add(_agent);
            _agents.Add(webServerAgent);
            //this.commandsEvents = new List<WebServerCommands>();
            //this.commandsEvents.Add(new webServer.Commands(this));
            _commands = new List<CommandRealisation>();
            pluginsLoader = new PluginsLoader(this);
            //pluginsLoader.Load(_agent);
            this.server = new webServer.Server(_agent, "http://127.0.0.1:8486/", this, pluginsLoader);

            //this.server = new webServer.Server("http://192.168.1.11:8486/", this, pluginsLoader);
            //this.server = new webServer.Server("http://10.1.30.36:8486/", this, pluginsLoader);
        }



        public void Start()
        {
            //-----------------
            //Вкл/Выкл ТелНет

            _agent.ShowInfo("запуск " + GetNameService() + " " + GetVersionService());
            //TelNetServer server = new TelNetServer(this, "root", "123");

            //загрузка стартовых команд
            string dir = Path.Combine(this.MainDir(), "configs");
            string file = Path.Combine(dir, "start.bat");
            // 1. проверка/создание папки
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            // 2. проверка/создание файла
            if (!File.Exists(file))
            {
                File.WriteAllText(file, ""); // или дефолтный текст
            }
            // 3. чтение
            string[] text = File.ReadAllText(file, Encoding.UTF8).Replace("\r", "").Split("\n");
            foreach (string line in text)
            {
                if (line.IndexOf("#") == 0) continue;
                this.SendCommand(webServerAgent, line);
            }
            


            //server.Start(_agent);
            while (true)
            {
                string? command = _agent.ReadLine();                        //получаем исходный вид комманды
                if(command != null)
                {
                    SendCommand(_agent, command);
                }
            }

        }

        public void SendCommand(ConsoleAgent agent, String command)
        {
            bool getCMD = true;                                         //проверка, нашлась ли подобная комманда. Если true, значит не нашлась
            if (command != null)
            {
                /*if (command.Equals("close"))
                {
                    this.Stop(agent);
                    this.ShowInfo("сервер выключен!");
                    Environment.Exit(0);
                    return;
                }*/
                string[] split = CommandRealisation.CommandToMatrArgs(command);
                string[] splitHelp = new string[0];
                if(split.Length > 0) splitHelp = new string[split.Length - 1];
                for(int i = 0; i <  split.Length - 1; i++) splitHelp[i] = split[i];
                /*string[] parms = new string[split.Length - 1];
                string name = split[0];
                for (int i = 0; i < parms.Length; i++)
                {
                    parms[i] = split[i + 1];
                }*/

                List<string> helpListCommand = new List<string>(); 
                foreach (CommandRealisation commandR in _commands)
                {                                                             //Список команд ответом на запрос о помощи
                    if (split.Length > 0) if (split[split.Length - 1].Equals("?") || split[split.Length - 1].Equals("help"))
                    {
                        string? line = commandR.GetDescription(splitHelp, helpListCommand);
                        if (line != null)
                        {
                            getCMD = false;
                        }
                        continue;
                    }
                    if (split.Length == 0)
                    {
                        if (split.Equals("?") || split.Equals("help"))
                        {
                            string? line = commandR.GetDescription(splitHelp, helpListCommand);
                            if (line != null)
                            {
                                getCMD = false;
                            }
                            continue;
                        }
                    }
                    if (commandR.Realization(agent, split))
                    {
                        getCMD = false;
                        return;
                    }
                }
                foreach (string line in helpListCommand)
                {
                    agent.ShowInfo(line);
                }
            }
            if (getCMD)
            {
                agent.ShowInfo("не удалось распознать команду!");
            }
        }

        /// <summary>
        /// Добавить эвент комманд
        /// </summary>
        /// <param name="Commands">обработчик событий</param>
        /*public void addCommandEvent(WebServerCommands Commands)
        {
            this.commandsEvents.Add(Commands);
        }*/
        /// <summary>
        /// Удалить эвент комманд
        /// </summary>
        /// <param name="Commands">обработчик событий который нужно удалить</param>
        /*public void removeCommandEvent(WebServerCommands Commands)
        {
            this.commandsEvents.Remove(Commands);
        }*/

        public void Stop(ConsoleAgent agent)
        {
            this.server.Stop(agent);
        }

        public ConsoleLog getLog()
        {
            return _log;
        }

        public void AddAgent(ConsoleAgent agent)
        {
            _agents.Add(agent);
        }

        public void RemoveAgent(ConsoleAgent agent)
        {
            _agents.Remove(agent);
        }

        public void ShowInfo(String info)
        {
            foreach (ConsoleAgent agent in _agents)
            {
                agent.ShowInfo(info);
            }
        }
        public void ShowError(String info)
        {
            foreach (ConsoleAgent agent in _agents)
            {
                agent.ShowError(info);
            }
        }
        public void ShowWarning(String info)
        {
            foreach (ConsoleAgent agent in _agents)
            {
                agent.ShowWarning(info);
            }
        }

        public string GetNameService()
        {
            return "Mika Web Server";
        }

        public string GetVersionService()
        {
            return "Beta v0.0.2";
        }

        public string MainDir()
        {
            string mainPatch = AppDomain.CurrentDomain.BaseDirectory;
            return mainPatch;
        }

        WebServerAgent MainDataServer.webServerAgent()
        {
            return webServerAgent;
        }

        public void AddCommand(CommandRealisation command)
        {
            _commands.Add(command);
        }

        public void RemoveCommand(CommandRealisation command)
        {
            _commands.Remove(command);
        }

        /// <summary>
        /// Возвращает объект отвечающий за модули
        /// </summary>
        /// <returns>Элемент с модулями</returns>
        internal PluginsLoader GetPluginLoader()
        {
            return this.pluginsLoader;
        }
    }
}
