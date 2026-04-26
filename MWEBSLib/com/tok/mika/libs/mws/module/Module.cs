using com.tok.mika.libs.mws;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace com.tok.mika.libs.mws.module
{
    public interface Module
    {
        /// <summary>
        /// Происходит после инициализации модуля
        /// </summary>
        /// <param name="mainData"></param>
        public void onLoad(MainDataServer mainData);
        /// <summary>
        /// Происходит после выгрузки модуля
        /// </summary>
        public void upLoad();
        /// <summary>
        /// Происходит при включении модуля
        /// </summary>
        public void onEnable();

        /// <summary>
        /// Происходит при отключении модуля
        /// </summary>
        public void onDisable();

        /// <summary>
        /// Обработчик запросов
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true - эсли запрос обработан, false если запрос не обработан</returns>
        public bool tick(HttpContext context);
        /// <summary>
        /// Имя модуля
        /// </summary>
        /// <returns></returns>
        public String getName();
        /// <summary>
        /// Короткое имя модуля
        /// </summary>
        /// <returns>название модуля без пробелов до 15 символов</returns>
        public String getShortName();
        /// <summary>
        /// Приоритет загрузки поумолчанию
        /// </summary>
        /// <returns>уровень приоритета загрузки</returns>
        public int GetDefaultLavel();

    }
}
