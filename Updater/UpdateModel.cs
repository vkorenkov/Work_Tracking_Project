using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WorkTrackingLib.Models;

namespace Updater
{
    public delegate void MessageDelegate(string msg);
    public delegate void ProgDelegate(int a, int b);

    class UpdateModel
    {
        #region Поля 

        /// <summary>
        /// Поле имени программы
        /// </summary>
        public static string ProgName = "Work_Tracking.exe";
        /// <summary>
        /// Название программы для обновления
        /// </summary>
        public static string UpdatePackage = "WorkTrackingUpdate.zip";
        /// <summary>
        /// Событие сообщений окна обновления
        /// </summary>
        public static event MessageDelegate MesEvent;
        /// <summary>
        /// Событиеи остановки прогресс-бара
        /// </summary>
        public static event MessageDelegate StopProgress;

        static string path = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;

        #endregion

        #region Методы

        /// <summary>
        /// Метод обновления PassYourWork
        /// </summary>
        /// <returns></returns>
        public bool UpdateProgramm()
        {
            // Вызов события сообщения
            MesEvent("Обновление");

            try
            {
                if (DownloadFile())
                {
                    if (File.Exists($@"{path}\{UpdatePackage}"))
                    {
                        var tempFiles = Directory.GetFiles(path);

                        try
                        {
                            if (Directory.Exists($@"{path}\dll"))
                            {
                                Directory.Delete($@"{path}\dll", true);
                            }
                        }
                        catch
                        { }

                        foreach (var t in tempFiles)
                        {
                            if (t.Contains("WorkTrackingUpdate.zip"))
                                continue;
                            else
                            {
                                var tempProcess = GetActualProcess();

                                if (tempProcess != null)
                                {
                                    KillProcess(tempProcess);
                                }

                                File.Delete(t);
                            }
                        }

                        ZipFile.ExtractToDirectory($@"{path}\{UpdatePackage}", $@"{path}");
                    }

                    // Получение новой версии программы для отображения
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo($@"{ProgName}");

                    File.Delete($@"{path}\{UpdatePackage}");

                    StopProgress?.Invoke($@"Обновление завершено. Новая версия программы {fileVersionInfo.FileVersion}");
                }
                else
                {
                    StopProgress?.Invoke("Ошибка скачивания");
                }
               
                return true;
            }
            catch (Exception ex)
            {
                StopProgress?.Invoke($"Ошибка обновления: {ex.InnerException.Message} Программа будет перезапущена через 5 сек.");

                File.Delete($@"{path}\{UpdatePackage}");

                return false;
            }
        }

        private bool DownloadFile()
        {
            // Инициализация экземпляра класса Веб-клиента для скачивания обновления
            WebClient webClient = new WebClient();

            try
            {
                var updateServer = JsonConvert.DeserializeObject<ConnectionPathInfo>(File.ReadAllText($@"{path}/Resources/serverInfo.json"));

                // Скачивание новой версии программы
                webClient.DownloadFile(new Uri($@"http://{updateServer.UpdateServer}/Update/{UpdatePackage}"), $@"{path}\{UpdatePackage}");

                return true;
            }
            catch
            {
                return false;
            }
        }

        private Process GetActualProcess()
        {
            Process[] proc = Process.GetProcesses();

            Process temp = null;

            foreach (var t in proc)
            {
                if (t.ProcessName.Contains("Work Tracking"))
                {
                    temp = t;
                    break;
                }
                else
                {
                    temp = null;
                }
            }

            return temp;
        }

        /// <summary>
        /// Метод отмены текущего действия
        /// </summary>
        /// <param name="result"></param>
        private void KillProcess(Process killedProcess)
        {
            killedProcess.Kill();
            killedProcess.WaitForExit();
        }

        #endregion
    }
}
