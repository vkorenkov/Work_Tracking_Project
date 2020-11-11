using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using NewWorkTracking.Models;
using NewWorkTracking.UpdateApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WorkTrackingLib;
using WorkTrackingLib.Models;
using WorkTrackingLib.ProcessClasses;

namespace NewWorkTracking.ViewModels
{
    class ProgressBarViewModel : PropertyChangeClass
    {
        /// <summary>
        /// Поле объекта проверки обновлений
        /// </summary>
        GetNewVersionApi getNewVersionApi;
        /// <summary>
        /// Поле подключения к серверу
        /// </summary>
        ConnectionClass connection;

        private string status;
        /// <summary>
        /// Свойство вывода текущего действия на экран
        /// </summary>
        public string Status
        {
            get => status;
            set { status = value; OnPropertyChanged(nameof(Status)); }
        }

        private string newServer;
        /// <summary>
        ///  Поле нового сервера
        /// </summary>
        public string NewServer
        {
            get => newServer;
            set { newServer = value; OnPropertyChanged(nameof(NewServer)); }
        }

        private bool controlEnable;
        /// <summary>
        /// Поле активации\деактивации управления
        /// </summary>
        public bool ControlEnable
        {
            get { return controlEnable; }
            set { controlEnable = value; OnPropertyChanged(nameof(ControlEnable)); }
        }

        private Visibility buttonsVis;
        /// <summary>
        /// Свойство видимости кнопок управления
        /// </summary>
        public Visibility ButtonsVis
        {
            get => buttonsVis;
            set { buttonsVis = value; OnPropertyChanged(nameof(ButtonsVis)); }
        }

        private Visibility changeTextBoxVis;
        /// <summary>
        /// Свойство видимости поля ввода нового сервера
        /// </summary>
        public Visibility ChangeTextBoxVis
        {
            get => changeTextBoxVis;
            set { changeTextBoxVis = value; OnPropertyChanged(nameof(ChangeTextBoxVis)); }
        }

        public ICommand ChouseMenu => new RelayCommand<object>(obj =>
        {
            switch (obj)
            {
                case "Yes":
                    ChangeTextBoxVis = Visibility.Visible;
                    break;
                case "No":
                    Application.Current.Shutdown();
                    break;
                case "Repeat":
                    ButtonsVis = Visibility.Collapsed;
                    ChangeTextBoxVis = Visibility.Collapsed;
                    Task.Run(() => StartApp());
                    break;
            }
        });

        /// <summary>
        /// Команда изменения сервера
        /// </summary>
        public ICommand ChangeServer
        {
            get
            {
                return new RelayCommand<object>(async obg =>
                {
                    Status = "Проверка введенных данных. Подождите.";

                    ControlEnable = false;

                    if (await newServer.WriteNewServer())
                    {
                        WindowUsage.RestartProgramm("ProgrBar");
                    }
                    else
                    {
                        Status = $"Ошибка подключения к новому серверу. Проверьте введенные данные, сервер не будет изменен. Введенный вами сервер {NewServer}. Текущий сервер: {ConnectionClass.connectionPath.Server}";

                        ControlEnable = true;
                    }
                });
            }
        }

        public ProgressBarViewModel()
        {
            connection = new ConnectionClass();
            ButtonsVis = Visibility.Collapsed;
            ChangeTextBoxVis = Visibility.Collapsed;
            ControlEnable = true;
            getNewVersionApi = new GetNewVersionApi();

            Task.Run(() =>
            {
                Status = "Проверка обновлений.";
                if (connection.CreateConnectionString())
                {
                    StartCheckUpdate();
                }
                else
                {
                    OutputCantConnection();
                }
            });
        }

        private async Task<bool> StartConnection()
        {
            try
            {
                Status = "Проверка подключения.";

                await ConnectionClass.hubConnection.StartAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void StartApp()
        {
            if (StartConnection().Result)
            {
                StartCheckAccess();
            }
            else
            {
                OutputCantConnection();
            }
        }

        private void StartCheckAccess()
        {
            Status = "Подключение установлено.";

            ConnectionClass.hubConnection.InvokeAsync("RunCheckAccess", UserPrincipal.Current.DisplayName);

            ConnectionClass.hubConnection.On<bool>("AccessDenide", (access) => Status = "Доступ запрещен. Обратитесь к руководителю или в отдел технической поддержки");

            Status = "Получение данных.";

            ConnectionClass.hubConnection.On<MainObject>("GiveAll", (mainObject) =>
            {
                mainObject.ComboBox.Accesses = mainObject.ComboBox.Accesses.OrderBy(x => x.Name).ToList();

                Status = "Запуск программы.";

                Application.Current.Dispatcher.Invoke(() => StartMainWindow(mainObject));
            });
        }

        private void OutputCantConnection()
        {
            if (File.Exists(@"Resources/serverInfo.json"))
            {
                Status = $@"Подключение отсутствует. Проверьте сетевое подключение и адрес сервера. Повторить попытку подключения или изменить сервер? Текущий сервер: ""{ConnectionClass.connectionPath.Server}""";

                ButtonsVis = Visibility.Visible;
            }
            else
            {
                if (CreateConnectionFile())
                {
                    Status = "Отсутствует файл подключения к серверу. Создан стандартный файл подключения. Перезапустите программу и укажите корректный сервер.";
                }
                else
                {
                    Status = "Отсутствует файл подключения к серверу. Создать стандартный файл подключения не удалось. Переустановите программу.";
                }
            }
        }

        private void StartCheckUpdate()
        {
            if (!getNewVersionApi.CheckUpdate(Convert.ToInt32(FileVersionInfo.GetVersionInfo($@"{Directory.GetCurrentDirectory()}/Work_Tracking.exe").FileVersion.Replace(".", ""))))
                StartApp();
            else
            {
                if (Application.Current.Dispatcher.Invoke(() => Message.Show("Внимание", "Обноружено обновление. Установить?", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                    StartUpdate();
                else
                    StartApp();
            }
        }

        private void StartMainWindow(MainObject mainObject)
        {
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(mainObject);

            MainWindow mainWindow = new MainWindow()
            {
                DataContext = mainWindowViewModel
            };

            mainWindow.Show();

            WindowUsage.CloseWindow("ProgrBar");
        }

        private void StartUpdate()
        {
            if (File.Exists(@"Updater\Updater.exe"))
            {
                Process.Start(@"Updater\Updater.exe");

                // Закрытие основного приложения
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                Message.Show("Ошибка", "Отсутствует модуль обновления. Рекомендуется переустановка программы", MessageBoxButton.OK);

                StartApp();
            }
        }

        private bool CreateConnectionFile()
        {
            try
            {
                if (Directory.Exists("Resources"))
                {
                    ConnectionPathInfo connectionPath = new ConnectionPathInfo() { Server = "#", UpdateServer = "" };

                    File.WriteAllText(@"Resources/serverInfo.json", JsonConvert.SerializeObject(connectionPath));

                    return true;
                }
                else
                {
                    Directory.CreateDirectory("Resources");

                    CreateConnectionFile();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
