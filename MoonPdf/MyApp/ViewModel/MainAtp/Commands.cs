using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ATPWork.MyApp.ViewModel.MainAtp
{
    public class Commands
    {
        public DelegateCommand CreatePdfToWork { get; private set; }
        public DelegateCommand AddAktToworkFromPDF { get; private set; }
        public DelegateCommand IncrementAddPlombComand { get; private set; }
        public DelegateCommand DecrementAddPlombComand { get; private set; }

        private  MainAtpVM mainAtpVm;
        private bool checkCurrentWork()
        {
            return (mainAtpVm.AllAktInCurrentWork.Count > 0)&& !mainAtpVm.WorkinAddAktFromPdf;
        }
        private bool checkBysyWork()
        {
            return !mainAtpVm.WorkinAddAktFromPdf;
        }

        public Commands(MainAtpVM mainAtpVm)
        {
            this.mainAtpVm = mainAtpVm;
            Predicate<object> isCurrrentWork = f => checkCurrentWork(); // used for the CanExecute callback
            Predicate<object> isBysyAddWork = f => checkBysyWork(); // used for the CanExecute callback
            this.CreatePdfToWork = new DelegateCommand("Создать задание из PDF", async f =>
            {
                mainAtpVm.WorkinAddAktFromPdf = true;
                var dlg = new Microsoft.Win32.OpenFileDialog { Title = "Выберите PDF фаил...", DefaultExt = ".pdf", Filter = "PDF фаил (.pdf)|*.pdf", CheckFileExists = true };
                if (dlg.ShowDialog() == true)
                {
                    try
                    {
                        var tcs = new TaskCompletionSource<bool>();
                        await Task.Run(() =>
                        {
                            try
                            {
                               MainAtpModel.CreateWorkFromPdf(dlg.FileName, new Progress<double>(mainAtpVm.SetProgressBarValue));
                                tcs.SetResult(true);
                                mainAtpVm.WorkinAddAktFromPdf = false;
                            }
                            catch (Exception ex)
                            {
                                tcs.SetException(ex);
                            }
                        });

                        //var dd = MainAtpModel.CreateWorkFromPdfAsync(dlg.FileName, new Progress<double>(mainAtpVm.SetProgressBarValue));
                        //  mainAtpVm.WorkinAddAktFromPdf = dd.Result;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Ошибка при отрытии: " + ex.Message));
                    }
                }


            }, isBysyAddWork, new KeyGesture(Key.O, ModifierKeys.Control));
            this.AddAktToworkFromPDF = new DelegateCommand("Добавить к заданию  из PDF", async f =>
            {
                mainAtpVm.WorkinAddAktFromPdf = true;
                var dlg = new Microsoft.Win32.OpenFileDialog { Title = "Выберите PDF фаил...", DefaultExt = ".pdf", Filter = "PDF фаил (.pdf)|*.pdf", CheckFileExists = true };

                if (dlg.ShowDialog() == true)
                {
                    try
                    {
                        var tcs = new TaskCompletionSource<bool>();
                        await Task.Run(() =>
                        {
                            try
                            {
                                MainAtpModel.CreateWorkFromPdf(dlg.FileName, new Progress<double>(mainAtpVm.SetProgressBarValue), true);
                                tcs.SetResult(true);
                                mainAtpVm.WorkinAddAktFromPdf = false;
                            }
                            catch (Exception ex)
                            {
                                tcs.SetException(ex);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Ошибка при отрытии: " + ex.Message));
                    }
                }

            }, isCurrrentWork, new KeyGesture(Key.O, ModifierKeys.Control|ModifierKeys.Shift));

        }

    }
}
