using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackingLib
{
    [Table("Complited_Work")]
    public class NewWrite : PropertyChangeClass, ICloneable
    {
        #region Свойства

        [Display(Name = "Id")]
        /// <summary>
        /// Свойство идентификатора в БД
        /// </summary>
        public int Id { get; set; }

        private DateTime date;
        [Display(Name = "Дата")]
        /// <summary>
        /// Свойство заполнения даты и времени
        /// </summary>
        public DateTime Date
        {
            get { return date; }
            set { date = value; OnPropertyChanged(nameof(Date)); }
        }

        private string who;
        [Display(Name = "Системный администратор")]
        /// <summary>
        /// Свойство заполнения системного админитсратора
        /// </summary>
        public string Who
        {
            get { return who; }
            set { who = value; OnPropertyChanged(nameof(Who)); }
        }

        private string scOks;
        [Display(Name = "Сц Окс")]
        /// <summary>
        /// Свойство заполнения Сервисного центра ОКС
        /// </summary>
        public string ScOks
        {
            get { return scOks; }
            set { scOks = value; OnPropertyChanged(nameof(ScOks)); }
        }

        private string ospWork;
        [Display(Name = "ОСП Выполнения работ")]
        /// <summary>
        /// Свойство заполнения ОСП
        /// </summary>
        public string OspWork
        {
            get { return ospWork; }
            set { ospWork = value; OnPropertyChanged(nameof(OspWork)); }
        }

        private string ospOrder;
        [Display(Name = "ОСП заказчика")]
        /// <summary>
        /// Свойство заполнения осп заказчика
        /// </summary>
        public string OspOrder
        {
            get { return ospOrder; }
            set { ospOrder = value; OnPropertyChanged(nameof(OspOrder)); }
        }

        private string orderType;
        [Display(Name = "Тип заявки")]
        /// <summary>
        /// Свойство заполнения вида заявки
        /// </summary>
        public string OrderType
        {
            get { return orderType; }
            set { orderType = value; OnPropertyChanged(nameof(OrderType)); }
        }

        private string orderNum;
        [Display(Name = "Номер заявки")]
        /// <summary>
        /// Свойство заполнения номера заявки
        /// </summary>
        public string OrderNum
        {
            get { return orderNum; }
            set { orderNum = value; OnPropertyChanged(nameof(OrderNum)); }
        }

        private string oldInv;
        [Display(Name = "Инвентарный номер снятого")]
        /// <summary>
        /// Свойство заполнения инвентарного номера снятого 
        /// </summary>
        public string OldInv
        {
            get { return oldInv; }
            set { oldInv = value; OnPropertyChanged(nameof(OldInv));  }
        }

        private string newInv;
        [Display(Name = "Инвентарный номер нового")]
        /// <summary>
        /// Свойство заполнения нового инвентарного номера
        /// </summary>
        public string NewInv
        {
            get { return newInv; }
            set { newInv = value; OnPropertyChanged(nameof(NewInv)); }
        }

        private string oldPCName;
        [Display(Name = "Старое имя ПК")]
        /// <summary>
        /// Свойство заполнения старого имени ПК
        /// </summary>
        public string OldPCName
        {
            get { return oldPCName; }
            set { oldPCName = value; OnPropertyChanged(nameof(OldPCName)); }
        }

        private string osType;
        [Display(Name = "Тип ОС")]
        /// <summary>
        /// Свойство заполнения типа ОС
        /// </summary>
        public string OsType
        {
            get { return osType; }
            set { osType = value; OnPropertyChanged(nameof(OsType)); }
        }

        private string why;
        [Display(Name = "Причина обращения")]
        /// <summary>
        /// Свойство заполнения причины
        /// </summary>
        public string Why
        {
            get { return why; }
            set { why = value; OnPropertyChanged(nameof(Why)); }
        }

        private string results;
        [Display(Name = "Результат работ")]
        /// <summary>
        /// Свойство заполнения результата
        /// </summary>
        public string Results
        {
            get { return results; }
            set { results = value; OnPropertyChanged(nameof(Results)); }
        }

        private string brocken;
        [Display(Name = "Заявленная неисправность")]
        /// <summary>
        /// Свойство заполнения неисправности
        /// </summary>
        public string Brocken
        {
            get { return brocken; }
            set { brocken = value; OnPropertyChanged(nameof(Brocken)); }
        }

        private string pass;
        [Display(Name = "Выполнение")]
        /// <summary>
        /// Свойство заполнения выполненной работы
        /// </summary>
        public string Pass
        {
            get { return pass; }
            set { pass = value; OnPropertyChanged(nameof(Pass)); }
        }

        private string noActive;
        /// <summary>
        /// Свойство состояния заявки
        /// </summary>
        [Display(Name = "Состояние")]
        public string NoActive
        {
            get { return noActive; }

            set { noActive = value; OnPropertyChanged(nameof(NoActive)); }
        }

        private string writeOffNum;
        /// <summary>
        /// Свойство номера списания
        /// </summary>
        [Display(Name = "Номер списания")]
        public string WriteOffNum
        {
            get { return writeOffNum; }

            set
            {
                writeOffNum = value;
                OnPropertyChanged(nameof(WriteOffNum));

                if (string.IsNullOrEmpty(WriteOffNum))
                {
                    VisModernNum = true;
                    VisModernNewPc = true;
                    WriteOffNum = "нет";
                }
                else if(!string.IsNullOrEmpty(value) && value != "нет")
                {
                    VisModernNum = false;
                    VisModernNewPc = false;
                    NoActive = "Списано";
                }
            }
        }

        private string modernNum;
        /// <summary>
        /// Свойство номера модернизации
        /// </summary>
        [Display(Name = "Номер модернизации")]
        public string ModernNum
        {
            get { return modernNum; }

            set
            {
                modernNum = value;
                OnPropertyChanged(nameof(ModernNum));

                if (string.IsNullOrEmpty(ModernNum))
                {
                    VisWriteOff = true;
                    VisModernNewPc = true;
                    ModernNum = "нет";
                }
                else if (!string.IsNullOrEmpty(value) && value != "нет")
                {
                    VisWriteOff = false;
                    VisModernNewPc = false;
                    NoActive = "Модернизировано";
                }
            }
        }

        private string modernNewPc;
        /// <summary>
        /// Свойство номера модернизации нового ПК
        /// </summary>
        [Display(Name = "Номер модернизации нового ПК")]
        public string ModernNewPc
        {
            get { return modernNewPc; }

            set
            {
                modernNewPc = value;
                OnPropertyChanged(nameof(ModernNewPc));

                if (string.IsNullOrEmpty(ModernNewPc))
                {
                    VisWriteOff = true;
                    VisModernNum = true;
                    ModernNewPc = "нет";
                }
                else if (!string.IsNullOrEmpty(value) && value != "нет")
                {
                    VisWriteOff = false;
                    VisModernNum = false;
                    NoActive = "Модернизировано";
                }
            }
        }
      
        private bool visWriteOff = true;
        [NotMapped]
        /// <summary>
        /// Свойство возможности редактирования записи номера списания
        /// </summary>
        public bool VisWriteOff
        {
            get { return visWriteOff; }
            set { visWriteOff = value; OnPropertyChanged(nameof(VisWriteOff)); }
        }

        private bool visModernNum = true;
        [NotMapped]
        /// <summary>
        /// Свойство возможности редактирования записи номера модернизации
        /// </summary>
        public bool VisModernNum
        {
            get { return visModernNum; }
            set { visModernNum = value; OnPropertyChanged(nameof(VisModernNum)); }
        }

        private bool visModernNewPc = true;
        [NotMapped]
        /// <summary>
        /// Свойство возможности редактирования записи номера модернизации нового ПК
        /// </summary>
        public bool VisModernNewPc
        {
            get { return visModernNewPc; }
            set { visModernNewPc = value; OnPropertyChanged(nameof(VisModernNewPc)); }
        }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Пустой конструктор
        /// </summary>
        public NewWrite()
        {
            NoActive = "Активно";
            WriteOffNum = "нет";
            ModernNum = "нет";
            ModernNewPc = "нет";
        }

        ///// <summary>
        ///// Конструктор принимает все введенные данные
        ///// </summary>
        ///// <param name="Osp"></param>
        ///// <param name="OspOrder"></param>
        ///// <param name="CatOrder"></param>
        ///// <param name="DateTime"></param>
        ///// <param name="NumOrder"></param>
        ///// <param name="NumInv"></param>
        ///// <param name="OldName"></param>
        ///// <param name="NewInv"></param>
        ///// <param name="OsType"></param>
        ///// <param name="Why"></param>
        ///// <param name="Resut"></param>
        ///// <param name="Brocken"></param>
        ///// <param name="Pass"></param>
        //public NewWrite( int Id, DateTime Date, string Who,
        //    string OspWork, 
        //    string OspOrder, 
        //    string OrderType,
        //    string OrderNum,
        //    string OldInv,
        //    string NewInv,
        //    string OldPCName,
        //    string OsType,
        //    string Why,
        //    string Results,
        //    string Brocken,
        //    string Pass,
        //    string NoActive,
        //    string WriteOffNum,
        //    string ModernNum,
        //    string ModernNewPc
        //    )
        //{
        //    this.Id = Id;
        //    this.Date = Date;
        //    this.Who = Who;
        //    this.OspWork = OspWork;
        //    this.OspOrder = OspOrder;
        //    this.OrderType = OrderType;
        //    this.OrderNum = OrderNum;
        //    this.OldInv = OldInv;
        //    this.NewInv = NewInv;
        //    this.OldPCName = OldPCName;
        //    this.OsType = OsType;
        //    this.Why = Why;
        //    this.Results = Results;
        //    this.Brocken = Brocken;
        //    this.Pass = Pass;
        //    this.NoActive = NoActive;
        //    this.WriteOffNum = WriteOffNum;
        //    this.ModernNum = ModernNum;
        //    this.ModernNewPc = ModernNewPc;
        //}

        #endregion

        #region Методы         

        /// <summary>
        /// Метод реализует интерфейс клонирования объекта
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
