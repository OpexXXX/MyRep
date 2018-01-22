using ATPWork.MyApp.ViewModel.MainAtp;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.ViewModel
{
    public class MainAtpVM:ViewModelBase
    {


        #region Статус Бар 
        private double _progressBarValue;
        public double ProgressBarValue
        {
            get { return _progressBarValue; }
            set { _progressBarValue = value;
                OnPropertyChanged("ProgressBarValue");
            }
        }
        private string _progressBarText;
        public string ProgressBarText
        {
            get { return _progressBarText; }
            set
            {
                _progressBarText = value;
                OnPropertyChanged("ProgressBarText");
            }
        }
        public void SetProgressBarValue(double val)
        {
            ProgressBarValue = val;
            ProgressBarText = ((int)val).ToString() + '%';
        }
        #endregion


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
        private ObservableCollection<AktTehProverki> _allAkt;
        public ObservableCollection<AktTehProverki> AllAkt
        {
            get { return _allAkt; }
            set { _allAkt = value;

                OnPropertyChanged("AllAkt");
            }
        }

        private ObservableCollection<AktTehProverki> _allAktInCurrentWork = new ObservableCollection<AktTehProverki>();
        public ObservableCollection<AktTehProverki> AllAktInCurrentWork
        {
            get { return _allAktInCurrentWork; }
            set
            {
                _allAktInCurrentWork = value;
                OnPropertyChanged("AllAktInCurrentWork");
            }
        }

        private bool _workinAddAktFromPdf;
        public bool WorkinAddAktFromPdf
        {
            get { return _workinAddAktFromPdf; }
            set
            {
                _workinAddAktFromPdf = value;
                OnPropertyChanged("WorkinAddAktFromPdf");
            }
        }

        private bool _inCurrentWork;
        public bool InCurrentWork
        {
            get { return _inCurrentWork; }
            set { _inCurrentWork = value;
                OnPropertyChanged("InCurrentWork");
            }
        }

        private AktTehProverki _selectedAkt;
        public AktTehProverki SelectedAkt
        {
            get { return _selectedAkt; }
            set {
                _selectedAkt = value;
                OnPropertyChanged("SelectedAkt");
                if (InCurrentWork!= (_allAktInCurrentWork.Count > 0) && (SelectedAkt != null)) InCurrentWork = (_allAktInCurrentWork.Count > 0) && (SelectedAkt != null);

            }
        }
        public MainAtpVM()
        {
            Commands = new Commands(this);
            refreshAllAktList();
            MainAtpModel.AllAtpRefreshRefresh += refreshAllAktList;
            MainAtpModel.CurrentWorkRefresh += refreshCurrentWorkAktList;
        }
        public void refreshAllAktList()
        {
            AllAkt = new ObservableCollection<AktTehProverki>(MainAtpModel.AllAkt);
        }
        public void refreshCurrentWorkAktList()
        {
            AllAktInCurrentWork = new ObservableCollection<AktTehProverki>(MainAtpModel.AllAktInCurrentWork);
        }
    }
}
