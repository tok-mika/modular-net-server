using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using com.tok.mika.libs.mws;
using com.tok.mika.libs.mws.console;
using com.tok.mika.libs.mws.module;

namespace com.tok.mika.projects.mws.webServer
{
    internal class Server : Module
    {
        /// <summary>
        /// Статус режима разработчика
        /// </summary>
        internal bool devStatus;
        public MainData main { get; }
        //public HttpListener listener;
        private WebApplication? _webApp;
        private string _url;
        public bool status { get; private set; }
        private PluginsLoader _pluginsLoader;
        private ConsoleAgent _agent;
        private Commands _commands;

        /// <summary>
        /// Веб сервер
        /// </summary>
        /// <param name="address">Адрес который слушает сервер</param>
        public Server(ConsoleAgent agent, string address, MainData main, PluginsLoader pluginsLoader)
        {
            _url = address;
            this.status = false;
            this.main = main;
            //this.main.addCommandEvent(new Commands(this));
            _commands = new Commands(this);
            _pluginsLoader = pluginsLoader;
            _agent = agent;
            _webApp = null;
            devStatus = false;
        }

        /// <summary>
        /// Запустить работу сервера
        /// </summary>
        public void Start(ConsoleAgent agent)
        {
            _agent = agent;
            if (this.status)
            {
                _agent.ShowWarning("Web сервер уже запущен! Для повторного выполнения команды - остановите работу сервера!");
                return;
            }
            try
            {

                var builder = WebApplication.CreateBuilder();
                builder.WebHost.UseUrls(_url);
                /*builder.WebHost.ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(505, listenOptions =>
                    {
                        listenOptions.UseHttps("cert.pfx", "password"); // путь к сертификату и пароль
                    });
                });*/
                builder.Logging.ClearProviders();
                if (devStatus) builder.Services.AddCors(options =>
                {
                    options.AddPolicy("dev", policy =>
                    {
                        policy
                            .WithOrigins("http://localhost:5173") // твой dev сайт
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
                });
                _webApp = builder.Build();
                _webApp.Map("{*path}", async context =>
                {
                    this.tickM(context);
                });
                _webApp.RunAsync();
                _agent.ShowInfo("Web сервер запущен!");
                this.status = true;
                _pluginsLoader.EnableAll();
            }
            catch (Exception e)
            {
                _agent.ShowError(e.Message);
            }
        }

        /// <summary>
        /// Тут творить магию по обработке запросов
        /// </summary>
        private void tickM(HttpContext context)
        {
            try
            {
                //Console.WriteLine("входящий запрос - " + $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
                if (this._pluginsLoader.tick(context)) return;
            }
            catch (Exception exp)
            {
                _agent.ShowError($"Error for tick: {exp.Message}");
            }


            var response = context.Response;
            byte[]? buffer = null;
            WebFiles webFiles = new WebFiles(this);
            if (context.Request.Path != null)
            {
                int indexKey = context.Request.Path.ToString().LastIndexOf('.');
                string type = "txt";
                if (indexKey != -1)
                {
                    type = context.Request.Path.ToString().Substring(indexKey + 1);
                    buffer = webFiles.getFile(_agent, context.Request.Path.ToString());
                }
                else
                {
                    buffer = webFiles.getFile(_agent, context.Request.Path.ToString() + "/index.html");
                    type = "html";
                }

                if (buffer != null)
                {
                    if (type.Equals("txt")) response.ContentType = "text/plain";
                    if (type.Equals("html")) response.ContentType = "text/html";
                    if (type.Equals("htm")) response.ContentType = "text/html";
                    if (type.Equals("css")) response.ContentType = "text/css";
                    if (type.Equals("js")) response.ContentType = "text/javascript";

                    if (type.Equals("jpg")) response.ContentType = "image/jpeg";
                    if (type.Equals("jpeg")) response.ContentType = "image/jpeg";
                    if (type.Equals("png")) response.ContentType = "image/png";
                    if (type.Equals("gif")) response.ContentType = "image/gif";
                    if (type.Equals("ico")) response.ContentType = "image/x-icon";
                    response.ContentLength = buffer.Length;
                    response.StatusCode = 200;
                    try
                    {
                        context.Response.Body.WriteAsync(buffer);
                    }
                    catch (Exception exp)
                    {
                        main.ShowError($"Error: {exp.Message}");
                    }
                    return;
                }
            }
            buffer = System.Text.Encoding.UTF8.GetBytes("error 404");

            response.ContentType = "text/html";
            response.ContentLength = buffer.Length;
            response.StatusCode = 404;
            try
            {
                context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
                return;
            }
            catch (Exception exp)
            {
                _agent.ShowError($"Error: {exp.Message}");
            }
            return;
        }

        /// <summary>
        /// Остановить работу сервера
        /// </summary>
        public void Stop(ConsoleAgent agent)
        {
            _agent = agent;
            _pluginsLoader.DisableAll();
            if (_webApp != null)
            {
                _webApp.StopAsync();
                _webApp = null;
            }
            this.status = false;
            _agent.ShowInfo("Web сервер остановлен!");
        }

        public override void onLoad(MainDataServer mainData)
        {
            throw new NotImplementedException();
        }

        public override void unLoad()
        {
            throw new NotImplementedException();
        }

        public override void onEnable()
        {
            throw new NotImplementedException();
        }

        public override void onDisable()
        {
            throw new NotImplementedException();
        }

        public override bool tick(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public override string getName()
        {
            throw new NotImplementedException();
        }

        public override string getShortName()
        {
            throw new NotImplementedException();
        }

        public override int GetDefaultLavel()
        {
            throw new NotImplementedException();
        }
    }
}
