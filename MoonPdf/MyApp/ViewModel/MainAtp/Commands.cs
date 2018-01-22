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
        public DelegateCommand ProcessCompletedActs { get; private set; }
        public DelegateCommand SaveCurrentWork { get; private set; }
        private  MainAtpVM mainAtpVm;

        private bool checkCurrentWork()
        {
            return (mainAtpVm.AllAktInCurrentWork.Count > 0)&& !mainAtpVm.WorkinAddAktFromPdf;
        }
        private bool checkBysyWork()
        {
            return !mainAtpVm.WorkinAddAktFromPdf;
        }
        private bool CanProcessCompleteAkts()
        {
            bool itemComplete = false;
            foreach (AktTehProverki item in mainAtpVm.AllAktInCurrentWork)
            {
                if (item.checkToComplete()) itemComplete = true;
            }
            return !mainAtpVm.WorkinAddAktFromPdf && itemComplete;
        }

        public Commands(MainAtpVM mainAtpVm)
        {
            this.mainAtpVm = mainAtpVm;
            Predicate<object> isCurrrentWork = f => checkCurrentWork(); 
            Predicate<object> isBysyAddWork = f => checkBysyWork();
            Predicate<object> isCanProcessCompleteAkts = f => CanProcessCompleteAkts();

            this.CreatePdfToWork = new DelegateCommand("Создать задание из PDF", async f =>
            {
                mainAtpVm.WorkinAddAktFromPdf = true;
                var dlg =  new Microsoft.Win32.OpenFileDialog { Title = "Выберите PDF фаил...", DefaultExt = ".pdf", Filter = "PDF фаил (.pdf)|*.pdf", CheckFileExists = true };
                if (dlg.ShowDialog() == true)
                {
                        await Task.Run(() =>
                        {
                            try
                            {
                               MainAtpModel.CreateWorkFromPdf(dlg.FileName, new Progress<double>(mainAtpVm.SetProgressBarValue));
                                mainAtpVm.WorkinAddAktFromPdf = false;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(string.Format("Ошибка: " + ex.Message));
                                mainAtpVm.WorkinAddAktFromPdf = false;
                            }
                        });
                }
            }, isBysyAddWork, new KeyGesture(Key.O, ModifierKeys.Control));
            this.AddAktToworkFromPDF = new DelegateCommand("Добавить к заданию  из PDF", async f =>
            {
                mainAtpVm.WorkinAddAktFromPdf = true;
                var dlg = new Microsoft.Win32.OpenFileDialog { Title = "Выберите PDF фаил...", DefaultExt = ".pdf", Filter = "PDF фаил (.pdf)|*.pdf", CheckFileExists = true };
                if (dlg.ShowDialog() == true)
                {
                        await Task.Run(() =>
                        {
                            try
                            {
                                MainAtpModel.CreateWorkFromPdf(dlg.FileName, new Progress<double>(mainAtpVm.SetProgressBarValue), true);
                                mainAtpVm.WorkinAddAktFromPdf = false;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(string.Format("Ошибка: " + ex.Message));
                                mainAtpVm.WorkinAddAktFromPdf = false;
                            }
                        });

                }
            }, isCurrrentWork, new KeyGesture(Key.O, ModifierKeys.Control|ModifierKeys.Shift));
            this.ProcessCompletedActs = new DelegateCommand("Обработать акты", async f =>
            {
                mainAtpVm.WorkinAddAktFromPdf = true;
                    await Task.Run(() =>
                    {
                        try
                        {
                            MainAtpModel.MoveComleteAtp(new Progress<double>(mainAtpVm.SetProgressBarValue));
                            mainAtpVm.WorkinAddAktFromPdf = false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("Ошибка: " + ex.Message));
                            mainAtpVm.WorkinAddAktFromPdf = false;
                        }
                    });
                
            }, isCanProcessCompleteAkts, null);
            this.SaveCurrentWork = new DelegateCommand("Сохранить", async f =>
            {
                mainAtpVm.WorkinAddAktFromPdf = true;
                    await Task.Run(() =>
                    {
                    });
            }, isCurrrentWork, new KeyGesture(Key.S, ModifierKeys.Control));

        }
    }
}
