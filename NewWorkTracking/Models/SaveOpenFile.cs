using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewWorkTracking.Models
{
    class SaveOpenFile
    {
        /// <summary>
        /// Метод вызова диолога сохранения файла
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string SaveDialog(string fileName)
        {
            // Инициализация диалогового окна сохранения файла
            SaveFileDialog sfd = new SaveFileDialog();

            // Фильт расширений файлов диалогового окна сохранения файла
            sfd.Filter = "Файл Excel 2007+ (*.xlsx)|*.xlsx|Файл Exel 2003 (*.xls)|*.xls";

            sfd.FileName = fileName;

            // Запуск диалогового окна сохранения файла
            if (sfd.ShowDialog() == true)
            {
                return sfd.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Метод открывает диалоговое окно выбора файла и возвращает полный путь к файлу
        /// </summary>
        /// <returns></returns>
        public string OpenDialog()
        {
            // Инициализация класса выбора файла
            OpenFileDialog opn = new OpenFileDialog();

            // Фильтр расширений файлов
            opn.Filter = "Файл Excel 2007+ (*.xlsx)|*.xlsx|Файл Excel 2003 (*.xls)|*.xls";

            // условие нормальной отработки диалогового окна
            if (opn.ShowDialog() == true)
            {
                return opn.FileName;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
