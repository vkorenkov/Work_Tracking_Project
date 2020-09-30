using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTrackingLib.Models
{
    /// <summary>
    /// Класс предоставляет списки для окна администрирования
    /// </summary>
    public class ControlSqlObjects : PropertyChangeClass
    {
        #region Свойства

        private ObservableCollection<AccessModel> admins;
        /// <summary>
        /// Свойство списка пользователей
        /// </summary>
        public ObservableCollection<AccessModel> Admins 
        {
            get { return admins; }
            set { admins = value; OnPropertyChanged(nameof(Admins)); }
        }

        private ObservableCollection<Osp> ospCol;
        /// <summary>
        /// Свойство списка ОСП
        /// </summary>
        public ObservableCollection<Osp> OspCol
        {
            get { return ospCol; }
            set { ospCol = value; OnPropertyChanged(nameof(OspCol)); }
        }

        private ObservableCollection<OsType> osTypeCol;
        /// <summary>
        /// Свойство списка типа ОС
        /// </summary>
        public ObservableCollection<OsType> OsTypeCol
        {
            get { return osTypeCol; }
            set { osTypeCol = value; OnPropertyChanged(nameof(OsTypeCol)); }
        }

        private ObservableCollection<Results> resultsCol;
        /// <summary>
        /// Свойство списка результатов
        /// </summary>
        public ObservableCollection<Results> ResultsCol
        {
            get { return resultsCol; }
            set { resultsCol = value; OnPropertyChanged(nameof(ResultsCol)); }
        }

        private ObservableCollection<Why> whyCol;
        /// <summary>
        /// Свойство списка причин обращения
        /// </summary>
        public ObservableCollection<Why> WhyCol
        {
            get { return whyCol; }
            set { whyCol = value; OnPropertyChanged(nameof(WhyCol)); }
        }
        
        private ObservableCollection<ScOks> scOksCol;
        /// <summary>
        /// Свойство списка причин обращения
        /// </summary>
        public ObservableCollection<ScOks> ScOksCol
        {
            get { return scOksCol; }
            set { scOksCol = value; OnPropertyChanged(nameof(ScOksCol)); }
        }

        #endregion

        #region Конструкторы

        public ControlSqlObjects(ObservableCollection<AccessModel> accessModel,
            ObservableCollection<Osp> ops, ObservableCollection<OsType> osType,
            ObservableCollection<Results> results, ObservableCollection<Why> why, ObservableCollection<ScOks> scOks)
        {
            Admins = accessModel;
            OspCol = ops;
            OsTypeCol = osType;
            ResultsCol = results;
            WhyCol = why;
            ScOksCol = scOks;
        }

        public ControlSqlObjects()
        {
        }

        #endregion
    }
}
