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
            AbonentsForVnePlan.Refresh();
        }
        public int GetLastRegNumber()
        { 
            int result = VnePlanModel.GetLastNumber(); 
            return result;
        }

        public void refreshAbonentList()
        {
            AllZayvki = new ObservableCollection<VnePlanZayavka>(VnePlanModel.Zayavki);
            AbonentsForVnePlan = CollectionViewSource.GetDefaultView(AllZayvki);
            AbonentsForVnePlan.GroupDescriptions.Clear();
            AbonentsForVnePlan.GroupDescriptions.Add(new PropertyGroupDescription("City"));
          
            // AbonentsForVnePlan.Filter = str => ((str as VnePlanZayavka).NumberAktTehProverki=="");


        }

        /*internal void CreatePdf()
        {
            PlanWorkModel.CreatePDF(SelectedDate);
        }*/
    }
}
