using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WorkTracking_Server.Context;
using WorkTrackingLib;
using WorkTrackingLib.Models;

namespace WorkTracking_Server.Sql
{
    public class FromSql
    {
        private DataContext dataContext;

        public FromSql(DataContext context)
        {
            dataContext = context;
        }

        public ObservableCollection<NewWrite> GetWorks()
        {
            return new ObservableCollection<NewWrite>(dataContext.ComplitedWorks.OrderByDescending(x => x.Date));
        }

        public ObservableCollection<NewWrite> GetWorks(string userName)
        {
            return new ObservableCollection<NewWrite>(dataContext.ComplitedWorks.Where(x => x.Who == userName && x.Date <= DateTime.Today && x.Date >= DateTime.Now.AddMonths(-1)).OrderByDescending(x => x.Date));
        }

        public ObservableCollection<Devices> GetDevices()
        {
            ObservableCollection<Devices> tempDev = new ObservableCollection<Devices>(dataContext.Devices);
            ObservableCollection<RepairClass> tempRep = new ObservableCollection<RepairClass>(GetRepairs());

            foreach (var d in tempDev)
            {
                d.Repairs = tempRep.Where(x => x.DeviceId == d.Id).ToList();
            }

            return new ObservableCollection<Devices>(tempDev.OrderBy(x => x.InvNumber));
        }

        public ObservableCollection<RepairClass> GetRepairs()
        {
            return new ObservableCollection<RepairClass>(dataContext.Repairs);
        }

        /// <summary>
        /// Метод получает список всех пользователей, содержащихся в таблице пользователей БД
        /// </summary>
        /// <returns></returns>
        public AccessModel GetAdmins(string userName)
        {
            var tempUser = dataContext.Admins.Where(x => x.Name == userName).FirstOrDefault();

            if (tempUser != null)
            {
                return new AccessModel() { Id = tempUser.Id, Name = tempUser.Name, Access = tempUser.Access };
            }
            else
            {
                return null;
            }
        }

        public ComboboxDataSource GetComboboxes()
        {            
            ComboboxDataSource comboboxData = new ComboboxDataSource(
                dataContext.Admins.Select(x => x.Name).ToList(),
                dataContext.Osp.ToList(),
                dataContext.OsType.ToList(),
                dataContext.Results.ToList(),
                dataContext.Why.ToList(),
                dataContext.ScOks.ToList(),
                dataContext.RepairsStatuses.ToList(),
                new List<Admins>(dataContext.Admins));

            return comboboxData;
        }
    }
}
