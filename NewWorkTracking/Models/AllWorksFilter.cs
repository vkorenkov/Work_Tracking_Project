using NewWorkTracking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTrackingLib;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Reflection;

namespace NewWorkTracking.Models
{
    class AllWorksFilter : PropertyChangeClass, IFilters
    {
        /// <summary>
        /// Коллекция представления для фильтрации
        /// </summary>
        private ListCollectionView allWorks;

        private DateTime? dateOne;
        /// <summary>
        /// Первая дата периода фильтрации
        /// </summary>
        public DateTime? DateOne 
        {
            get => dateOne;
            set { dateOne = value; OnPropertyChanged(nameof(DateOne)); allWorks.Filter = new Predicate<object>(ItemsFilter); }
        }

        private DateTime? dateTwo;
        /// <summary>
        /// Вторая дата периода фильтрации
        /// </summary>
        public DateTime? DateTwo 
        {
            get => dateTwo;
            set { dateTwo = value; OnPropertyChanged(nameof(DateTwo)); allWorks.Filter = new Predicate<object>(ItemsFilter); }
        }

        private string adminFilter;
        /// <summary>
        /// Фильтр по системному администратору
        /// </summary>
        public string AdminFilter
        {
            get => adminFilter;
            set { adminFilter = value; OnPropertyChanged(nameof(AdminFilter)); allWorks.Filter = new Predicate<object>(ItemsFilter); }
        }

        private string orderType;
        /// <summary>
        /// ФИльтр по типу заявки
        /// </summary>
        public string OrderType
        {
            get => orderType;
            set 
            { 
                orderType = value; OnPropertyChanged(nameof(OrderType)); 
                allWorks.Filter = new Predicate<object>(ItemsFilter); 
            }
        }

        private string ospWorkFilter;
        /// <summary>
        /// Фильтр по ОСП, где выполнялась работа
        /// </summary>
        public string OspWorkFilter
        {
            get => ospWorkFilter;
            set { ospWorkFilter = value; OnPropertyChanged(nameof(OspWorkFilter)); allWorks.Filter = new Predicate<object>(ItemsFilter); }
        }
        
        private string ospOrderFilter;
        /// <summary>
        /// Фильтр по ОСП, где располагается заказчик
        /// </summary>
        public string OspOrderFilter
        {
            get => ospOrderFilter;
            set { ospOrderFilter = value; OnPropertyChanged(nameof(OspOrderFilter)); allWorks.Filter = new Predicate<object>(ItemsFilter); }
        }

        private string reasonFilter;
        /// <summary>
        /// Фильтр по причине обращения
        /// </summary>
        public string ReasonFilter
        {
            get => reasonFilter;
            set { reasonFilter = value; OnPropertyChanged(nameof(ReasonFilter)); allWorks.Filter = new Predicate<object>(ItemsFilter); }
        }

        private string resultFilter;
        /// <summary>
        /// Фильтр по результатам работы
        /// </summary>
        public string ResultFilter
        {
            get => resultFilter;
            set { resultFilter = value; OnPropertyChanged(nameof(ResultFilter)); allWorks.Filter = new Predicate<object>(ItemsFilter); }
        }

        private string osTypeFilter;
        /// <summary>
        /// Фильтр по типу ОС
        /// </summary>
        public string OsTypeFilter 
        {
            get => osTypeFilter;
            set { osTypeFilter = value; OnPropertyChanged(nameof(OsTypeFilter)); allWorks.Filter = new Predicate<object>(ItemsFilter); }
        }

        private bool matches;
        /// <summary>
        /// фильтр совпадения инвентарных номеров
        /// </summary>
        public bool Matches
        {
            get => matches;
            set { matches = value; OnPropertyChanged(nameof(Matches));  }
        }

        private string searchLine;
        /// <summary>
        /// Строка поиска
        /// </summary>
        public string SearchLine 
        {
            get => searchLine;
            set { searchLine = value; OnPropertyChanged(nameof(SearchLine)); allWorks.Filter = new Predicate<object>(Search); }
        }       

        public AllWorksFilter(ListCollectionView worksView)
        {
            allWorks = worksView;
        }

        /// <summary>
        /// Метод фильтрации данных
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool ItemsFilter(object obj)
        {
            if (obj is NewWrite temp)
            {
                return ((temp.Date >= DateOne || DateOne == null)
                    && (temp.Date <= DateTwo || DateTwo == null))
                    && (temp.OspOrder == OspOrderFilter || string.IsNullOrWhiteSpace(OspOrderFilter))
                    && (temp.Who == AdminFilter || string.IsNullOrWhiteSpace(AdminFilter))
                    && (temp.OspWork == OspWorkFilter || string.IsNullOrWhiteSpace(OspWorkFilter))
                    && (temp.OsType == OsTypeFilter || string.IsNullOrWhiteSpace(OsTypeFilter))
                    && (temp.OrderType == OrderType || string.IsNullOrWhiteSpace(OrderType))
                    && (temp.Why == ReasonFilter || string.IsNullOrWhiteSpace(ReasonFilter))
                    && (temp.Results == ResultFilter || string.IsNullOrWhiteSpace(ResultFilter));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Метод поиска по всем отображаемым данным
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool Search(object obj)
        {
            if (obj is NewWrite temp)
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
