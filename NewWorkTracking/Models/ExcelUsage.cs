using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WorkTrackingLib;
using WorkTrackingLib.Models;

namespace NewWorkTracking.Models
{
    class ExcelUsage
    {
        /// <summary>
        /// Метод выгрузки данных в файл Excel
        /// </summary>
        public static bool GetExcel(List<NewWrite> temp, string path)
        {
            // Создание новой книги Exel
            var wb = new XLWorkbook();

            // Инициализация интерфейса добавления данных в файл Exel
            var ws = wb.Worksheets.Add("Data");

            try
            {
                // Условие для сохранения файла
                if (path != null)
                {
                    ws.Row(1).Delete();

                    // Запись данных в файл Exel
                    ws.Cell(1, 1).InsertTable(temp);

                    // Удаление колонок с ненужной пользователю информацией
                    ws.Column("V").Delete();
                    ws.Column("U").Delete();
                    ws.Column("T").Delete();
                    ws.Column("A").Delete();

                    // Получение свойств объекта для извлечения атрибутов
                    var t = temp[0].GetType().GetProperties().ToList();

                    // Цикл получения свойст и установки заголовков столбцов
                    for (var i = 1; i < t.Count; i++)
                    {
                        // Условие извлекает пользовательские атрибуты свойств если они не пустые
                        if (t[i].GetCustomAttribute(typeof(DisplayAttribute)) != null)
                        {
                            // Запись в ячейки заголовков извлеченных атрибутов свойст
                            ws.Cell(1, i).SetValue((t[i].GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute).Name);
                        }
                    }

                    // Установка ширины ячейки в соответствии с шириной текста
                    ws.Columns().AdjustToContents();

                    // Отклучение автофильтра по умолчанию
                    ws.RangeUsed().SetAutoFilter(false);

                    // Сохранение файла
                    wb.SaveAs(path);

                    // Указание результата выполнение действий с файлом при удачном сохранении
                    return true;
                }
                else
                {
                    // Указание результата выполнение действий с файлом при отмене сохранения
                    return false;
                }
            }
            catch
            {
                //Message.Show("Ошибка", ex.Message, MessageBoxButton.OK);
                // Указание результата выполнение действий с файлом при ошибке сохранения
                return false;
            }
        }

        /// <summary>
        /// Метод считывает файл Excel и загружает в память для последующей записи данных в БД
        /// </summary>
        /// <param name="path"></param>
        /// <param name="emptyCol"></param>
        /// <returns></returns>
        public string LoadExcel(string path, List<RepairClass> emptyCol)
        {
            if (!string.IsNullOrEmpty(path))
            {
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
                {
                    // Создание книги Excel
                    using (var workbook = new XLWorkbook(ms))
                    {
                        // Создание таблицы
                        var worksheet = workbook.Worksheet(1);

                        try
                        {
                            // Формирование строк таблицы
                            var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                            // Цикл создания объектов для добавления в коллекцию
                            foreach (var t in rows)
                            {
                                RepairClass repairClass = new RepairClass()
                                {
                                    Id = 0,
                                    DeviceId = 0,
                                    Date = t.Cell(1).Value.GetType() != typeof(DateTime) ? new DateTime?() : Convert.ToDateTime(t.Cell(1).Value),
                                    Status = t.Cell(2).Value.ToString(),
                                    OsName = t.Cell(3).Value.ToString(),
                                    Model = t.Cell(4).Value.ToString(),
                                    SNumber = t.Cell(5).Value.ToString(),
                                    InvNumber = t.Cell(6).Value.ToString(),
                                    ScOks = t.Cell(7).Value.ToString(),
                                    DiagNumber = t.Cell(8).Value.ToString(),
                                    KaProvider = t.Cell(9).Value.ToString(),
                                    KaRepair = t.Cell(10).Value.ToString(),
                                    HandedOver = t.Cell(11).Value.ToString(),
                                    Defect = t.Cell(12).Value.ToString(),
                                    ShipmentDate = t.Cell(13).Value.GetType() != typeof(DateTime) ? new DateTime?() : Convert.ToDateTime(t.Cell(13).Value),
                                    DaysOfRepair = 0,
                                    ReturnFromRepair = t.Cell(14).Value.GetType() != typeof(DateTime) ? new DateTime?() : Convert.ToDateTime(t.Cell(14).Value),
                                    ProviderOrder = t.Cell(15).Value.ToString(),
                                    RepairBill = t.Cell(16).Value.ToString(),
                                    WarrantyBasis = t.Cell(17).Value.ToString(),
                                    StartWarranty = t.Cell(18).Value.GetType() != typeof(DateTime) ? new DateTime?() : Convert.ToDateTime(t.Cell(18).Value),
                                    Warranty = t.Cell(19).Value.ToString()
                                };

                                emptyCol.Add(repairClass);
                            }

                            return "Файл считан.";
                        }
                        catch (Exception e)
                        {
                            return e.Message;
                        }
                    }
                }
            }
            else
            {
                return null;
            }
        }
    }
}
