using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewWorkTracking.Models
{
    class AdUsage
    {
        public delegate void UserSearch(string m);
        public event UserSearch UserSearchEvent;

        DirectoryEntry entry;
        DirectorySearcher searcher;

        public AdUsage()
        {
            entry = new DirectoryEntry("");
            searcher = new DirectorySearcher(entry);
        }

        public ObservableCollection<string> GetAdUsers(string searchObject, string searchName)
        {
            UserSearchEvent?.Invoke($@"Поиск...");

            var userList = new ObservableCollection<string>();

            searcher.Filter = $"(&(objectClass={searchObject})(Name=*{searchName}*))";

            try
            {
                foreach (SearchResult result in searcher.FindAll())
                {
                    var name = result.GetDirectoryEntry().Properties["DisplayName"].Value;

                    if (name != null)
                    {
                        Application.Current.Dispatcher.Invoke(() => userList.Add(name.ToString()));
                    }

                    UserSearchEvent?.Invoke($@"Найдено...{userList.Count}");
                }
            }
            catch (NotSupportedException e)
            {
                throw new NotSupportedException(e.Message);
            }

            userList = new ObservableCollection<string>(userList.OrderBy(x => x));

            UserSearchEvent?.Invoke($@"Найдено {userList.Count} совпадений. Выберите из списка.");

            return userList;
        }
    }
}
