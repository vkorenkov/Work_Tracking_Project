using NewWorkTracking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WorkTrackingLib;
using WorkTrackingLib.Models;

namespace NewWorkTracking.Models
{
    class AllRepairsFilter : PropertyChangeClass, IFilters
    {
        /// <summary>
        /// Коллекция представления для фильтрации
        /// </summary>
        private ListCollectionView allRepairs;
       
        private DateTime? dateOne;
        /// <summary>
        /// Первая дата периода фильтрации
        /// </summary>
        public DateTime? DateOne 
        {
            get => dateOne;
            set { dateOne = value; OnPropertyChanged(nameof(DateOne)); allRepairs.Filter = new Predicate<object>(ItemsFilter); }
        }
      
        private DateTime? dateTwo;
        /// <summary>
        /// Вторая дата периода фильтрации
        /// </summary>
        public DateTime? DateTwo 
        {
            get => dateTwo;
            set { dateTwo = value; OnPropertyChanged(nameof(DateTwo)); allRepairs.Filter = new Predicate<object>(ItemsFilter); }
        }

        private string osTypeFilter;
        /// <summary>
        /// Фильтр по типу основного средства
        /// </summary>
        public string OsTypeFilter
        {
            get => osTypeFilter;
            set { osTypeFilter = value; OnPropertyChanged(nameof(OsTypeFilter)); allRepairs.Filter = new Predicate<object>(ItemsFilter); }
        }

        private string selectedStatus;
        /// <summary>
        /// Фильтр по выбранному статусу ремонта
        /// </summary>
        public string SelectedStatus
        {
            get => selectedStatus;
            set { selectedStatus = value; OnPropertyChanged(nameof(SelectedStatus)); allRepairs.Filter = new Predicate<object>(ItemsFilter); }
        }

        private string selectedModel;
        /// <summary>
        /// Фильтр по выбранной модели устройства
        /// </summary>
        public string SelectedModel
        {
            get => selectedModel;
            set { selectedModel = value; OnPropertyChanged(nameof(selectedModel)); allRepairs.Filter = new Predicate<object>(ItemsFilter); }
        }

        private string selectedScOks;
        /// <summary>
        /// Фильтр по СЦ ОКС
        /// </summary>
        public string SelectedScOks
        {
            get => selectedScOks;
            set { selectedScOks = value; OnPropertyChanged(nameof(SelectedScOks)); allRepairs.Filter = new Predicate<object>(ItemsFilter); }
        }

        private string selectedKaProvider;
        /// <summary>
        /// Фильтр по КА постовщика
        /// </summary>
        public string SelectedKaProvider
        {
            get => selectedKaProvider;
            set { selectedKaProvider = value; OnPropertyChanged(nameof(SelectedKaProvider)); allRepairs.Filter = new Predicate<object>(ItemsFilter); }
        }

        private string selectedOsName;
        /// <summary>
        /// Фильтр по типу основного средства
        /// </summary>
        public string SelectedOsName
        {
            get => selectedOsName;
            set { selectedOsName = value; OnPropertyChanged(nameof(SelectedOsName)); allRepairs.Filter = new Predicate<object>(ItemsFilter); }
        }

        private string selectedWaranty;
        /// <summary>
        /// Фильтр по типу гарантии
        /// </summary>
        public string SelectedWaranty
        {
            get => selectedWaranty;
            set { selectedWaranty = value; OnPropertyChanged(nameof(SelectedWaranty)); allRepairs.Filter = new Predicate<object>(ItemsFilter); }
        }

        private string searchLine;
        /// <summary>
        /// Строка поиска
        /// </summary>
        public string SearchLine
        {
            get => searchLine;
            set { searchLine = value; OnPropertyChanged(nameof(SearchLine)); allRepairs.Filter = new Predicate<object>(Search); }
        }

        /// <summary>
        /// Конструктор принимает коллекцию представления для фильтрации
        /// </summary>
        /// <param name="worksView"></param>
        public AllRepairsFilter(ListCollectionView worksView)
        {
            allRepairs = worksView;
        }

        /// <summary>
        /// Метод фильтрует данные по выбранным параметрам
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool ItemsFilter(object obj)
        {
            if (obj is RepairClass temp)
            {
                return ((temp.Date >= DateOne || DateOne == null)
                    && (temp.Date <= DateTwo || DateTwo == null))
                    && (temp.OsName == SelectedOsName || string.IsNullOrWhiteSpace(SelectedOsName))
                    && (temp.Status == SelectedStatus || string.IsNullOrWhiteSpace(SelectedStatus))
                    && (temp.Model == SelectedModel || string.IsNullOrWhiteSpace(SelectedModel))
                    && (temp.KaProvider == SelectedKaProvider || string.IsNullOrWhiteSpace(SelectedKaProvider))
                    && (temp.Warranty == SelectedWaranty || string.IsNullOrWhiteSpace(SelectedWaranty))
                    && (temp.ScOks == SelectedScOks || string.IsNullOrWhiteSpace(SelectedScOks));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Метод производит поиск по ключевым словам во всех отображаемых полях объекта
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool Search(object obj)
        {
            if (obj is RepairClass temp)
            {
                bool result = false;

                foreach (var p in temp.GetType().GetProperties())
                {
                    if (p.GetValue(temp) != null && p.GetValue(temp).ToString().ToLower().Contains(SearchLine.ToLower()))
                    {
                        result = true;
                    }
                }

                return result;
            }
            else
            {
                return false;
            }
        }
    }
}
