using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTrackingLib.Interfaces;

namespace WorkTrackingLib
{
    public class ControlSqlModel : ISelectedItem
    {
        #region Свойства

        /// <summary>
        /// Свойство идентификационного номера в таблице БД
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Свойство имени
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор получения данных из БД 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Name"></param>
        public ControlSqlModel(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

        /// <summary>
        /// пустой конструктор
        /// </summary>
        public ControlSqlModel()
        {
        }

        #endregion

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
