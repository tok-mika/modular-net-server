using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.tok.mika.libs.mws.console;
using com.tok.mika.libs.mws.webServer;

namespace com.tok.mika.libs.mws
{
    public interface MainDataServer
    {
        /*/// <summary>
        /// Добавить эвент комманд
        /// </summary>
        /// <param name="Commands">обработчик событий</param>
        //public void addCommandEvent(WebServerCommands Commands);
        /// <summary>
        /// Удалить эвент комманд
        /// </summary>
        /// <param name="Commands">обработчик событий который нужно удалить</param>
        //public void removeCommandEvent(WebServerCommands Commands);*/
        
        /// <summary>
        /// Добавить комманду
        /// </summary>
        /// <param name="command"></param>
        public void AddCommand(CommandRealisation command);
        /// <summary>
        /// Удалить комманду
        /// </summary>
        /// <param name="command"></param>
        public void RemoveCommand(CommandRealisation command);
        /// <summary>
        /// Запуск сервера
        /// </summary>
        public void Start();
        /// <summary>
        /// Выключение сервера
        /// </summary>
        public void Stop(ConsoleAgent agent);
        /// <summary>
        /// Возвращает объект с логом
        /// </summary>
        /// <returns>ConsoleLog</returns>
        public ConsoleLog getLog();
        /// <summary>
        /// Добавить агента
        /// </summary>
        /// <param name="agent"></param>
        public void AddAgent(ConsoleAgent agent);
        /// <summary>
        /// Удалить агента
        /// </summary>
        /// <param name="agent"></param>
        public void RemoveAgent(ConsoleAgent agent);
        /// <summary>
        /// Вывод информации для всех агентов
        /// </summary>
        /// <param name="info"></param>
        public void ShowInfo(String info);
        /// <summary>
        /// Вывод ошибок для всех агентов
        /// </summary>
        /// <param name="info"></param>
        public void ShowError(String info);
        /// <summary>
        /// Вывод предупреждений для всех агентов
        /// </summary>
        /// <param name="info"></param>
        public void ShowWarning(String info);
        /// <summary>
        /// Выполнить команду на сервере через агента
        /// </summary>
        /// <param name="agent">агент</param>
        /// <param name="command">команда</param>
        public void SendCommand(ConsoleAgent agent, String command);
        /// <summary>
        /// Наименование програмы
        /// </summary>
        /// <returns></returns>
        public String GetNameService();
        /// <summary>
        /// Версия программы
        /// </summary>
        /// <returns></returns>
        public String GetVersionService();
        /// <summary>
        /// Корневой коталог приложения
        /// </summary>
        /// <returns>Путь к корневому коталогу приложения</returns>
        public String MainDir();
        public WebServerAgent webServerAgent();

    }
}
