using Microsoft.AspNetCore.SignalR.Client;
using NewWorkTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WorkTrackingLib.Models;
using WorkTrackingLib.ProcessClasses;

namespace NewWorkTracking.ViewModels
{
    class RepairCardViewModel
    {
        public RepairClass Repair { get; set; }

        public MainObject MainObject { get; set; }

        public ICommand AddNewRepair => new RelayCommand<object>(obj => ConnectionClass.hubConnection.InvokeAsync("RunAddRepair", Repair));

        public ICommand CloseCard => new RelayCommand<object>(obj => Application.Current.Windows.CloseWindow("RepairCardWindow"));

        public RepairCardViewModel(MainObject mainObject)
        {
            Repair = new RepairClass();

            MainObject = mainObject;
        }

        public RepairCardViewModel(MainObject mainObject, Devices device)
        {
            Repair = new RepairClass() 
            { 
                Model = device.DeviceName, 
                InvNumber = device.InvNumber, 
                DeviceId = device.Id,
                OsName = device.OsName,
            };

            if (device.Repairs.Count != 0)
                Repair.SNumber = device.Repairs[0].SNumber;

            MainObject = mainObject;
        }
    }
}
