using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WorkTracking_Server.Context;
using WorkTracking_Server.Extentions;
using WorkTracking_Server.Models;
using System.Text.Json;
using Newtonsoft.Json;
using WorkTracking_Server.Sql;
using WorkTrackingLib;
using WorkTrackingLib.Interfaces;
using WorkTrackingLib.Models;

namespace WorkTracking_Server.Hubs
{
    public class SqlHub : Hub
    {
        private FromSql fromSql;

        private ToSql toSql;

        public SqlHub(DataContext context)
        {
            fromSql = new FromSql(context);

            toSql = new ToSql(context);
        }

        /// <summary>
        /// Метод проверки соединения при изменении адреса сервера
        /// </summary>
        /// <returns></returns>
        public async Task TestConnection()
        {
            await Clients.Caller.SendAsync("TestConnection", true);
        }

        public async Task RunAddNewWork(NewWrite newWork)
        {
            var result = toSql.AddWork(newWork);

            result.Wait();

            if (result.Result)
                await Clients.All.SendAsync("UpdateWorks", newWork);
            else
                await Clients.Caller.SendAsync("AddedError", result.Result);        
        }

        public async Task RunAddRepair(RepairClass repair)
        {
            bool isDevice = false;

            DataContext dataContext = new DataContext();

            foreach (var d in dataContext.Devices)
            {
                if (d.InvNumber == repair.InvNumber)
                {
                    repair.DeviceId = d.Id;

                    var tempBool = toSql.AddRepair(repair).Result;

                    if (tempBool)
                    {
                        isDevice = true;

                        await Clients.All.SendAsync("UpdateRepairs", repair);
                    }
                    else
                    {
                        await Clients.Caller.SendAsync("UpdateRepairFaled", false);
                    }

                    break;
                }
            }

            if (!isDevice)
            {
                var device = new Devices() {DeviceName = repair.Model, InvNumber = repair.InvNumber, OsName = repair.OsName };

                var tempBool = toSql.AddDevice(device).Result;

                if (tempBool)
                {
                    repair.DeviceId = dataContext.Devices.Where(x => x.InvNumber == repair.InvNumber).FirstOrDefault().Id;

                    var tempBoolRepair = toSql.AddRepair(repair).Result;

                    if (tempBoolRepair)
                    {
                        device.Repairs = new List<RepairClass>(dataContext.Repairs.Where(x => x.InvNumber == device.InvNumber).ToList());

                        await Clients.All.SendAsync("UpdateRepairs", repair);

                        await Clients.All.SendAsync("UpdateDevices", device);
                    }
                    else
                    {
                        await Clients.Caller.SendAsync("UpdateRepairFaled", false);
                    }
                }
                else
                {
                    await Clients.Caller.SendAsync("UpdateDeviceFaled", false);
                }
            }
        }

        public async Task RunAddDevice(Devices device)
        {
            var result = toSql.AddDevice(device);

            result.Wait();

            if (result.Result)
                await Clients.All.SendAsync("UpdateDevices", device);
            else
                await Clients.Caller.SendAsync("DeviceNotAdded", result.Result);
        }

        public async Task RunAddNewUser(Admins user)
        {
            var result = toSql.AddNewUser(user);

            result.Wait();

            if (result.Result)
                await Clients.All.SendAsync("UpdateUsers", user);
            else
                await Clients.Caller.SendAsync("UserNotAdded", result.Result);
        }

        public async Task RunAddNewItem(object newItem, string table)
        {
            var result = toSql.AddNewItem(newItem, table);

            result.Wait();

            await UpdateItems(result.Result);
        }

        public async Task RunDelObject(object delItem, string table)
        {
            var result = toSql.DelItem(delItem, table);

            await UpdateItems(result.Result);
        }

        private async Task UpdateItems(bool result)
        {
            if (result)
                await Clients.All.SendAsync("UpdateItem", fromSql.GetComboboxes());
            else
                await Clients.Caller.SendAsync("ItemNotAdded", result);
        }

        public async Task RunCheckAccess(string userName)
        {
            Console.WriteLine(userName);

            AccessModel accessModel = fromSql.GetAdmins(userName);

            if (accessModel != null)
            {
                accessModel.ChangeVisibility();

                await RunGetAll(accessModel);
            }
            else
            {
                await Clients.Caller.SendAsync("AccessDenide", false);
            }
        }

        public async Task RunGetAll(AccessModel accessModel)
        {
            if (accessModel.Access == 0)
            {
                await Clients.Caller.SendAsync("GiveAll", new MainObject(accessModel, fromSql.GetWorks(accessModel.Name), fromSql.GetDevices(), fromSql.GetRepairs(), fromSql.GetComboboxes()));
            }
            else
            {
                await Clients.Caller.SendAsync("GiveAll", new MainObject(accessModel, fromSql.GetWorks(), fromSql.GetDevices(), fromSql.GetRepairs(), fromSql.GetComboboxes()));
            }
        }

        public async Task UpdateAll(AccessModel accessModel)
        {
            await Clients.Caller.SendAsync("UpdateRequest", new MainObject(accessModel, fromSql.GetWorks(), fromSql.GetDevices(), fromSql.GetRepairs(), fromSql.GetComboboxes()));
        }
     
        public async Task StartChangeWork(NewWrite mutableWork)
        {
            var result = toSql.ChangeWork(mutableWork);

            result.Wait();

            if (result.Result)
            {
                await Clients.All.SendAsync("ChangedWork", mutableWork);
            }
            else
            {
                await Clients.Caller.SendAsync("ChangedError", result.Result);
            }
        }

        public async Task StartChangeRepair(RepairClass mutalbeRepair)
        {
            var result = toSql.ChangeRepair(mutalbeRepair);

            result.Wait();

            if (result.Result)
            {
                await Clients.All.SendAsync("ChangedRepair", mutalbeRepair);
            }
            else
            {
                await Clients.Caller.SendAsync("ChangedError", result.Result);
            }
        }

        public async Task StartChangeDevice(Devices mutableDevice)
        {
            var result = toSql.ChangeDevice(mutableDevice);

            result.Wait();

            if (result.Result)
            {
                await Clients.All.SendAsync("ChangedDevice", mutableDevice);
            }
            else
            {
                await Clients.Caller.SendAsync("DeviceChangedError", result.Result);
            }
        }

        public async Task StartChangeUser(Admins mutableUser)
        {
            var result = toSql.ChangeUser(mutableUser);

            result.Wait();

            await UpdateItems(result.Result);

            AccessModel accessModel = fromSql.GetAdmins(mutableUser.Name).ChangeVisibility();

            await Clients.All.SendAsync("ChangeAccess", accessModel);
        }

        public async Task StartChangeItem(object mutableItem, string table)
        {
            var result = toSql.ChangeItem(mutableItem, table);

            result.Wait();

            await UpdateItems(result.Result);
        }
    }
}
