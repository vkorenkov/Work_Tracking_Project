using Newtonsoft.Json;
using NewWorkTracking.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WorkTrackingLib.Interfaces;
using WorkTrackingLib.Models;

namespace NewWorkTracking.UpdateApi
{
    /// <summary>
    /// Класс проверки обновлений программы
    /// </summary>
    public class GetNewVersionApi : INewVersionApi
    {
        private HttpClient client;

        public GetNewVersionApi()
        {
            client = new HttpClient();
        }

        /// <summary>
        /// Метод проверки обновлений программы
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool CheckUpdate(int version)
        {
            try
            {
                // Формирование строки подключения к серверу обновлений
                string url = $@"http://{ConnectionClass.connectionPath.UpdateServer}/api/Update/Check";

                // Получение версии программы с сервера обновлений
                var updateVersion = JsonConvert.DeserializeObject<ProgramInfo>(client.GetStringAsync(url).Result);

                // Сравнение версий
                if (updateVersion != null && updateVersion.Version > version)
                {
                    return true;
                }
                // Действие при ошибке получения данных из файла версии
                else if (updateVersion == null)
                {
                    Application.Current.Dispatcher.Invoke(() => Message.Show("Внимание", "Не удалось получить файл версии", MessageBoxButton.OK));

                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                Application.Current.Dispatcher.Invoke(() => Message.Show("Внимание", "Нет подключения к серверу обновлений", MessageBoxButton.OK));

                return false;
            }
        }
    }
}
