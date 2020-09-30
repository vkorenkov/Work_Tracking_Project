using NewWorkTracking.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WorkTrackingLib;
using WorkTrackingLib.Models;
using System.Windows.Threading;
using System.Windows.Input;
using WorkTrackingLib.ProcessClasses;
using System.Drawing;

namespace NewWorkTracking.ViewModels
{
    class RepairsViewModel : AbstractViewModel
    {
        private RepairCardDataContext repairContext;
        public RepairCardDataContext RepairContext
        {
            get => repairContext;
            set { repairContext = value; OnPropertyChanged(nameof(RepairContext)); }
        }

        private List<string> filterModelList;
        public List<string> FilterModelList
        {
            get => filterModelList;
            set { filterModelList = value; OnPropertyChanged(nameof(FilterModelList)); }
        }

        private List<string> filterProviderList;
        public List<string> FilterProviderList
        {
            get => filterProviderList;
            set { filterProviderList = value; OnPropertyChanged(nameof(FilterProviderList)); }
        }

        private List<string> filterRepairTypeList;
        public List<string> FilterRepairTypeList
        {
            get => filterRepairTypeList;
            set { filterRepairTypeList = value; OnPropertyChanged(nameof(FilterRepairTypeList)); }
        }

        private RepairClass selectedRepair;
        public RepairClass SelectedRepair
        {
            get => selectedRepair;
            set { selectedRepair = value; OnPropertyChanged(nameof(SelectedRepair)); }
        }

        private Dispatcher dispatcher;

        public HideColumns HideColumns { get; set; }

        public ICommand OpenRepairCard => new RelayCommand<object>(obj =>
        {
            Actively = typeof(RepairCardDataContext).ToString();

            RepairContext.SelectedItem = SelectedRepair;

            RepairContext.Combobox = MainObject.ComboBox;

            RepairContext.RepairCardVis = Visibility.Visible;

            ActiveVisibility = Visibility.Collapsed;
        });

        public ICommand ChangeRepair => new RelayCommand<object>(obj =>
        {
            ConnectionClass.hubConnection.InvokeAsync("StartChangeRepair", SelectedRepair);
        });

        public ICommand RefreshFilters => new RelayCommand<object>(obj =>
        {
            foreach (var p in Filter.GetType().GetProperties())
            {
                if (p.PropertyType == typeof(DateTime?))
                {
                    p.SetValue(Filter, null);
                }
                else if (p.PropertyType == typeof(bool))
                {
                    p.SetValue(Filter, false);
                }
                else
                {
                    p.SetValue(Filter, string.Empty);
                }
            }
        });

        public ICommand OpenCleanRepairCard => new RelayCommand<object>(obj =>
        {
            RepairCardViewModel cardViewModel = new RepairCardViewModel(MainObject);

            RepairCard repairCard = new RepairCard()
            {
                DataContext = cardViewModel
            };

            repairCard.ShowDialog();
        });

        public ICommand BudgetingRepairs => new RelayCommand<object>(obj =>
        {
            foreach (PropertyInfo h in HideColumns.GetType().GetProperties())
            {
                if (h.Name == "ProviderOrder" || h.Name == "RepairBill" || h.Name == "KaRepair" || h.Name == "ScOks")
                {
                    h.SetValue(HideColumns, true);
                }
                else
                {
                    h.SetValue(HideColumns, false);
                }
            }
        });

        public ICommand SeeAll => new RelayCommand<object>(obj =>
        {
            CreateHidenColumns();
        });

        public RepairsViewModel(MainObject mainObject)
        {
            SignalRActions();
            RepairContext = new RepairCardDataContext();
            ActiveVisibility = Visibility.Collapsed;
            MainObject = mainObject;
            UsersWorks = new ListCollectionView(MainObject.Repairs);
            Filter = new AllRepairsFilter(UsersWorks);
            HideColumns = new HideColumns();
            dispatcher = Application.Current.Dispatcher;
            GetFiltersLists();
            CreateHidenColumns();
            GetDataGridElements();
        }

        protected override void SignalRActions()
        {
            ConnectionClass.hubConnection.On<RepairClass>("UpdateRepairs", (newRepair) =>
            {
                dispatcher.Invoke(() => MainObject.Repairs.Insert(0, newRepair));
                GetFiltersLists();
            });

            ConnectionClass.hubConnection.On<RepairClass>("ChangedRepair", (changeRepair) =>
            {
                dispatcher.Invoke(() =>
                {
                    foreach (var a in MainObject.Repairs.Where(x => x.Id == changeRepair.Id).FirstOrDefault().GetType().GetProperties())
                    {
                        foreach (var c in changeRepair.GetType().GetProperties())
                        {
                            if (a.Name == c.Name)
                            {
                                a.SetValue(MainObject.Repairs.Where(x => x.Id == changeRepair.Id).FirstOrDefault(), c.GetValue(changeRepair));

                                continue;
                            }
                        }
                    }
                });

                GetFiltersLists();
            });
        }

        private void CreateHidenColumns()
        {
            foreach (PropertyInfo h in HideColumns.GetType().GetProperties())
            {
                h.SetValue(HideColumns, true);
            }
        }

        private void GetDataGridElements()
        {
            FrameworkElement.DataContextProperty.AddOwner(typeof(DataGridColumn));

            FrameworkElement.DataContextProperty.OverrideMetadata(typeof(DataGrid),
                    new FrameworkPropertyMetadata
                       (null, FrameworkPropertyMetadataOptions.Inherits,
                       new PropertyChangedCallback(OnDataContextChanged)));
        }

        public void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid grid = d as DataGrid;
            if (grid != null)
            {
                foreach (DataGridColumn col in grid.Columns)
                {
                    col.SetValue(FrameworkElement.DataContextProperty, e.NewValue);
                }
            }
        }        

        private void GetFiltersLists()
        {
            FilterModelList = MainObject.Repairs.Select(x => x.Model).Distinct().ToList();
            FilterProviderList = MainObject.Repairs.Select(x => x.KaProvider).Distinct().ToList();
            FilterRepairTypeList = MainObject.Repairs.Select(x => x.Warranty).Distinct().ToList();
        }
    }
}
