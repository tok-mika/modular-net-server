using com.tok.mika.libs.mws.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tok.mika.libs.mws.console
{
    public class CommandParameter
    {
        internal string name;
        internal string description;
        public CommandParameter(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
    }
    public class CommandRealisation
    {
        /// <summary>
        /// Представление команды
        /// </summary>
        protected string[] command;
        /// <summary>
        /// Описание действия команды
        /// </summary>
        protected string description;
        /// <summary>
        /// функция реализации команды
        /// </summary>
        protected Func<ConsoleAgent, string[], bool> realisation;
        /// <summary>
        /// Параметры/значения передаваемые коммандой
        /// </summary>
        protected CommandParameter[] parameters;
        public Module module1 { get; }
        
        public CommandRealisation(Module module, string command, Func<ConsoleAgent, string[], bool> realisation, string description, CommandParameter[] parameters)
        {
            this.command = command.Split(" ");
            this.realisation = realisation;
            this.description= description;
            this.parameters = parameters;
            this.module1 = module;
        }
        public CommandRealisation(Module module, string command, Func<ConsoleAgent, string[], bool> realisation, string description) : this(module, command, realisation, description, new CommandParameter[0]){}
        /// <summary>
        /// Полная длина комманды, колличество параметров
        /// </summary>
        /// <returns>Полная длина комманды, колличество параметров</returns>
        public int GetFullLength()
        {
            return command.Length + parameters.Length;
        }
        /// <summary>
        /// Реализация команды
        /// </summary>
        /// <param name="agent">Агент исполнителя команды</param>
        /// <param name="command">аргументы команды</param>
        /// <returns></returns>
        public bool Realization(ConsoleAgent agent, string[] command)
        {
            if(GetFullLength() > command.Length) return false;
            if (command.Length - this.command.Length != parameters.Length) return false;
            for (int i = 0; i < command.Length && i < this.command.Length; i++)
            {
                if (!command[i].Equals(this.command[i])) return false;
            }
            string[] args = new string[parameters.Length];
            int cmcount = this.command.Length;
            for(int i = 0; i < parameters.Length; i++)
            {
                args[i] = command[i + cmcount];
            }
            return this.realisation.Invoke(agent, args);
        }

        public string? GetDescription(string[] command, List<string> listCommands)
        {
            for (int i = 0; i < command.Length && i < this.command.Length; i++)
            {
                if (!command[i].Equals(this.command[i])) return null;
            }
            if (command.Length == this.command.Length - 1)
            {
                string result = this.command[this.command.Length - 1] + "\t\t--\t" + this.description;
                listCommands.Add(result);
                return result;
            }
            /*if(command.Length < this.command.Length)
            {
                string result = this.command[command.Length];
                listCommands.Add(result);
                return result;
            }*/
            if(this.command.Length <= command.Length && command.Length < this.GetFullLength())
            {
                int idPar = command.Length - this.command.Length;
                string result = "<" + this.parameters[idPar].name + ">\t\t--\t" + this.parameters[idPar].description;
                listCommands.Add(result);
                return result;
            }
            return null;
        }

        /// <summary>
        /// Разбивает строку на аргументы команды
        /// </summary>
        /// <param name="command">строковое представление команды</param>
        /// <returns>аргументы команды</returns>
        public static string[] CommandToMatrArgs(string command)
        {
            var result = new List<string>();
            var current = new StringBuilder();

            bool inQuotes = false;
            bool escape = false;

            foreach (char c in command)
            {
                if (escape)
                {
                    current.Append(c);
                    escape = false;
                    continue;
                }

                if (c == '\\')
                {
                    escape = true;
                    continue;
                }

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    if (current.Length > 0)
                    {
                        result.Add(current.ToString());
                        current.Clear();
                    }
                    continue;
                }

                current.Append(c);
            }

            if (current.Length > 0)
            {
                result.Add(current.ToString());
            }

            return result.ToArray();
        }
    }
}
