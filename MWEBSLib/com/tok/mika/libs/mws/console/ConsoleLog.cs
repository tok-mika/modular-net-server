using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace com.tok.mika.libs.mws.console
{
    public class ConsoleLog
    {
        public MainDataServer main { get; }
        String _log;
        private string _mainPatch;
        private string _logFileName;
        private StreamWriter? _file = null;
        public ConsoleLog(MainDataServer mainData) 
        {

            _mainPatch = AppDomain.CurrentDomain.BaseDirectory + "log";
            _logFileName = DateTime.Now.ToString("HH.mm dd.MM.yyyy") + ".log";
            main = mainData;
            _log = "";
        }

        /*public void Log(string message)
        {
            _log += message;
        }*/

        public void AddLine(string line)
        {
            if(_file != null)
            {
                if(_log.Length > 1000000)
                {
                    _log = "";
                    _file.Close();
                    _logFileName = DateTime.Now.ToString("HH.mm dd.MM.yyyy") + ".log";
                    _file = null;
                }
            }
            if(_file == null)
            {
                try
                {
                    // 1. Проверяем/создаём директорию
                    string? dir = Path.GetDirectoryName(_mainPatch + "/" + _logFileName);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    // 2. Открываем файл (создаст если нет)
                    _file = new StreamWriter(_mainPatch + "/" + _logFileName, true, Encoding.UTF8);
                }
                catch
                {
                    Console.WriteLine("не удается открыть/создать лог в файл!");
                    _file = null;
                    return;
                }
            }
            _file.WriteLine(line);
            _file.Flush();
            _log += line;
        }

        public bool getLog(String log)
        {
            return _log == log;
        }

        public void show(ConsoleAgent agent)
        {
            //Console.WriteLine(_log);
            agent.ShowInfo(_log);
        }
    }
}
