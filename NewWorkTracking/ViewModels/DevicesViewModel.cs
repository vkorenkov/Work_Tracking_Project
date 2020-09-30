using NewWorkTracking.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WorkTrackingLib.Models;
using System.Windows.Threading;
using System.Windows.Input;
using WorkTrackingLib.ProcessClasses;
using System.Collections.ObjectModel;

namespace NewWorkTracking.ViewModels
{
    /// <summary>
    /// Контекст данных вкладки устройств
    /// </summary>
    class DevicesViewModel : AbstractViewModel
    {
        /// <summary>
        /// Коллекция импортированных данных из файла Excel
        /// </summary>
        private List<RepairClass> LoadedRepairs;

        /// <summary>
        /// Копия выбранного устройства 
        /// </summary>
        private Devices selectedDeviceCopy;

        private Devices selectedDevice;
        /// <summary>
        /// Выбранное устройство
        /// </summary>
        public Devices SelectedDevice
        {
            get => selectedDevice;
            set { selectedDevice = value; OnPropertyChanged(nameof(SelectedDevice)); selectedDeviceCopy = (Devices)SelectedDevice.Clone(); }
        }

        private RepairClass selectedRepair;
        /// <summary>
        /// Выбранный ремонт
        /// </summary>
        public RepairClass SelectedRepair
        {
            get => selectedRepair;
            set { selectedRepair = value; OnPropertyChanged(nameof(SelectedRepair)); }
        }

        private string deviceName;
        /// <summary>
        /// Свойство модели устройства
        /// </summary>
        public string DeviceName
        {
            get => deviceName;
            set { deviceName = value; OnPropertyChanged(nameof(DeviceName)); }
        }

        private string invNumber;
        /// <summary>
        /// Свойство инвентарного номера устройства
        /// </summary>
        public string InvNumber
        {
            get => invNumber;
            set { invNumber = value; OnPropertyChanged(nameof(InvNumber)); }
        }

        private string osName;
        /// <summary>
        /// Свойство типа устройства
        /// </summary>
        public string OsName
        {
            get => osName;
            set { osName = value; OnPropertyChanged(nameof(OsName)); }
        }

        /// <summary>
        /// Переменная диспетчера для обращения к объектам из другого потока
        /// </summary>
        private Dispatcher dispather;

        /// <summary>
        /// Команда записи изменений ремонта
        /// </summary>
        public ICommand ChangeRepair => new RelayCommand<object>(obj =>
        {
            ConnectionClass.hubConnection.InvokeAsync("StartChangeRepair", SelectedRepair);
        });

        /// <summary>
        /// Команда записи изменений устройства
        /// </summary>
        public ICommand ChangeDevice => new RelayCommand<object>(obj =>
        {
            ConnectionClass.hubConnection.InvokeAsync("StartChangeDevice", SelectedDevice);
        });

        /// <summary>
        /// Команда добавления нового устройства
        /// </summary>
        public ICommand AddNewDevice => new RelayCommand<object>(obj =>
        {
            ConnectionClass.hubConnection.InvokeAsync("RunAddDevice", new Devices() { DeviceName = DeviceName, InvNumber = InvNumber, OsName = OsName });
        });

        /// <summary>
        /// Команда импорта данных из excel
        /// </summary>
        public ICommand LoadRepairs => new RelayCommand<object>(obj =>
        {
            SaveOpenFile saveOpenFile = new SaveOpenFile();

            ExcelUsage excelUsage = new ExcelUsage();

            var path = saveOpenFile.OpenDialog();

            // Условие выполнения команды
            if (!string.IsNullOrEmpty(path))
            {
                // запуск считывания файла в другом потоке
                Task.Run(() =>
                {
                    // Выполнение метода импорта данных из файла excel
                    string output = excelUsage.LoadExcel(path, LoadedRepairs);

                    if (output.Contains("считан"))
                    {
                        // Условие выполнения запроса на сервер на запись данных в БД
                        if (dispather.Invoke(() => Message.Show("Внимание", $"{output} Объектов в файле: {LoadedRepairs.Count()}. Загрузить в БД?", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                        {
                            foreach (var t in LoadedRepairs)
                            {
                                ConnectionClass.hubConnection.InvokeAsync("RunAddRepair", t);
                            }

                            LoadedRepairs.Clear();
                        }
                        else
                        {
                            LoadedRepairs.Clear();
                        }
                    }
                    // Действие при ошибке выполнения метода excelUsage.LoadExcel
                    else
                    {
                        dispather.Invoke(() => Message.Show("Внимание", output, MessageBoxButton.OK));
                    }
                });
            }
        });

        /// <summary>
        /// Команда запуска окна добавления нового ремонта
        /// </summary>
        public ICommand OpenCleanRepairCard => new RelayCommand<object>(obj =>
        {
            // Условие при котором будет открыто окно добавлнеия новой работы
            if (SelectedDevice != null)
            {
                RepairCardViewModel cardViewModel = new RepairCardViewModel(MainObject, SelectedDevice);

                RepairCard repairCard = new RepairCard()
                {
                    DataContext = cardViewModel
                };

                repairCard.ShowDialog();
            }
            else
            {
                Message.Show("", "Выберите устройство", MessageBoxButton.OK);
            }
        });

        public DevicesViewModel(MainObject mainObject)
        {
            SignalRActions();
            LoadedRepairs = new List<RepairClass>();
            ActiveVisibility = Visibility.Collapsed;
            MainObject = mainObject;
            UsersWorks = new ListCollectionView(MainObject.Devices);
            Filter = new DeviceFilter(UsersWorks);
            dispather = Application.Current.Dispatcher;
        }

        /// <summary>
        /// Метод подписки на сообщения сервера
        /// </summary>
        protected override void SignalRActions()
        {
            ConnectionClass.hubConnection.On<Devices>("UpdateDevices", (newDevice) =>
            {
                dispather.Invoke(() => MainObject.Devices.Insert(0, newDevice));
            });

            ConnectionClass.hubConnection.On<bool>("DeviceNotAdded", (result) => dispather.Invoke(() => 
            Message.Show("Ошибка добавления", "Ошибка добавления. Возможно такое устройство уже существует", MessageBoxButton.OK)));

            ConnectionClass.hubConnection.On<bool>("DeviceChangedError", (result) => dispather.Invoke(() => 
            {
                Message.Show("Ошибка изменения", $@"Вы не можете изменить инв.номер выбранного устройства на ""{SelectedDevice.InvNumber}"", т.к. устройство с таким инв.номером уже существует", MessageBoxButton.OK);

                SelectedDevice = selectedDeviceCopy;
            }));

            ConnectionClass.hubConnection.On<Devices>("ChangedDevice", (changeDevice) =>
            {
                dispather.Invoke(() =>
                {
                    foreach (var a in MainObject.Devices.Where(x => x.Id == changeDevice.Id).FirstOrDefault().GetType().GetProperties())
                    {
                        foreach (var c in changeDevice.GetType().GetProperties())
                        {
                            if (a.Name == c.Name)
                            {
                                a.SetValue(MainObject.Devices.Where(x => x.Id == changeDevice.Id).FirstOrDefault(), c.GetValue(changeDevice));

                                continue;
                            }
                        }
                    }
                });
            });

            ConnectionClass.hubConnection.On<RepairClass>("UpdateRepairs", (newRepair) =>
            {
                foreach (var r in MainObject.Devices)
                {
                    if (r.Id == newRepair.DeviceId)
                    {
                        dispather.Invoke(() => { r.Repairs.Add(newRepair); r.Repairs = new List<RepairClass>(r.Repairs); });

                        break;
                    }
                }
            });

            ConnectionClass.hubConnection.On<RepairClass>("ChangedRepair", (changeRepair) =>
            {
                dispather.Invoke(() =>
                {
                    var tempDevice = MainObject.Devices.Where(x => x.Id == changeRepair.DeviceId).FirstOrDefault();

                    foreach (var a in tempDevice.Repairs.Where(x => x.Id == changeRepair.Id).FirstOrDefault().GetType().GetProperties())
                    {
                        foreach (var c in changeRepair.GetType().GetProperties())
                        {
                            if (a.Name == c.Name)
                            {
                                a.SetValue(tempDevice.Repairs.Where(x => x.Id == changeRepair.Id).FirstOrDefault(), c.GetValue(changeRepair));

                                continue;
                            }
                        }
                    }
                });
            });
        }
    }
}
