using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WorkTrackingLib;
using WorkTrackingLib.Models;
using NewWorkTracking.Interfaces;
using System.Windows;

namespace NewWorkTracking.ViewModels
{
    abstract class AbstractViewModel : PropertyChangeClass
    {
        /// <summary>
        /// 
        /// </summary>
        public string Actively { get; set; }

        private IFilters filter;
        /// <summary>
        /// Свойство фильтра
        /// </summary>
        public IFilters Filter 
        {
            get => filter;
            set { filter = value; OnPropertyChanged(nameof(Filter)); }
        }

        private ListCollectionView usersWorks;
        /// <summary>
        /// Коллекция представления вывода данных в таблицу
        /// </summary>
        public ListCollectionView UsersWorks
        {
            get => usersWorks;
            set { usersWorks = value; OnPropertyChanged(nameof(UsersWorks)); }
        }

        private MainObject mainObject;
        /// <summary>
        /// Свойство основного объекта полученных данных
        /// </summary>
        public MainObject MainObject 
        {
            get => mainObject;
            set { mainObject = value; OnPropertyChanged(nameof(MainObject)); }
        }

        private Visibility activeVisibility;
        /// <summary>
        /// Свойство видимости вкладки
        /// </summary>
        public Visibility ActiveVisibility
        {
            get => activeVisibility;
            set { activeVisibility = value; OnPropertyChanged(nameof(activeVisibility)); }
        }

        public AbstractViewModel()
        {
            ActiveVisibility = Visibility.Collapsed;

            MainObject = new MainObject();
        }

        /// <summary>
        /// Метод подписки на ответы сервра
        /// </summary>
        protected abstract void SignalRActions();
    }
}
