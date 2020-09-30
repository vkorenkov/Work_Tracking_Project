using NewWorkTracking.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WorkTrackingLib;
using WorkTrackingLib.ProcessClasses;

namespace NewWorkTracking.ViewModels
{
    class ChangeServerViewModel : PropertyChangeClass
    {
        private string description;
        /// <summary>
        /// Свойство вывода текущего действия 
        /// </summary>
        public string Description
        {
            get => description;
            set { description = value; OnPropertyChanged(nameof(Description)); }
        }

        private string newServer;
        /// <summary>
        /// Свойство нового сервера
        /// </summary>
        public string NewServer
        {
            get => newServer;
            set { newServer = value; OnPropertyChanged(nameof(NewServer)); }
        }

        private bool controlEnable;
        /// <summary>
        /// Активация\деактивация управления окном загрузки
        /// </summary>
        public bool ControlEnable
        {
            get { return controlEnable; }
            set { controlEnable = value; OnPropertyChanged(nameof(ControlEnable)); }
        }


        /// <summary>
        /// Команда изменения сервера
        /// </summary>
        public ICommand ChangeServer
        {
            get
            {
                return new RelayCommand<object>(async obg =>
                {
                    Description = "Проверка введенных данных. Подождите.";

                    ControlEnable = false;

                    if (await newServer.WriteNewServer())
                    {
                        Description = $@"Сервер успешно изменен. Текущий сервер ""{ConnectionClass.connectionPath.Server}""";

                        ControlEnable = true;

                        Process.Start($"Work_Tracking.exe");

                        Application.Current.Shutdown();
                    }
                    else
                    {
                        Description = $@"Ошибка подключения к новому серверу. Проверьте введенные данные, сервер не будет изменен. Введенный вами сервер ""{NewServer}"". Текущий сервер: ""{ConnectionClass.connectionPath.Server}""";

                        ControlEnable = true;
                    }
                });
            }
        }

        public ICommand CloseChsngeServer => new RelayCommand<object>(obj => 
        {
            Application.Current.Windows.CloseWindow("ChangeServer");
        });

        public ChangeServerViewModel()
        {
            ControlEnable = true;

            Description = $@"Введите имя или IP-адрес сервера без ""\\"". Пример имени сервера: m1-dl-server. Пример IP-адреса: 10.130.10.10. Текущий адрес сервера: ""{ConnectionClass.connectionPath.Server}""";
        }
    }
}
