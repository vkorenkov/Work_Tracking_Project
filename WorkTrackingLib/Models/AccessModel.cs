using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTrackingLib.Models
{
    public class AccessModel : PropertyChangeClass
    {
        #region Свойства

        /// <summary>
        /// Свойство ID Сотрудника в БД
        /// </summary>
        public int Id { get; set; }

        private string name;
        /// <summary>
        /// Свойство имени сотрудника в БД
        /// </summary>
        public string Name 
        {
            get { return name; }
            set { name = value; OnPropertyChanged(nameof(Name)); }
        }

        private int access;
        /// <summary>
        /// Свойство уровня доступа
        /// </summary>
        public int Access 
        {
            get { return access; }
            set { access = value; OnPropertyChanged(nameof(Access)); }
        }

        private bool visibilityControlSql;
        /// <summary>
        /// Свойство видимости кнопки управления БД
        /// </summary>
        public bool VisibilityControlSql
        {
            get { return visibilityControlSql; }
            set { visibilityControlSql = value; OnPropertyChanged(nameof(VisibilityControlSql)); }
        }

        private bool visibilityControl;
        /// <summary>
        /// Свойство видимости кнопки управления БД
        /// </summary>
        public bool VisibilityControl
        {
            get { return visibilityControl; }
            set { visibilityControl = value; OnPropertyChanged(nameof(VisibilityControl)); }
        }
        
        private bool visibilityRepairs;
        /// <summary>
        /// Свойство видимости кнопки управления БД
        /// </summary>
        public bool VisibilityRepairs
        {
            get { return visibilityRepairs; }
            set { visibilityRepairs = value; OnPropertyChanged(nameof(VisibilityRepairs)); }
        }
        
        private bool visibilityDevices;
        /// <summary>
        /// Свойство видимости кнопки управления БД
        /// </summary>
        public bool VisibilityDevices
        {
            get { return visibilityDevices; }
            set { visibilityDevices = value; OnPropertyChanged(nameof(VisibilityDevices)); }
        }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор, принимающий получаемые параметры
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Name"></param>
        /// <param name="Access"></param>
        public AccessModel(int Id, string Name, int Access)
        {
            this.Id = Id;
            this.Name = Name;
            this.Access = Access;
        }

        /// <summary>
        /// Пустой конструктор
        /// </summary>
        public AccessModel()
        {
        }

        public AccessModel(string Name)
        {
            this.Name = Name;
        }

        #endregion
    }
}
