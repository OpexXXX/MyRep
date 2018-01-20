using ATPWork.MyApp.View;
using ATPWork.MyApp.ViewModel;
using ATPWork.MyApp.ViewModel.PlombEditorVm;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.ViewModel.AtpEditor
{
    public class AtpEditorVM : ViewModelBase
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
            return  (AktInWork != null) ? ((AktInWork.NumberLS != null) ? AktInWork.NumberLS.ToString().Length > 9 : false) : false; 
        }
        public bool CheckFindByPu()
        {
            return (AktInWork != null) ? ((AktInWork.PuOldNumber!=null)? AktInWork.PuOldNumber.ToString().Length > 4:false) : false;
        }
        #endregion

        private static List<string> _alentList;
        public static List<string> AgentList
        {
            get { return _alentList; }
            set { _alentList = value; }
        }


        private AktTehProverki _aktInWork;
        public AktTehProverki AktInWork {
            get { return _aktInWork; }
            set {
                _aktInWork = value;
                OnPropertyChanged("AktInWork");
            }
        }

        public AtpEditorVM()
        {
           Commands = new Commands(this);
        }
    }
}
