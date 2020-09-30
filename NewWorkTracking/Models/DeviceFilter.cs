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
    class DeviceFilter : PropertyChangeClass, IFilters
    {
        public DateTime? DateOne { get; set; } // Не используется в фильтрации устройств
        public DateTime? DateTwo { get; set; } //

        private ListCollectionView devices;

        private string searchLine;
        /// <summary>
        /// Строка поиска
        /// </summary>
        public string SearchLine
        {
            get => searchLine;
            set { searchLine = value; OnPropertyChanged(nameof(SearchLine)); devices.Filter = new Predicate<object>(Search); }
        }

        public string OsTypeFilter { get ; set; } // Не используется в фильтрации устройств

        public DeviceFilter(ListCollectionView worksView)
        {
            devices = worksView;
        }

        /// <summary>
        /// Метод поиска
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool Search(object obj)
        {
            if (obj is Devices temp)
            {
                if (temp.OsName.ToLower().Contains(SearchLine.ToLower()))
                {
                    return true;
                }
                else if (temp.DeviceName.ToLower().Contains(SearchLine.ToLower()))
                {
                    return true;
                }
                else if (temp.InvNumber.ToLower().Contains(SearchLine.ToLower()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
