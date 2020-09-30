using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTrackingLib.Models
{
    public class Admins : ICloneable
    {
        /// <summary>
        /// Свойство ID Сотрудника в БД
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Свойство имени сотрудника в БД
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Свойство уровня доступа
        /// </summary>
        public int Access { get; set; }

        /// <summary>
        /// Метод реализует интерфейс клонирования объекта
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
