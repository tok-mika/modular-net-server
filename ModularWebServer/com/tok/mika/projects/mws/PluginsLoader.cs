using com.tok.mika.libs.mws;
using com.tok.mika.libs.mws.console;
using com.tok.mika.projects.mws.pluginLoader;
using System.Reflection;
using System.Reflection.Emit;

namespace com.tok.mika.projects.mws
{
    internal class PluginsLoader
    {
        private List<PluginLoadContext> modules;
        internal MainDataServer mainDataServer { get; }
        internal PluginsLoader(MainDataServer mainDataServer)
        {
            this.mainDataServer = mainDataServer;
            this.modules = new List<PluginLoadContext>();
        }

        internal void Load(ConsoleAgent agent)
        {
            string mainPatch = AppDomain.CurrentDomain.BaseDirectory + "modules";
            if (Directory.Exists(mainPatch))
            {
                string[] files = Directory.GetFiles(mainPatch, "*.dll");
                foreach (string file in files)
                {
                    Load(agent, file);
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
        /// Загрузить модуль из файла
        /// </summary>
        /// <param name="agent">агент совершивший команду</param>
        /// <param name="file">файл который необходимо загрузить</param>
        internal void Load(ConsoleAgent agent, string file)
        {
            try
            {
                var loadContext = new PluginLoadContext(file);
                var assembly = loadContext.LoadFromAssemblyPath(file);
                List<libs.mws.module.Module> loadModules = new List<libs.mws.module.Module>();
                foreach (var type in assembly.DefinedTypes)
                {
                    if (typeof(libs.mws.module.Module).IsAssignableFrom(type) && !type.IsInterface)
                    {
                        loadContext.module = (libs.mws.module.Module?)Activator.CreateInstance(type);

                        if (loadContext.module != null)
                        {
                            this.modules.Add(loadContext);
                            loadModules.Add(loadContext.module);
                            loadContext.level = loadContext.module.GetDefaultLavel();
                            //loadContext.module.onLoad(this.mainDataServer);
                            //agent.ShowInfo("модуль \"" + loadContext.module.getName() + "\" загружен");
                        }
                        else
                        {
                            agent.ShowInfo("не удалось загрузить модуль");
                        }
                    }
                }
                foreach (var module in loadModules) {
                    module.onLoad(this.mainDataServer);
                    agent.ShowInfo("модуль \"" + module.getName() + "\" загружен");
                }
                loadModules.Clear();
            }
            catch (Exception ex)
            {
                agent.ShowWarning("Модуль '" + file + "' не загружен, произошла ошибка при его загрузке!");
                agent.ShowError(ex.Message);
            }

            Sort();
            /*List<int> list = new List<int>();
            list.Add(0);
            list.Add(6);
            list.Add(3);
            list.Add(7);
            list.Add(1);
            list.Sort((x, y) => { if (x > y) return 1; else if(x == y) return 0; return -1; });
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }*/
        }

        /// <summary>
        /// Сортировка модулей по их уровню
        /// </summary>
        internal void Sort()
        {
            this.modules.Sort((x, y) => { if (x.level > y.level) return 1; if (x.level == y.level) return 0; return -1; });
        }

        /// <summary>
        /// Выгрузить все модули из системы
        /// </summary>
        internal void unLoadAll(ConsoleAgent agent)
        {
            for (int i = modules.Count - 1; i >= 0; i--)
            {
                var module = modules[i];
                string nameM = "";
                if (modules[i].module != null)
                {
                    nameM = modules[i].module.getName();
                    modules[i].module.unLoad();
                }
                modules.RemoveAt(i);
                module.Unload();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                module = null;
                agent.ShowInfo("модуль " + nameM + " выгружен");
            }
        }

        /// <summary>
        /// Выгрузить конкретный модуль
        /// </summary>
        /// <param name="agent">Агент</param>
        /// <param name="moduleName">короткое имя модуля</param>
        internal void unLoad(ConsoleAgent agent, string moduleName)
        {
            for (int i = modules.Count - 1; i >= 0; i--)
            {
                if (modules[i].module == null) continue;
                if (modules[i].module.getShortName().Equals(moduleName))
                {
                    var module = modules[i];
                    string nameM = "";
                    if (modules[i].module != null)
                    {
                        nameM = modules[i].module.getName();
                        modules[i].module.unLoad();
                    }
                    modules.RemoveAt(i);
                    module.Unload();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    module = null;
                    agent.ShowInfo("модуль " + nameM + " выгружен");
                }
            }
        }

        /// <summary>
        /// Установить приоритет загрузки модуля
        /// </summary>
        /// <param name="moduleName">короткое имя модуля</param>
        /// <param name="level">уровень загрузки, чем больше значение, тем выше приоритет, тем раньше будет выполняться функции</param>
        internal void SetLevelModule(ConsoleAgent agent, string moduleName, int level)
        {
            foreach(var module in modules)
            {
                if (module.module == null) continue;
                if (module.module.getShortName().Equals(moduleName))
                {
                    module.level = level;
                    agent.ShowInfo("Модулю '" + module.module.getName() + "' установлен уровень приоритета " + module.level.ToString());
                }
            }
            Sort();
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
                if(module.module != null) agent.ShowInfo(module.module.getShortName() + "\t--\t" + module.module.getName());
            }
        }

        /// <summary>
        /// Найти модуль по его короткому имени
        /// </summary>
        /// <param name="name">короткое имя модуля</param>
        /// <returns>модуль или null в случаи неудачи</returns>
        internal libs.mws.module.Module? GetModuleForName(string name)
        {
            foreach (PluginLoadContext module in modules)
            {
                if (module.module != null) if (module.module.getShortName().Equals(name)) return module.module;
            }
            return null;
        }

        /// <summary>
        /// Включить все запущенные модули
        /// </summary>
        internal void EnableAll()
        {
            foreach (var module in modules)
            {
                if(module.module != null)module.module.onEnable();
            }
        }

        /// <summary>
        /// Выключить все запущенные модули
        /// </summary>
        internal void DisableAll()
        {
            foreach(var module in modules)
            {
                if(module.module != null) module.module.onDisable();
            }
        }

        /// <summary>
        /// Обработка входящих запросов
        /// </summary>
        /// <param name="listener"></param>
        internal bool tick(HttpContext context)
        {
            foreach(var module in modules)
            {
                if(module.module != null) if(module.module.tick(context)) return true;
            }
            return false;
        }
    }
}
