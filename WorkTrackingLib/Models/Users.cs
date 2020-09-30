using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTrackingLib
{
    /// <summary>
    /// Класс Содержит поля и методы для получения ФИО авторизованного в системе пользователя
    /// </summary>
    public class Users
    {
        #region Свойства

        /// <summary>
        /// Свойство имени пользователя
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Свойство ID выбранной заявки
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Свойство сообщения
        /// </summary>
        public string Msg { get; set; } = string.Empty;

        #endregion

        #region Конструкторы

        public Users(string displayName, int orderId)
        {
            this.DisplayName = displayName;
            this.OrderId = orderId;
        }

        public Users()
        {
        }

        #endregion 

        #region Методы

        /// <summary>
        /// Метод получение данных по пользователю запустишему программу
        /// </summary>
        /// <returns></returns>
        public static string GetAccaunt()
        {
            return UserPrincipal.Current.DisplayName;

            #region Получение должности (на данный момент не используется)

            //// получение данных о пользователе из AD
            //DirectoryEntry dirEntr = userName.GetUnderlyingObject() as DirectoryEntry;

            //// Условие совпадения должности
            //if (dirEntr.Properties.Contains("Title"))
            //{
            //    // ПРисвоение переменной значения должности пользователя
            //    var title = dirEntr.Properties["Title"].Value.ToString();

            //    // Условие, при котором дается доступ к модулю управления пользователю (Если должность содержится в списке доверенных должностей пользователь получит доступ)
            //    if (MainViewModel.AccauntList.Contains(title))
            //    {
            //        buttonVis = Visibility.Visible;
            //    }
            //    else
            //    {
            //        buttonVis = Visibility.Hidden;
            //    }

            //}
            //else
            //{
            //    buttonVis = Visibility.Hidden;
            //}

            #endregion

        }

        #endregion
    }
}
