using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace com.tok.mika.projects.mwebs.templetes
{
    internal class serverTemp
    {
        const String ADRESS = "http://localhost:8085/";
        public HttpListener listener;
        public bool status;
        public serverTemp()
        {
            this.listener = new HttpListener();
            this.listener.Prefixes.Add(ADRESS);
            this.status = true;
        }

        /// <summary>
        /// Запустить работу сервера
        /// </summary>
        public async void Start()
        {
            if (this.status)
            {
                try
                {
                    this.listener.Start();
                    this.process();
                    Console.WriteLine("Сервер готов к запуску!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ошибка:\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Тут творить магию по обработке запросов
        /// </summary>
        private void tick()
        {
            if (this.listener.IsListening)
            {
                try
                {
                    var context = this.listener.GetContext();
                    var response = context.Response;
                    byte[] buffer = null;
                    buffer = System.Text.Encoding.UTF8.GetBytes("hello world!");

                    response.ContentType = "text/html";
                    response.ContentLength64 = buffer.Length;
                    try
                    {
                        Stream output = response.OutputStream;
                        // отправляем данные
                        output.Write(buffer, 0, buffer.Length);
                        output.Flush();
                    }
                    catch (Exception exp)
                    {

                    }
                    response.Close();
                }
                catch (Exception ex)
                {
                }
            }
        }


        async private void process()
        {
            serverTemp server = this;
            await Task.Run(() =>
            {
                while (true)
                {
                    if (server != null)
                    {
                        if (server.status == false)
                        {
                            server.status = true;
                            Console.WriteLine("сервер остановлен");
                            return;
                        }
                        server.tick();
                    }
                    else
                    {
                        return;
                    }
                }
            });
        }

        /// <summary>
        /// Остановить работу сервера
        /// </summary>
        public void Stop()
        {
            this.listener.Stop();
            this.status = false;
        }
    }
}
