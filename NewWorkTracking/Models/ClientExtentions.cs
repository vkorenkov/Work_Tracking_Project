using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WorkTrackingLib.Models;

namespace NewWorkTracking.Models
{
    static class ClientExtentions
    {
        static HubConnection hubConnection;

        /// <summary>
        /// (Расширение) Метод записывает данные по новому серверу в файл адреса сервера
        /// </summary>
        /// <param name="newServer"></param>
        public async static Task<bool> WriteNewServer(this string newServer)
        {
            hubConnection = CreateTempConnectionString(newServer);

            if (hubConnection != null)
            {
                // Проверка соединения с новым сервером
                if (await hubConnection.CheckNewServer())
                {
                    try
                    {
                        // Десериализация json файла подключения
                        var tempReadedFile = JsonConvert.DeserializeObject<ConnectionPathInfo>(File.ReadAllText(@"Resources/serverInfo.json"));

                        // Временная переменная объекта подключения
                        tempReadedFile.Server = newServer;

                        // Запись временного объекта в файл Json
                        File.WriteAllText(@"Resources/serverInfo.json", JsonConvert.SerializeObject(tempReadedFile));

                        // Десериализация актуального файла подключения в основной объект подключения
                        ConnectionClass.connectionPath = JsonConvert.DeserializeObject<ConnectionPathInfo>(File.ReadAllText(@"Resources/serverInfo.json"));

                        await hubConnection.DisposeAsync();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Message.Show("Ошибка", ex.Message, MessageBoxButton.OK);

                        return false;
                    }
                }
                else
                    return false;
            }
            else
                return false;

        }

        private static HubConnection CreateTempConnectionString(string newServer)
        {
            try
            {
                // Создание тестовой строки подключения
                return hubConnection = new HubConnectionBuilder().WithUrl($"http://{newServer = newServer.Replace(",", ".")}:5010/TrackingServer").Build();
            }
            catch (Exception ex)
            {
                Message.Show("Ошибка", ex.Message, MessageBoxButton.OK);

                return null;
            }
        }

        /// <summary>
        /// (Расширение) Метод проверки корректности введенных данных
        /// </summary>
        /// <param name="testCon"></param>
        /// <returns></returns>
        public async static Task<bool> CheckNewServer(this HubConnection testCon)
        {
            try
            {
                await testCon.StartAsync();

                await testCon.InvokeAsync("TestConnection");

                await testCon.StopAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// (Расширение) Метод определяет открыто ли окно с определенным именем в данный момент
        /// </summary>
        /// <param name="windows"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool ContainsWindow(this WindowCollection windows, string name)
        {
            bool temp = false;

            foreach (Window t in windows)
            {
                if (t.Name == name)
                {
                    temp = true;
                }
            }

            return temp;
        }

        /// <summary>
        /// (Расширение) Метод закрытия окна
        /// </summary>
        /// <param name="windows"></param>
        /// <param name="name"></param>
        public static void CloseWindow(this WindowCollection windows, string name)
        {
            foreach (Window t in windows)
            {
                if (t.Name == name)
                {
                    t.Close();
                }
            }
        }
    }
}
