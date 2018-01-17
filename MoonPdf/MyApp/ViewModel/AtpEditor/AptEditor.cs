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
    public class AtpEditor : ViewModelBase
    {
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

        public bool findByLs()
        {
            return  (AktInWork != null) ? ((AktInWork.NumberLS != null) ? AktInWork.NumberLS.ToString().Length > 9 : false) : false; 
        }
        public bool findByPu()
        {
            return (AktInWork != null) ? ((AktInWork.PuOldNumber!=null)? AktInWork.PuOldNumber.ToString().Length > 4:false) : false;
        }
        private PlombEditorVM PlombEditorV = new PlombEditorVM();
        public View.AtpEditor AtpEdirorWPF;
        private AktTehProverki _aktInWork;
        public AktTehProverki AktInWork {
            get { return _aktInWork; }
            set {
                _aktInWork = value;
                PlombEditorV._pl = _aktInWork.Plombs;
                OnPropertyChanged("AktInWork");
            }
        }

        public AtpEditor(View.AtpEditor AtpEdit)
        {
           Commands = new Commands(this);
            AtpEdirorWPF = AtpEdit;
            AtpEdit.DataContext = this;
            AtpEdit.PlombEditorR.DataContext = PlombEditorV;
        }
    }
}
