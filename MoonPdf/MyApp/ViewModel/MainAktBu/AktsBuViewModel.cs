using ATPWork.MyApp.Model.AktBuWork;
using MoonPdfLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ATPWork.MyApp.ViewModel.MainAktBu
{
    public class AktsBuViewModel : ViewModelBase
    {
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

        internal void SetProgressBarValue(double obj)
        {
            throw new NotImplementedException();
        }

        internal void SetProgressBarText(string obj)
        {
            throw new NotImplementedException();
        }

        public MoonPdfPanel PdfViewer;
        #endregion
        public ListView ListBoxAktInWork { get; internal set; }

        private AktBu _selectedAkt;
        public AktBu SelectedAkt
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
                    CurrentFilePdf = value.AktBuPdf;
                    OnPropertyChanged("SelectedAkt");
                }
            }
        }
        private ObservableCollection<AktBu> _allAkt;
        public ObservableCollection<AktBu> AllAkt
        {
            get { return _allAkt; }
            set
            {
                _allAkt = value;
                OnPropertyChanged("AllAkt");
            }
        }

        public int UnmailedAkt { get; internal set; }
        public int MailNumber { get; internal set; }
        public object MailDate { get; internal set; }
        public bool WorkinAddAktFromPdf { get; internal set; }
        public int UnEnterSAPAkt { get; internal set; }

        internal void GoNextAkt()
        {
            throw new NotImplementedException();
        }

        internal void GoPrevousAkt()
        {
            throw new NotImplementedException();
        }

        public AktsBuViewModel()
        {
            Commands = new Commands(this);
            refreshAllAktList();
            refreshCurrentWorkAktList();
            MainAtpModel.AllAtpRefreshRefresh += refreshAllAktList;
        }

        internal void GoSecongPageAkt()
        {
            throw new NotImplementedException();
        }

        internal void GoFirstPageAkt()
        {
            throw new NotImplementedException();
        }
    }
}
