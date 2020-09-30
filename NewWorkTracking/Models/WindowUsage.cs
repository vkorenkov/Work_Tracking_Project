using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewWorkTracking.Models
{
    class WindowUsage
    {
        /// <summary>
        /// Метод перезапуска программы
        /// </summary>
        public static void RestartProgramm(string WindowName)
        {
            // Закрытие окна ProgBar
            foreach (Window t in Application.Current.Windows)
            {
                if (t.Name.ToString() == $"{WindowName}")
                {
                    t.Close();
                }
            }

            // Перезагрузка программы
            System.Windows.Forms.Application.Restart();
        }

        /// <summary>
        /// Метод закрытия окон
        /// </summary>
        /// <param name="WindowName"></param>
        public static void CloseWindow(string WindowName)
        {
            // Закрытие окна ProgBar
            foreach (Window t in Application.Current.Windows)
            {
                if (t.Name.ToString() == $"{WindowName}")
                {
                    t.Close();
                }
            }
        }
    }
}
