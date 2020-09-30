using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WorkTrackingLib.Models
{
    public class RepairClass : PropertyChangeClass, ICloneable
    {
        private int id = 0;
        public int Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged(nameof(Id)); }
        }

        private int deviceId;
        public int DeviceId
        {
            get { return deviceId; }
            set { deviceId = value; OnPropertyChanged(nameof(DeviceId)); }
        }

        private DateTime? date;
        public DateTime? Date
        {
            get { return date; }
            set { date = value; OnPropertyChanged(nameof(Date)); }
        }

        private string status = string.Empty;
        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged(nameof(Status)); }
        }

        private string osName = string.Empty;
        public string OsName
        {
            get { return osName; }
            set { osName = value; OnPropertyChanged(nameof(OsName)); }
        }

        private string model = string.Empty;
        public string Model
        {
            get { return model; }
            set { model = value; OnPropertyChanged(nameof(Model)); }
        }

        private string sNumber = string.Empty;
        public string SNumber
        {
            get { return sNumber; }
            set { sNumber = value; OnPropertyChanged(nameof(SNumber)); }
        }

        private string invNumber = string.Empty;
        public string InvNumber
        {
            get { return invNumber; }
            set { invNumber = value; OnPropertyChanged(nameof(InvNumber)); }
        }

        private string diagNumber = string.Empty;
        public string DiagNumber
        {
            get { return diagNumber; }
            set { diagNumber = value; OnPropertyChanged(nameof(DiagNumber)); }
        }

        private string scOks = string.Empty;
        public string ScOks
        {
            get => scOks;
            set { scOks = value; OnPropertyChanged(nameof(ScOks)); }
        }

        private string kaProvider = string.Empty;
        public string KaProvider
        {
            get { return kaProvider; }
            set { kaProvider = value; OnPropertyChanged(nameof(KaProvider)); }
        }

        private string kaRepair = string.Empty;
        public string KaRepair
        {
            get { return kaRepair; }
            set { kaRepair = value; OnPropertyChanged(nameof(KaRepair)); }
        }

        private string handedOver = string.Empty;
        public string HandedOver
        {
            get { return handedOver; }
            set { handedOver = value; OnPropertyChanged(nameof(HandedOver)); }
        }

        private string defect = string.Empty;
        public string Defect
        {
            get { return defect; }
            set { defect = value; OnPropertyChanged(nameof(Defect)); }
        }

        private DateTime? shipmentDate;
        public DateTime? ShipmentDate
        {
            get { return shipmentDate; }
            set { shipmentDate = value; OnPropertyChanged(nameof(ShipmentDate)); ChangeDaysOfRepair(); }
        }

        private int daysOfRepair;
        public int DaysOfRepair
        {
            get { return daysOfRepair; }
            set { daysOfRepair = value; OnPropertyChanged(nameof(DaysOfRepair)); }
        }

        private DateTime? returnFromRepair;
        public DateTime? ReturnFromRepair
        {
            get { return returnFromRepair; }
            set { returnFromRepair = value; OnPropertyChanged(nameof(ReturnFromRepair)); ChangeDaysOfRepair(); }
        }

        private string providerOrder = string.Empty;
        public string ProviderOrder
        {
            get { return providerOrder; }
            set { providerOrder = value; OnPropertyChanged(nameof(ProviderOrder)); }
        }

        private string repairBill = string.Empty;
        public string RepairBill
        {
            get { return repairBill; }
            set { repairBill = value; OnPropertyChanged(nameof(RepairBill)); }
        }

        private string warrantyBasis = string.Empty;
        public string WarrantyBasis
        {
            get { return warrantyBasis; }
            set { warrantyBasis = value; OnPropertyChanged(nameof(WarrantyBasis)); }
        }

        private DateTime? startWarranty;
        public DateTime? StartWarranty
        {
            get { return startWarranty; }
            set { startWarranty = value; OnPropertyChanged(nameof(StartWarranty)); }
        }

        private string warranty = string.Empty;
        public string Warranty
        {
            get { return warranty; }
            set { warranty = value; OnPropertyChanged(nameof(Warranty)); }
        }

        private void ChangeDaysOfRepair()
        {
            if (ShipmentDate != null && ReturnFromRepair == null)
            {
                TimeSpan time = DateTime.Now - Convert.ToDateTime(ShipmentDate);

                DaysOfRepair = time.Days;
            }

            if (ShipmentDate != null && ReturnFromRepair != null)
            {
                TimeSpan time = Convert.ToDateTime(ReturnFromRepair) - Convert.ToDateTime(ShipmentDate);

                DaysOfRepair = time.Days;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
