using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WorkTrackingLib.Models;

namespace NewWorkTracking.Models
{
    class ConnectionClass
    {
        /// <summary>
        /// Основной объект подключения
        /// </summary>
        public static ConnectionPathInfo connectionPath;

        /// <summary>
        /// Строка подключения
        /// </summary>
        public static HubConnection hubConnection;   

        /// <summary>
        /// Метод создания строки подключения
        /// </summary>
        /// <returns></returns>
        public bool CreateConnectionString()
        {
            try
            {
                // Десериализация файла подключения
                connectionPath = JsonConvert.DeserializeObject<ConnectionPathInfo>(File.ReadAllText(@"Resources/serverInfo.json"));

                // Составление строки подключения
                hubConnection = new HubConnectionBuilder().WithUrl($"http://{connectionPath.Server}:5010/TrackingServer").Build();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
