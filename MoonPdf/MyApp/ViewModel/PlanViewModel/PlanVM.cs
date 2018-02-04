using ATPWork.MyApp.Model.Plan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.ViewModel.PlanViewModel
{
    public class PlanVM : ViewModelBase
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

        private DateTime _selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if ((_selectedDate != value) && (value != null))
                {
                    _selectedDate = value;
                    PlanWorkModel.refreshAbonentList(value);
                    OnPropertyChanged("SelectedDate");
                }
            }
        }
        private Abonent _selectedAbonent;
        public Abonent SelectedAbonent
        {
            get { return _selectedAbonent; }
            set
            {
                if ((_selectedAbonent != value) && (value != null))
                {
                    _selectedAbonent = value;
                    OnPropertyChanged("SelectedAbonent");
                }
            }
        }
        private ObservableCollection<Abonent> _allAbonent;
        public ObservableCollection<Abonent> AllAbonent
        {
            get { return _allAbonent; }
            set
            {
                _allAbonent = value;

                OnPropertyChanged("AllAbonent");
            }
        }

        public PlanVM()
        {
            Commands = new Commands(this);
            refreshAbonentList();
            PlanWorkModel.AbonentsRefresh += refreshAbonentList;
        }

        public void refreshAbonentList()
        {
            AllAbonent = new ObservableCollection<Abonent>(PlanWorkModel.AbonentList);
        }


        internal void CreatePdf()
        {
            PlanWorkModel.CreatePDF(SelectedDate);
        }
    }
}
