﻿using ATPWork.MyApp.Model.Plan;
using System;
using System.Collections.ObjectModel;

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
        private PlanAbonent _selectedAbonent;
        public PlanAbonent SelectedAbonent
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
        private ObservableCollection<PlanAbonent> _allAbonent;
        public ObservableCollection<PlanAbonent> AllAbonent
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
            AllAbonent = new ObservableCollection<PlanAbonent>(PlanWorkModel.AbonentList);
        }


        internal void CreatePdf()
        {

        }
    }
}
