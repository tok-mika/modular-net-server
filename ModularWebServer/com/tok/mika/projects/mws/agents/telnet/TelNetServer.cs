using com.tok.mika.libs.mws;
using com.tok.mika.libs.mws.console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace com.tok.mika.projects.mws.agents.telnet
{
    internal class TelNetServer
    {
        private MainDataServer main;
        private string _login;
        private string _password;
        public TelNetServer(MainDataServer main, string login, string password)
        {
            this.main = main;
            this._login = login;
            this._password = password;
        }

        public void Start(ConsoleAgent agent)
        {
            int port = 23; // стандартный telnet порт (можно взять 2323 без прав администратора)
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            agent.ShowInfo($"Telnet сервер запущен на порту {port}...");
            new Thread(() => {
                while (true)
                {
                    if (main == null) return;
                    TcpClient client = server.AcceptTcpClient();
                    agent.ShowInfo("Подключился клиент TelNet");
                    new Thread(() => HandleClient(agent, client)).Start();
                }
            }).Start();
        }

        private void HandleClient(ConsoleAgent agent, TcpClient client)
        {

            var RemoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            string ipClient = "";
            if(RemoteEndPoint != null)ipClient = RemoteEndPoint.Address.ToString();
            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                TelNetAgent agentTel = new TelNetAgent(main, "telnet-" + ipClient, stream, _login + "#");

                try
                {
                    string message = " ";
                    byte[] data = new byte[12];
                    int indxB = 0;
                    data[indxB * 3 + 0] = 0xFF; data[indxB * 3 + 1] = 0xFB; data[indxB * 3 + 2] = 0x01; 
                    indxB = 1;
                    data[indxB * 3 + 0] = 0xFF; data[indxB * 3 + 1] = 0xFB; data[indxB * 3 + 2] = 0x03;
                    indxB = 2;
                    data[indxB * 3 + 0] = 0xFF; data[indxB * 3 + 1] = 0xFD; data[indxB * 3 + 2] = 24;
                    indxB = 3;
                    data[indxB * 3 + 0] = 0xFF; data[indxB * 3 + 1] = 0xFD; data[indxB * 3 + 2] = 31;
                    stream.Write(data, 0, data.Length);


                    message = "\r\n\r\n\r\n\t\t\t" + main.GetNameService() + " " + main.GetVersionService() + "\r\n\n\n\nlogin:";
                    data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);

                    string login = ReadStrFromStream("\rlogin:", stream);
                    login = login.Replace("\n", "");
                    login = login.Replace("\r", "");
                    login = login.Replace("\0", "");
                    message = "\r\npassword:";
                    data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    string password = ReadStrFromStream("\rpassword:", stream, true);
                    password = password.Replace("\n", "");
                    password = password.Replace("\r", "");
                    password = password.Replace("\0", "");
                    if (login.Equals(_login) && password.Equals(_password))
                    {

                        agent.ShowInfo("Клиент TelNet " + ipClient + " успешно авторизовался.");
                        main.AddAgent(agentTel);
                        while (agentTel.Connect())
                        {
                            main.SendCommand(agentTel, agentTel.ReadLine().Replace("\n", "").Replace("\r", ""));
                        }
                    }
                }
                catch
                {

                }
                main.RemoveAgent(agent);
            }
           
            agent.ShowInfo("Клиент TelNet " + ipClient + " отключился.");
        }

        public static string ReadStrFromStream(string presting, NetworkStream stream, bool password = false)
        {
            string result = "";
            byte[] data1 = new byte[1024];
            try
            {
                int length = 0;
                while (true)
                {
                    int p = stream.Read(data1, 0, data1.Length);
                    if (data1.Length > 0)
                    {
                        if (data1[0] == 0xFF)
                        {
                            /*for(int i = 0; i < p; i++)
                            {
                                int b = (int)data1[i];
                                Console.WriteLine(b.ToString("X"));
                            }*/
                            continue;
                        }
                    }
                    result += Encoding.UTF8.GetString(data1, 0, p);
                    result = result.Replace("\r\0", "\n");
                    if (result[result.Length - 1] == '\n') { break; }


                    for(int i = result.IndexOf('\b'); i < result.Length && i >= 0; i = result.IndexOf('\b', i + 1))
                    {
                        if(i > 0)
                        {
                            if (i == 1)
                            {
                                if (i > result.Length - 1)
                                    result = result.Substring(i + 1);
                                else
                                {
                                    result = "";
                                    break;
                                }
                                i = 0;
                            }
                            else if (i >= result.Length - 1)
                            {
                                result = result.Substring(0, i - 1);
                                i -= 2;
                            }
                            else
                            {
                                result = result.Substring(0, i - 1) + result.Substring(i + 1);
                                i -= 2;
                            }
                        }
                        else
                        {
                            if(result.Length > 1)
                            result = result.Substring(1);
                            else
                            {
                                result = "";
                                break;
                            }
                            i = 0;
                        }
                    }

                    /*if(firstClear)
                    {
                        firstClear = false;
                        result = "";
                    }*/
                    string resultTo = result;
                    if (password && result.Length > 0) resultTo = new string('*', result.Length);

                    string message = "\r" + new string(' ', length) + "\r" + presting + resultTo;
                    length = (presting + result).Length;
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
