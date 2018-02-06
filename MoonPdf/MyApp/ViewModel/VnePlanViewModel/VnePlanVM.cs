using ATPWork.MyApp.Model.VnePlan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private VnePlanZayavka _zayvkaInwork = new VnePlanZayavka();
        public VnePlanZayavka ZayavkaInWork
        {
            get { return _zayvkaInwork; }
            set
            {
                _zayvkaInwork = value;
                OnPropertyChanged("ZayavkaInWork");
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

        }

        public void refreshAbonentList()
        {
            AllZayvki = new ObservableCollection<VnePlanZayavka>(VnePlanModel.Zayavki);
        }


        /*internal void CreatePdf()
        {
            PlanWorkModel.CreatePDF(SelectedDate);
        }*/
    }
}
