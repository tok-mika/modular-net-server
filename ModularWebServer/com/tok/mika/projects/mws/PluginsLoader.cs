using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using com.tok.mika.libs.mws;
using com.tok.mika.libs.mws.console;
using System.Net;

namespace com.tok.mika.projects.mws
{
    internal class PluginsLoader
    {
        private List<libs.mws.module.Module> modules;
        public MainDataServer mainDataServer { get; }
        public PluginsLoader(MainDataServer mainDataServer)
        {
            this.mainDataServer = mainDataServer;
            this.modules = new List<libs.mws.module.Module>();
        }

        public void Load(ConsoleAgent agent)
        {
            string mainPatch = AppDomain.CurrentDomain.BaseDirectory + "modules";
            if (Directory.Exists(mainPatch))
            {
                string[] files = Directory.GetFiles(mainPatch, "*.dll");
                foreach (string file in files)
                {
                    try
                    {
                        agent.ShowInfo("загрузка модуля: " + file);
                        Assembly assembly = Assembly.LoadFrom(file);
                        string repModule = "com.tok.mika.projects.mws.module";
                        foreach (Type type in assembly.GetTypes())
                        {
                            if (type.FullName != null)
                            {
                                string fullName = type.FullName.Substring(0, repModule.Length);
                                if (fullName.Equals(repModule))
                                {
                                    libs.mws.module.Module? module = Activator.CreateInstance(type) as libs.mws.module.Module;
                                    if (module != null)
                                    {
                                        this.modules.Add(module);
                                        module.onLoad(this.mainDataServer);
                                        agent.ShowInfo("модуль \"" + module.getName() + "\" загружен");
                                    }
                                    else
                                    {
                                        agent.ShowInfo("не удалось загрузить модуль");
                                    }
                                    break;
                                }
                            }

                        }
                        
                    }
                    catch(Exception ex)
                    {
                        agent.ShowWarning("Модуль не загружен, произошла ошибка при его загрузке!");
                        agent.ShowError(ex.Message);
                    }
                    
                }
            }
            else
            {
                agent.ShowWarning("Не найдена дериктория с модулями!");
            }


            //Assembly assembly = Assembly.LoadFrom("");
            //Directory.EnumerateFiles()

        }

        /// <summary>
        /// Выгрузить все модули из системы
        /// </summary>
        internal void upLoadAll()
        {
            foreach (libs.mws.module.Module module in modules)
            {
                module.upLoad();
                modules.Remove(module);
            }
        }

        /// <summary>
        /// Вывести список модулей агенту
        /// </summary>
        /// <param name="agent">Агент которому выведится информация о модулях</param>
        internal void ShowModules(ConsoleAgent agent)
        {
            agent.ShowInfo("-----модули-----");
            foreach (var module in modules)
            {
                agent.ShowInfo(module.getShortName() + "\t--\t" + module.getName());
            }
        }

        /// <summary>
        /// Найти модуль по его короткому имени
        /// </summary>
        /// <param name="name">короткое имя модуля</param>
        /// <returns>модуль или null в случаи неудачи</returns>
        public libs.mws.module.Module? GetModuleForName(string name)
        {
            foreach (libs.mws.module.Module module in modules)
            {
                if (module.getShortName().Equals(name)) return module;
            }
            return null;
        }

        /// <summary>
        /// Включить все запущенные модули
        /// </summary>
        public void EnableAll()
        {
            foreach (var module in modules)
            {
                module.onEnable();
            }
        }

        /// <summary>
        /// Выключить все запущенные модули
        /// </summary>
        public void DisableAll()
        {
            foreach(var module in modules)
            {
                module.onDisable();
            }
        }

        /// <summary>
        /// Обработка входящих запросов
        /// </summary>
        /// <param name="listener"></param>
        public bool tick(HttpContext context)
        {
            foreach(var module in modules)
            {
                if(module.tick(context)) return true;
            }
            return false;
        }
    }
}
