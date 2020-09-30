using Microsoft.AspNetCore.SignalR.Client;
using NewWorkTracking.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using WorkTrackingLib;
using WorkTrackingLib.Models;
using WorkTrackingLib.ProcessClasses;

namespace NewWorkTracking.ViewModels
{
    class AllWorksViewModel : AbstractViewModel
    {
        private Dispatcher dispatcher;

        /// <summary>
        /// Свойство коллекция статусов объекта работ
        /// </summary>
        public ObservableCollection<string> OrderActivity { get; set; } =
            new ObservableCollection<string>() { "Активно", "Архив", "Необходимо заполнить номер модернизации или списания", "Списано", "Модернизировано" };

        /// <summary>
        /// Коллекция выбранных объектов в DataGrid
        /// </summary>
        private static ObservableCollection<NewWrite> selectedOrders;

        private NewWrite selectedOrder;
        /// <summary>
        /// Свойство выбранного объекта
        /// </summary>
        public NewWrite SelectedOrder
        {
            get => selectedOrder;
            set
            {
                selectedOrder = value; OnPropertyChanged(nameof(SelectedOrder));

                if (SelectedOrder != null)
                    selectedOrderCopy = (NewWrite)SelectedOrder.Clone();
            }
        }

        /// <summary>
        /// Поле копии выбранного объекта
        /// </summary>
        private NewWrite selectedOrderCopy;

        //private WorkCardDataContext workContext;
        //public WorkCardDataContext WorkContext
        //{
        //    get => workContext;
        //    set { workContext = value; OnPropertyChanged(nameof(WorkContext)); }
        //}

        private string status;
        /// <summary>
        /// Поле статуса заявки
        /// </summary>
        public string Status
        {
            get => status;
            set { status = value; OnPropertyChanged(nameof(Status)); }
        }

        /// <summary>
        /// Команда выгрузки файла Excel
        /// </summary>
        public ICommand UploadExcel => new RelayCommand<object>(obj =>
        {
            GetExele();
        });

        /// <summary>
        /// команда изменения данных в выбранном объекте
        /// </summary>
        public ICommand ChangeWork => new RelayCommand<object>(obj =>
        {
            // Отправка запроса и измененного объекта на сервер
            ConnectionClass.hubConnection.InvokeAsync("StartChangeWork", SelectedOrder);
        });

        /// <summary>
        /// Очистка фильтров
        /// </summary>
        public ICommand RefreshFilters => new RelayCommand<object>(obj =>
        {
            // Цикл получения и удаления значний свойств
            foreach (var p in Filter.GetType().GetProperties())
            {
                if (p.PropertyType == typeof(DateTime?))
                {
                    p.SetValue(Filter, null);
                }
                else if (p.PropertyType == typeof(bool))
                {
                    p.SetValue(Filter, false);
                }
                else
                {
                    p.SetValue(Filter, string.Empty);
                }
            }
        });

        /// <summary>
        /// Команда перевода объекта в статус "В архиве"
        /// </summary>
        public ICommand OrderStatus => new RelayCommand<object>(obj =>
        {
            //Запрос на изменение статуса объекта
            if (Message.Show("Архивирование", "Вы уверены в изменении статуса "
                + selectedOrders.Count + " шт. объектов ",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // Добавление выделенных элементов в коллекцию
                foreach (NewWrite t in selectedOrders)
                {
                    // Изменение статуса доступности заявки
                    t.NoActive = "Архив";

                    ConnectionClass.hubConnection.InvokeAsync("StartChangeWork", t);
                }
            }
        });

        /// <summary>
        /// Команда обновления данных в таблице DataGrid
        /// </summary>
        public ICommand Refresh => new RelayCommand<object>(obj =>
        {
            MainObject.AdminWorks = null;

            UsersWorks = null;

            ConnectionClass.hubConnection.InvokeAsync("UpdateAll", MainObject.Access);

            Status = $"Обновлено в {DateTime.Now.ToShortTimeString()}";
        });

        public AllWorksViewModel(MainObject mainObject)
        {
            SignalRActions();

            MainObject = mainObject;

            UsersWorks = new ListCollectionView(mainObject.AdminWorks);

            Filter = new AllWorksFilter(UsersWorks);

            dispatcher = Application.Current.Dispatcher;
        }

        protected override void SignalRActions()
        {
            ConnectionClass.hubConnection.On<NewWrite>("UpdateWorks", (newWork) =>
            {
                Application.Current.Dispatcher.Invoke(() => MainObject.AdminWorks.Insert(0, newWork));
            });

            ConnectionClass.hubConnection.On<MainObject>("UpdateRequest", (main) =>
            {
                dispatcher.Invoke(() => MainObject = main);
                dispatcher.Invoke(() => UsersWorks = new ListCollectionView(MainObject.AdminWorks));
                dispatcher.Invoke(() => Filter = new AllWorksFilter(UsersWorks));
            });

            ConnectionClass.hubConnection.On<NewWrite>("ChangedWork", (changedWork) =>
            {
                dispatcher.Invoke(() =>
                {
                    foreach (var a in MainObject.AdminWorks.Where(x => x.Id == changedWork.Id).FirstOrDefault().GetType().GetProperties())
                    {
                        foreach (var c in changedWork.GetType().GetProperties())
                        {
                            if (a.Name == c.Name)
                            {
                                a.SetValue(MainObject.AdminWorks.Where(x => x.Id == changedWork.Id).FirstOrDefault(), c.GetValue(changedWork));

                                continue;
                            }
                        }
                    }
                });
            });

            ConnectionClass.hubConnection.On<NewWrite>("ChangedError", (changedWork) => Message.Show("Ошибка записи", "Ошибка записи", MessageBoxButton.OK));
        }

        /// <summary>
        /// Метод выгрузки Excel Файла
        /// </summary>
        private async void GetExele()
        {
            string fileName = string.Empty;

            string path = string.Empty;

            //ProgressBarStartStop = true;

            Status = "В работе";

            // Создание временной коллекции 
            List<NewWrite> tempCol;

            // Условие, при котором, выгружаются в файл Excel все работы по фильтрам или без
            if (selectedOrders.Count <= 2)
            {
                tempCol = MainObject.AdminWorks.ToList();
            }
            // Условие при котором выгружаются только выделенные работы
            else
            {
                tempCol = selectedOrders.ToList();
            }

            SaveOpenFile saveOpen = new SaveOpenFile();

            if (Filter.DateOne != null && Filter.DateTwo != null)
            {
                // Имя файла если даты были выбраны
                fileName = "Отчет " + Filter.DateOne.Value.ToString("dd.MM.yyyy") + " - " + Filter.DateTwo.Value.ToString("dd.MM.yyyy");
            }
            else
            {
                // Стандартное имя файла если даты не выбраны
                fileName = "Отчет за все время";
            }

            path = saveOpen.SaveDialog(fileName);

            if (!string.IsNullOrEmpty(path))
            {
                bool temp = await Task.Run(() => ExcelUsage.GetExcel(tempCol, path));

                if (temp == true)
                {
                    Status = $"Сохранено";

                    //ProgressBarStartStop = false;
                }
                else { Status = "Ошибка сохранения"; /*ProgressBarStartStop = false;*/ }
            }
            else { Status = "Сохранение отменено"; /*ProgressBarStartStop = false;*/ }
        }

        /// <summary>
        /// Метод получает список выбранных элементов и формирует из них коллекцию
        /// </summary>
        /// <param name="list"></param>
        public static void SelectionChanged(IList list)
        {
            selectedOrders = new ObservableCollection<NewWrite>();

            foreach (var item in list)
            {
                selectedOrders.Add(item as NewWrite);
            }
        }
    }
}
