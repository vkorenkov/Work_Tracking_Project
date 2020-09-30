using Microsoft.AspNetCore.SignalR.Client;
using NewWorkTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WorkTrackingLib.Interfaces;
using WorkTrackingLib.Models;
using WorkTrackingLib.ProcessClasses;

namespace NewWorkTracking.ViewModels
{
    class AdministrateViewModel : AbstractViewModel
    {
        private List<ISelectedItem> items;
        /// <summary>
        /// Свойство коллекция выбранных объектов
        /// </summary>
        public List<ISelectedItem> Items
        {
            get => items;
            set { items = value; OnPropertyChanged(nameof(Items)); }
        }

        private string catSelected;
        /// <summary>
        /// Свойство выбранной категории
        /// </summary>
        public string CatSelected
        {
            get => catSelected;
            set
            {
                catSelected = value;
                OnPropertyChanged(CatSelected);
                ChangeCat();
            }
        }

        /// <summary>
        /// Поле таблицы
        /// </summary>
        private string table;

        /// <summary>
        /// Поле копии выбранного объекта
        /// </summary>
        private ISelectedItem selectedItemCopy;

        private ISelectedItem selectedItem;
        /// <summary>
        /// Свойство выбранного объекта
        /// </summary>
        public ISelectedItem SelectedItem
        {
            get => selectedItem;
            set { selectedItem = value; OnPropertyChanged(nameof(SelectedItem)); if (SelectedItem != null) selectedItemCopy = (ISelectedItem)SelectedItem.Clone(); }
        }

        private List<int> accessLevels;
        /// <summary>
        /// Свойсто коллекция уровней доступа
        /// </summary>
        public List<int> AccessLevels
        {
            get => accessLevels;
            set { accessLevels = value; OnPropertyChanged(nameof(AccessLevels)); }
        }

        /// <summary>
        /// Копия выбранного объекта пользователя
        /// </summary>
        private Admins selectedAdminCopy;

        private Admins selectedAdmin;
        /// <summary>
        /// Свойство выбранного пользователя
        /// </summary>
        public Admins SelectedAdmin
        {
            get => selectedAdmin;
            set { selectedAdmin = value; OnPropertyChanged(nameof(SelectedAdmin)); if (SelectedAdmin != null) selectedAdminCopy = (Admins)SelectedAdmin.Clone(); }
        }

        private int accessLevel;
        /// <summary>
        /// Свойство уровня доступа
        /// </summary>
        public int AccessLevel
        {
            get => accessLevel;
            set { accessLevel = value; OnPropertyChanged(nameof(AccessLevel)); }
        }

        private string newUser;
        /// <summary>
        /// Свойтсво нового имени пользователя
        /// </summary>
        public string NewUser
        {
            get => newUser;
            set { newUser = value; OnPropertyChanged(nameof(NewUser)); }
        }

        private string item;
        /// <summary>
        /// Свойство нового объекта
        /// </summary>
        public string Item
        {
            get => item;
            set { item = value; OnPropertyChanged(nameof(Item)); }
        }

        private Dispatcher dispatcher;

        /// <summary>
        /// Команда добавления нового объекта
        /// </summary>
        public ICommand AddObject => new RelayCommand<object>(obj =>
        {
            // Действие, если добавляется пользователь
            if ((string)obj == "User")
            {
                // Поддтверждение действия
                if (Message.Show("Внимание", $@"Добавить нового польщователя с Ф.И.О ""{NewUser}"" и уровнем доступа ""{AccessLevel}""?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Отправка запроса и объекта нового пользователя на сервер
                    ConnectionClass.hubConnection.InvokeAsync("RunAddNewUser", new Admins() { Name = NewUser, Access = AccessLevel });

                    // Очистка строки нового пользователя
                    NewUser = string.Empty;
                }
            }
            // действие, если добавляется объект отличный от объекта пользователя
            else
            {
                // Поддтверждение действия
                if (Message.Show("Внимание", $@"Добавить ""{Item}""?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Отправка запроса и объекта нового объекта на сервер
                    ConnectionClass.hubConnection.InvokeAsync("RunAddNewItem", new BaseTableModel() { Name = Item }, table);

                    // Очистка строки нового объекта
                    Item = string.Empty;

                    ChangeCat();
                }
            }
        });

        /// <summary>
        /// Команда изменения выбранного объекта
        /// </summary>
        public ICommand ChangeItem => new RelayCommand<object>(obj =>
        {
            // Действие при изменении пользовтеля
            if ((string)obj == "User")
            {
                // Подтверждение действия
                if (Message.Show("Внимание", $@"Изменить пользователя ""{selectedAdminCopy.Name}"" с уровнем доступа ""{selectedAdminCopy.Access}"" на ""{SelectedAdmin.Name}"" и уровнем доступа ""{SelectedAdmin.Access}""?",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Отправка запроса и измененного объекта пользователя на сервер
                    ConnectionClass.hubConnection.InvokeAsync("StartChangeUser", new Admins() { Id = SelectedAdmin.Id, Name = SelectedAdmin.Name, Access = SelectedAdmin.Access });
                }
                else
                {
                    SelectedAdmin = selectedAdminCopy;
                }
            }
            // Действие при изменении объекта
            else
            {
                // Подтверждение действия
                if (Message.Show("Внимание", $@"Изменить ""{selectedItemCopy.Name}"" на ""{SelectedItem.Name}""?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Отправка запроса и измененного объекта на сервер
                    ConnectionClass.hubConnection.InvokeAsync("StartChangeItem", SelectedItem, table);
                }
                else
                {
                    SelectedItem = selectedItemCopy;
                }
            }
        });

        /// <summary>
        /// Команда удаления выбранного объекта
        /// </summary>
        public ICommand DelObject => new RelayCommand<object>(obj =>
        {
            // Действие при удалении пользовтеля
            if ((string)obj == "User")
            {
                if (Message.Show("Внимание", $@"Удалить пользователя ""{SelectedAdmin.Name}"" с уровнем доступа ""{SelectedAdmin.Access}""?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Отправка запроса на удаление пользователя на сервер
                    ConnectionClass.hubConnection.InvokeAsync("RunDelObject", SelectedAdmin, "User");

                    SelectedAdmin = null;
                }
            }
            // Действие при удалении объекта
            else
            {
                if (Message.Show("Внимание", $@"Удалить ""{SelectedItem.Name}""?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Отправка запроса на удаление объекта на сервер
                    ConnectionClass.hubConnection.InvokeAsync("RunDelObject", SelectedItem, table);

                    SelectedItem = null;
                }
            }
        });

        public AdministrateViewModel(MainObject mainObject)
        {
            dispatcher = Application.Current.Dispatcher;
            NewDictionary();
            MainObject = mainObject;
            SignalRActions();
            CatSelected = "СцОкс";
        }

        /// <summary>
        /// Метод подписки на сообщения от сервера
        /// </summary>
        protected override void SignalRActions()
        {
            // Действие при измении пользователя
            ConnectionClass.hubConnection.On<Admins>("UpdateUsers", (user) =>
            {
                dispatcher.Invoke(() =>
                {
                    MainObject.ComboBox.Accesses.Add(user);

                    MainObject.ComboBox.Accesses = new List<Admins>(MainObject.ComboBox.Accesses);
                });
            });

            // Действие при измении объекта
            ConnectionClass.hubConnection.On<ComboboxDataSource>("UpdateItem", (comboboxes) => 
            { 
                dispatcher.Invoke(() => MainObject.ComboBox = comboboxes); ChangeCat(); 
            });
        }

        private void NewDictionary()
        {
            AccessLevels = new List<int>()
            {
                0,
                1,
                2,
            };
        }

        /// <summary>
        /// Метод измнения названия таблиц
        /// </summary>
        private void ChangeCat()
        {
            if (!string.IsNullOrEmpty(CatSelected))
            {
                switch (CatSelected.Remove(0, CatSelected.IndexOf(' ') + 1))
                {
                    case "СцОкс":
                        Items = new List<ISelectedItem>(MainObject.ComboBox.ScOks);
                        table = "ScOks";
                        break;
                    case "ОСП":
                        Task.Run(() => Items = new List<ISelectedItem>(MainObject.ComboBox.OspList));
                        table = "Osp";
                        break;
                    case "Типы ОС":
                        Items = new List<ISelectedItem>(MainObject.ComboBox.OsTypeList);
                        table = "OsType";
                        break;
                    case "Результаты работ":
                        Items = new List<ISelectedItem>(MainObject.ComboBox.ResultsList);
                        table = "Results";
                        break;
                    case "Причины обращения":
                        Items = new List<ISelectedItem>(MainObject.ComboBox.WhyList);
                        table = "Why";
                        break;
                    case "Статусы ремонта":
                        Items = new List<ISelectedItem>(MainObject.ComboBox.RepairStatus);
                        table = "RepairStatus";
                        break;
                }
            }
        }
    }
}
