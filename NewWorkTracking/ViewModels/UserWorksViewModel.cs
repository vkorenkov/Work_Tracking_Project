using Microsoft.AspNetCore.SignalR.Client;
using NewWorkTracking.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WorkTrackingLib;
using WorkTrackingLib.Models;
using WorkTrackingLib.ProcessClasses;

namespace NewWorkTracking.ViewModels
{
    class UserWorksViewModel : AbstractViewModel
    {
        private NewWrite newWork;
        public NewWrite NewWork
        {
            get => newWork;
            set { newWork = value; OnPropertyChanged(nameof(NewWork)); }
        }

        private bool noPcNameCheck;
        public bool NoPcNameCheck
        {
            get => noPcNameCheck;
            set
            {
                noPcNameCheck = value; OnPropertyChanged(nameof(NoPcNameCheck));
                if (NoPcNameCheck) NewWork.OldPCName = "-";
                else NewWork.OldPCName = null;
            }
        }

        private bool noNewInvNumCheck;
        public bool NoNewInvNumCheck
        {
            get => noNewInvNumCheck;
            set
            {
                noNewInvNumCheck = value; OnPropertyChanged(nameof(NoNewInvNumCheck));
                if (NoNewInvNumCheck)
                    NewWork.NewInv = NewWork.OldInv;
                else NewWork.NewInv = null;
            }
        }

        public ICommand AddWork => new RelayCommand<object>(obj =>
        {
            if (!string.IsNullOrEmpty(MainObject.Access.Name))
                NewWork.Who = MainObject.Access.Name;
            else
                NewWork.Who = "Unknown_User";

            var tempListProp = ChechForEmpty(NewWork);

            if (tempListProp.Count <= 0)
            {
                // Запись объекта в БД
                ConnectionClass.hubConnection.InvokeAsync("RunAddNewWork", NewWork);

                UsersWorks.AddNewItem(NewWork);
            }
            else
            {
                string field = string.Empty;

                field = tempListProp.Count > 1 ? field = "поля" : field = "поле";

                // Вывод всех незаполненных свойств
                Message.Show("Ошибка", $"Заполните {field} {GetNullProperties(tempListProp)}", MessageBoxButton.OK);
            }

            if (NewWork.Results == "Гарантийный ремонт" || NewWork.Results == "Платный ремонт")
            {
                SetNewRepair(NewWork);
            }
        });

        /// <summary>
        /// Команда добавления тестовых работ
        /// </summary>
        public ICommand AddManyWork => new RelayCommand<object>(obj =>
        {
            NewWork.Who = MainObject.Access.Name;

            Task.Run(async () =>
            {
                for (int i = 0; i < 100; i++)
                {
                    NewWork.OspOrder = $"test-{i}";
                    await ConnectionClass.hubConnection.InvokeAsync("RunAddNewWork", NewWork);
                }
            });
        });

        public UserWorksViewModel(MainObject mainObject)
        {
            SignalRActions();
            ActiveVisibility = Visibility.Visible;
            MainObject = mainObject;
            UsersWorks = new ListCollectionView(MainObject.AdminWorks.Where(x => x.Who == MainObject.Access.Name).ToList());
            NewWork = new NewWrite();
            NewWork.Date = DateTime.Today;
        }

        protected override void SignalRActions()
        {
            ConnectionClass.hubConnection.On<bool>("AddedError", (result) =>
            {
                if (!result)
                {
                    Message.Show("Ошибка", "Ошибка записи работы", MessageBoxButton.OK);
                }
            });
        }

        /// <summary>
        /// Метод проверяет значения свойств нового объекта заявки на null
        /// </summary>
        /// <param name="newWrite"></param>
        /// <returns></returns>
        private List<PropertyInfo> ChechForEmpty(NewWrite newWrite)
        {
            // Пустая коллекция для свойств
            List<PropertyInfo> tempProp = new List<PropertyInfo>();

            // Цикл просмотра значения свойств для определение пустых значений
            foreach (var t in newWrite.GetType().GetProperties())
            {
                // Условие исключения некоторорых свойств из проверки
                if (t.Name != "Id" || t.Name != "WriteOffNum" || t.Name != "ModernNum" || t.Name != "ModernNewPc")
                {
                    // Условие при котором свойство имеющее значение null или string.Empty помещается во временную коллекцию
                    if (t.GetValue(newWrite) == null || t.GetValue(newWrite).ToString() == string.Empty)
                    {
                        tempProp.Add(t);
                    }
                }
            }

            return tempProp;
        }

        /// <summary>
        /// Метод возвращает строку с аттрибутами свойств
        /// </summary>
        /// <param name="tempListProp"></param>
        /// <returns></returns>
        private string GetNullProperties(List<PropertyInfo> tempListProp)
        {
            // Переменная незаполненых полей
            string noFill = string.Empty;

            // Цикл получения свойст и и атрибутов свойств
            for (var i = 0; i < tempListProp.Count; i++)
            {
                // Условие извлекает пользовательские атрибуты свойств если они не пустые
                if (tempListProp[i].GetCustomAttribute(typeof(DisplayAttribute)) != null)
                {
                    noFill = tempListProp.IndexOf(tempListProp[i]) == 0 ?
                        noFill = noFill + $@": " : noFill = noFill + $@", ";

                    noFill = noFill + $@"""{(tempListProp[i].GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute).Name}""";
                }
            }

            return noFill;
        }

        private void SetNewRepair(NewWrite newWork)
        {
            ConnectionClass.hubConnection.InvokeAsync("RunAddRepair", new RepairClass()
            {
                Date = DateTime.Now,
                DiagNumber = newWork.OrderNum,
                InvNumber = newWork.OldInv,
                OsName = newWork.OsType,
                Defect = newWork.Pass,
                Warranty = newWork.Results
            });
        }
    }
}
