using Microsoft.AspNetCore.SignalR.Client;
using NewWorkTracking.Models;
using NewWorkTracking.Windows;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WorkTrackingLib;
using WorkTrackingLib.Models;
using WorkTrackingLib.ProcessClasses;

namespace NewWorkTracking.ViewModels
{
    class MainWindowViewModel : AbstractViewModel
    {
        private UserWorksViewModel userWorksViewModel;
        /// <summary>
        /// Свойство контекста данных вкладки работ пользователя
        /// </summary>
        public UserWorksViewModel UserWorksViewModel
        {
            get => userWorksViewModel;
            set { userWorksViewModel = value; OnPropertyChanged(nameof(UserWorksViewModel)); }
        }

        private AllWorksViewModel allWorksViewModel;
        /// <summary>
        /// Свойство контекста данных вкладки всех работ
        /// </summary>
        public AllWorksViewModel AllWorksViewModel
        {
            get => allWorksViewModel;
            set { allWorksViewModel = value; OnPropertyChanged(nameof(AllWorksViewModel)); }
        }

        private RepairsViewModel repairsViewModel;
        /// <summary>
        /// Свойство контекста данных вкладки всех ремонтов
        /// </summary>
        public RepairsViewModel RepairsViewModel
        {
            get => repairsViewModel;
            set { repairsViewModel = value; OnPropertyChanged(nameof(RepairsViewModel)); }
        }

        private DevicesViewModel devicesViewModel;
        /// <summary>
        /// Свойство контекста данных вкладки устройств
        /// </summary>
        public DevicesViewModel DevicesViewModel
        {
            get => devicesViewModel;
            set { devicesViewModel = value; OnPropertyChanged(nameof(DevicesViewModel)); }
        }

        private AdministrateViewModel administrateViewModel;
        /// <summary>
        /// Свойство контекста данных вкладки администрирования
        /// </summary>
        public AdministrateViewModel AdministrateViewModel
        {
            get => administrateViewModel;
            set { administrateViewModel = value; OnPropertyChanged(nameof(AdministrateViewModel)); }
        }

        private string connectStatus;
        /// <summary>
        /// Свойство представления статуса подключения к серверу
        /// </summary>
        public string ConnectStatus
        {
            get { return connectStatus; }
            set { connectStatus = value; OnPropertyChanged(nameof(ConnectStatus)); }
        }
       
        private bool stat;
        /// <summary>
        /// Свойство статуса подключения к серверу
        /// </summary>
        public bool Stat
        {
            get { return stat; }
            set { stat = value; OnPropertyChanged(nameof(Stat)); }
        }

        /// <summary>
        /// Команда изменения отображения вкладок
        /// </summary>
        public ICommand ViewModelsVisibility => new RelayCommand<object>(obj =>
        {
            SwitchTab(obj);           
        });

        /// <summary>
        /// Команда кнопки назад (На данный момент не используется)
        /// </summary>
        public ICommand Back => new RelayCommand<object>(obj =>
        {
            if (AllWorksViewModel.Actively == typeof(WorkCardDataContext).ToString())
            {
                AllWorksViewModel.Actively = string.Empty;
                AllWorksViewModel.ActiveVisibility = Visibility.Visible;
            }
            if(RepairsViewModel.Actively == typeof(RepairCardDataContext).ToString())
            {
                RepairsViewModel.Actively = string.Empty;
                RepairsViewModel.RepairContext.RepairCardVis = Visibility.Collapsed;
                RepairsViewModel.ActiveVisibility = Visibility.Visible;
            }
        });

        /// <summary>
        /// Команда запуска окна смены сервера
        /// </summary>
        public ICommand ChangeServer => new RelayCommand<object>(obj => 
        {
            new ChangeServerWindow().ShowDialog();
        });

        public MainWindowViewModel(MainObject mainObject)
        {
            SignalRActions();

            MainObject.Access = mainObject.Access;

            UserWorksViewModel = new UserWorksViewModel(mainObject);

            AllWorksViewModel = new AllWorksViewModel(mainObject);

            RepairsViewModel = new RepairsViewModel(mainObject);

            DevicesViewModel = new DevicesViewModel(mainObject);

            AdministrateViewModel = new AdministrateViewModel(mainObject);

            ConnectionClass.hubConnection.Closed += Reconnect;

            ConnectStatus = $@"Установлено соединение с сервером ""{ConnectionClass.connectionPath.Server}"".";

            Stat = true;
        }

        public MainWindowViewModel()
        {
        }

        /// <summary>
        /// Метод подписки на сообщения сервера
        /// </summary>
        protected override void SignalRActions()
        {
            ConnectionClass.hubConnection.On<AccessModel>("ChangeAccess", (accessModel) =>
            {
                if (accessModel.Name == UserPrincipal.Current.DisplayName)
                {
                    string tempAccessDesc = string.Empty;

                    MainObject.Access = accessModel;

                    switch (MainObject.Access.Access)
                    {
                        case 0:
                            tempAccessDesc = "Просмотр";
                            break;
                        case 1:
                            tempAccessDesc = "Управление";
                            break;
                        case 2:
                            tempAccessDesc = "Администрирование";
                            break;
                    }

                    SwitchTab("UserWorks");

                    Application.Current.Dispatcher.Invoke(() => Message.Show("Внимание", $"Ваш уровень доступа изменился на {tempAccessDesc}", MessageBoxButton.OK));
                }
            });
        }

        /// <summary>
        /// Обработчик события обрыва соединения
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private Task Reconnect(Exception ex)
        {
            return Task.Run(async () =>
            {
                var i = 1;

                Stat = false;

                while (!Stat)
                {
                    try
                    {
                        ConnectStatus = $"Соединение разорвано. Попытка подключения {i++}";

                        await ConnectionClass.hubConnection.StartAsync();

                        Stat = true;

                        Thread.Sleep(1000);

                        ConnectStatus = $@"Установлено соединение с сервером ""{ConnectionClass.connectionPath.Server}""";
                    }
                    catch { continue; }
                }
            });
        }

        /// <summary>
        /// Метод изменения отображения вкладок
        /// </summary>
        /// <param name="obj"></param>
        private void SwitchTab(object obj)
        {
            switch (obj)
            {
                case "UserWorks":
                    UserWorksViewModel.ActiveVisibility = Visibility.Visible;
                    AllWorksViewModel.ActiveVisibility = Visibility.Collapsed;
                    RepairsViewModel.ActiveVisibility = Visibility.Collapsed;
                    DevicesViewModel.ActiveVisibility = Visibility.Collapsed;
                    AdministrateViewModel.ActiveVisibility = Visibility.Collapsed;
                    break;
                case "AllWorks":
                    AllWorksViewModel.ActiveVisibility = Visibility.Visible;
                    RepairsViewModel.ActiveVisibility = Visibility.Collapsed;
                    UserWorksViewModel.ActiveVisibility = Visibility.Collapsed;
                    DevicesViewModel.ActiveVisibility = Visibility.Collapsed;
                    AdministrateViewModel.ActiveVisibility = Visibility.Collapsed;
                    break;
                case "AllRepairs":
                    RepairsViewModel.ActiveVisibility = Visibility.Visible;
                    AllWorksViewModel.ActiveVisibility = Visibility.Collapsed;
                    UserWorksViewModel.ActiveVisibility = Visibility.Collapsed;
                    DevicesViewModel.ActiveVisibility = Visibility.Collapsed;
                    AdministrateViewModel.ActiveVisibility = Visibility.Collapsed;
                    break;
                case "AllDevices":
                    DevicesViewModel.ActiveVisibility = Visibility.Visible;
                    UserWorksViewModel.ActiveVisibility = Visibility.Collapsed;
                    AllWorksViewModel.ActiveVisibility = Visibility.Collapsed;
                    RepairsViewModel.ActiveVisibility = Visibility.Collapsed;
                    AdministrateViewModel.ActiveVisibility = Visibility.Collapsed;
                    break;
                case "Administrate":
                    AdministrateViewModel.ActiveVisibility = Visibility.Visible;
                    DevicesViewModel.ActiveVisibility = Visibility.Collapsed;
                    UserWorksViewModel.ActiveVisibility = Visibility.Collapsed;
                    AllWorksViewModel.ActiveVisibility = Visibility.Collapsed;
                    RepairsViewModel.ActiveVisibility = Visibility.Collapsed;
                    break;
            }
        }
    }
}
