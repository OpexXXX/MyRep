﻿using ATPWork.MyApp.Model.AktBuWork;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.ViewModel.MainAktBu.BuEditor
{
    public class BuEditorViewModel:ViewModelBase
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
        public bool CheckFindByLs()
        {
            return (AktInWork != null) ? ((AktInWork.NumberLS != null) ? AktInWork.NumberLS.ToString().Length > 9 : false) : false;
        }
        public bool CheckFindByPu()
        {
            return (AktInWork != null) ? ((AktInWork.PuOldNumber != null) ? AktInWork.PuOldNumber.ToString().Length > 4 : false) : false;
        }
        #endregion
        private static ObservableCollection<Agent> _alentList;
        public static ObservableCollection<Agent> AgentList
        {
            get { return _alentList; }
            set { _alentList = value; }
        }
        
        private AktBu _aktInWork;
        public AktBu AktInWork
        {
            get { return _aktInWork; }
            set
            {
                _aktInWork = value;
                OnPropertyChanged("AktInWork");
            }
        }
        public BuEditorViewModel()
        {
            Commands = new Commands(this);
            GetListForComboBox();
            MainAtpModel.ComboRefresh += GetListForComboBox;
        }

        private void GetListForComboBox()
        {
            AgentList = new ObservableCollection<Agent>(MainAtpModel.AgentList);
        }

    }
}
