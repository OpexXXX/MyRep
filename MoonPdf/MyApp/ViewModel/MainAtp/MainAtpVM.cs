using ATPWork.MyApp.ViewModel.MainAtp;
using MoonPdfLib;
using MyApp.Model;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ATPWork.MyApp.ViewModel
{
    public class MainAtpVM : ViewModelBase
    {
        #region Статус Бар 
        private double _progressBarValue;
        public double ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                _progressBarValue = value;

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

        private string _logText;
        public string LogText
        {
            get { return _logText; }
            set
            {
                _logText += "\n";
                _logText += DateTime.Now.ToString("H: mm:ss") + " " + value;
                OnPropertyChanged("LogText");
            }
        }
        public void SetProgressBarValue(double val)
        {
            ProgressBarValue = val;
            ProgressBarText = ((int)val).ToString() + '%';
        }
        public void SetProgressBarText(string text)
        {
            LogText = text;
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
        #region PdfView
        private int _currentPagePdf;
        public int CurrentPagePdf
        {
            get { return _currentPagePdf; }
            set
            {
                if (_currentPagePdf != value && PdfViewer?.CurrentSource != null)
                {
                    _currentPagePdf = value;
                    try
                    {
                        PdfViewer.GotoPage(value + 1);
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                    OnPropertyChanged("CurrentPagePdf");
                };
            }
        }
        private string _currentFilePdf;
        public string CurrentFilePdf
        {
            get { return _currentFilePdf; }
            set
            {
                if ((_currentFilePdf != value || PdfViewer?.CurrentSource == null) && PdfViewer != null)
                {
                    _currentFilePdf = value;
                    try
                    {
                        PdfViewer.OpenFile(value);
                        PdfViewer.ZoomToWidth();
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }

                    OnPropertyChanged("CurrentFilePdf");
                }
            }
        }
        public MoonPdfPanel PdfViewer;
        #endregion
        #region сопроводиловка
        private int _mailNumber;
        public int MailNumber
        {
            get { return _mailNumber; }
            set
            {
                _mailNumber = value;
                OnPropertyChanged("MailNumber");
            }
        }
        private DateTime? _mailDate;
        public DateTime? MailDate
        {
            get { return _mailDate; }
            set
            {
                _mailDate = value;
                OnPropertyChanged("MailDate");
            }
        }
        public int UnmailedAkt
        {
            get
            {
                int result = 0;
                foreach (AktTehProverki item in AllAkt)
                {
                    if (item.DateMail == null) result++;
                }
                return result;
            }
        }

        public int UnEnterSAPAkt
        {
            get
            {
                int result = 0;
                foreach (AktTehProverki item in AllAkt)
                {
                    if (item.SapNumberAkt == null || item.SapNumberAkt == "") result++;
                }
                return result;
            }
        }

        public long SizeUnmailedPdf
        {
            get
            {
                long result = 0;
                foreach (AktTehProverki item in AllAkt)
                {
                    if (item.NumberMail == 0) result += item.SizePDF;
                }

                return result;
            }

        }
        public int CountReadyAkt
        {
            get
            {
                int result = 0;
                foreach (AktTehProverki item in AllAktInCurrentWork)
                {
                    if (item.checkToComplete()) result++;
                }
                return result;
            }
        }
        public long SizeReadyAkt
        {
            get
            {

                long result = 0;
                foreach (AktTehProverki item in AllAktInCurrentWork)
                {
                    if (item.checkToComplete()) result += item.SizePDF;
                }
                return result;
            }

        }
        private AktTehProverki _selectedAktComplete;
        public AktTehProverki SelectedAktComplete
        {
            get { return _selectedAktComplete; }
            set
            {
                _selectedAktComplete = value;
                OnPropertyChanged("SelectedAktComplete");
            }
        }
        #endregion
        public ListView ListBoxAktInWork { get; internal set; }

        private AktTehProverki _selectedAkt;
        public AktTehProverki SelectedAkt
        {
            get { return _selectedAkt; }
            set
            {
                if (value == null)
                {

                }

                if ((_selectedAkt != value) && (value != null))
                {
                    _selectedAkt = value;
                    CurrentFilePdf = value.NamePdfFile;
                    CurrentPagePdf = value.NumberOfPagesInSoursePdf[0];
                    OnPropertyChanged("SelectedAkt");
                    OnPropertyChanged("CountReadyAkt");
                    OnPropertyChanged("SizeReadyAkt");


                    if (InCurrentWork != (_allAktInCurrentWork.Count > 0) && (SelectedAkt != null)) InCurrentWork = (_allAktInCurrentWork.Count > 0) && (SelectedAkt != null);
                }
            }
        }
        private ObservableCollection<AktTehProverki> _allAkt;
        public ObservableCollection<AktTehProverki> AllAkt
        {
            get { return _allAkt; }
            set
            {
                _allAkt = value;

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
            set
            {
                _inCurrentWork = value;
                OnPropertyChanged("InCurrentWork");
            }
        }

        public MainAtpVM()
        {
            Commands = new Commands(this);
            refreshAllAktList();
            refreshCurrentWorkAktList();
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
            OnPropertyChanged("UnmailedAkt");
            OnPropertyChanged("SizeUnmailedPdf");
            OnPropertyChanged("CountReadyAkt");
            OnPropertyChanged("SizeReadyAkt");
        }

        internal void GoNextAkt()
        {
            AktTehProverki oldAkt = (AktTehProverki)ListBoxAktInWork.SelectedItem;
            ListBoxAktInWork.SelectedIndex++;
            AktTehProverki NewAkt = (AktTehProverki)ListBoxAktInWork.SelectedItem;
            if (NewAkt.Number == 0 && oldAkt.Number != 0) NewAkt.Number = oldAkt.Number + 1;
            if (NewAkt.DateWork == null && oldAkt.DateWork != null) NewAkt.DateWork = oldAkt.DateWork;

        }
        internal void GoPrevousAkt()
        {
            ListBoxAktInWork.SelectedIndex--;
        }
        internal void GoFirstPageAkt()
        {
            CurrentPagePdf = SelectedAkt.NumberOfPagesInSoursePdf[0];
        }
        internal void GoSecongPageAkt()
        {
            CurrentPagePdf = SelectedAkt.NumberOfPagesInSoursePdf[1];
        }
        internal void CreateMailAllComplete(Progress<string> progress)
        {
            MainAtpModel.CreateMailATP(progress, MailNumber, (DateTime)MailDate);
        }

    }
}
