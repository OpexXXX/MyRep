using ATPWork.MyApp.Model.VnePlan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ATPWork.MyApp.ViewModel.VnePlanViewModel
{
    public class VnePlanVM : ViewModelBase
    {
        #region Команды
        private Commands _commands;
        public Commands Commands
        {
            get { return _commands; }
            set
            {
                _commands = value;
                OnPropertyChanged("Commands");
            }
        }
        #endregion
        private ObservableCollection<VnePlanZayavka> _allZayvki;
        public ObservableCollection<VnePlanZayavka> AllZayvki
        {
            get { return _allZayvki; }
            set
            {
                _allZayvki = value;
                OnPropertyChanged("AllZayvki");
            }
        }
        private ICollectionView _abonentsForVnePlan ;

        public ICollectionView AbonentsForVnePlan
        {
            get { return _abonentsForVnePlan; }
            set { _abonentsForVnePlan = value;
                OnPropertyChanged("AbonentsForVnePlan");
            }
        }
        private VnePlanZayavka _zayvkaToAdd = new VnePlanZayavka();
        public VnePlanZayavka ZayavkaToAdd
        {
            get { return _zayvkaToAdd; }
            set
            {
                _zayvkaToAdd = value;
                OnPropertyChanged("ZayavkaToAdd");
            }
        }
        private VnePlanZayavka _selectedZayvka;
        public VnePlanZayavka SelectedZayvka
        {
            get { return _selectedZayvka; }
            set
            {
                _selectedZayvka = value;
                OnPropertyChanged("SelectedZayvka");
            }
        }


        private bool _expanderOpen = false;
        public bool ExpanderOpen
        {
            get { return _expanderOpen; }
            set
            {
                _expanderOpen = value;
                OnPropertyChanged("ExpanderOpen");
            }
        }
        private string _searchStringView;
        public string SearchStringView
        {
            get { return _searchStringView; }
            set
            {
                _searchStringView = value;
                if (value.Length == 0)
                {
                    AbonentsForVnePlan.Filter = str => ((str as VnePlanZayavka).NumberAktTehProverki == "");
                    ExpanderOpen = false;
                }
                else if(value.Length>3)
                {
                    AbonentsForVnePlan.Filter = filterZayvki;
                  ExpanderOpen = true;
                }

                OnPropertyChanged("SearchStringView");
            }
        }
        private bool filterZayvki(object obj)
        {
            VnePlanZayavka d = (VnePlanZayavka)obj;

            bool a = (d.FIO != null)? (d.FIO.Contains(_searchStringView)) : false;
               bool b = d.Adress != null ? d.Adress.Contains(_searchStringView) : false;
               bool c = d.NumberLS != null ? d.NumberLS.Contains(_searchStringView) : false;
                bool g = d.PuOldNumber != null ? d.PuOldNumber.Contains(_searchStringView) : false;
               bool e = d.RegNumber != null ? d.RegNumber.ToString().Contains(_searchStringView) : false;
                bool f = d.DateReg != null ? d.DateReg.ToString("d").Contains(_searchStringView) : false;

            return a || b || c || g || e || f;
                    
        }
        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
               
                   _searchString = value;
                OnPropertyChanged("SearchString");
            }
        }
        public VnePlanVM()
        {
            Commands = new Commands(this);
            refreshAbonentList();
            VnePlanModel.AbonentsRefresh += refreshAbonentList;
            AbonentsForVnePlan = CollectionViewSource.GetDefaultView(AllZayvki);
        }
        public int GetLastRegNumber()
        { 
            int result = VnePlanModel.GetLastNumber(); 
            return result;
        }
        public void refreshAbonentList()
        {
            AllZayvki = new ObservableCollection<VnePlanZayavka>(VnePlanModel.Zayavki);
            AbonentsForVnePlan = CollectionViewSource.GetDefaultView(VnePlanModel.Zayavki);
            AbonentsForVnePlan.GroupDescriptions.Clear();
            AbonentsForVnePlan.GroupDescriptions.Add(new PropertyGroupDescription("City"));
            AbonentsForVnePlan.Filter = str => ((str as VnePlanZayavka).NumberAktTehProverki=="");
            AbonentsForVnePlan.SortDescriptions.Clear();
            
            AbonentsForVnePlan.SortDescriptions.Add(new SortDescription("City", ListSortDirection.Ascending));
            AbonentsForVnePlan.SortDescriptions.Add(new SortDescription("Street", ListSortDirection.Ascending));
            AbonentsForVnePlan.SortDescriptions.Add(new SortDescription("House", ListSortDirection.Ascending));
            AbonentsForVnePlan.SortDescriptions.Add(new SortDescription("Kvartira", ListSortDirection.Ascending)); 
            AbonentsForVnePlan.SortDescriptions.Add(new SortDescription("DateReg", ListSortDirection.Ascending));
            AbonentsForVnePlan.SortDescriptions.Add(new SortDescription("RegNumber", ListSortDirection.Ascending));


        }
    }
}
