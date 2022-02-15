using MyApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ATPWork.MyApp.ViewModel.MainAtp
{
    public class Commands
    {
        public DelegateCommand AddAktToworkFromPDF { get; private set; }
        public DelegateCommand ProcessCompletedActs { get; private set; }
        public DelegateCommand SaveCurrentWork { get; private set; }
        public DelegateCommand RefreshSapFlTable { get; private set; }
        public DelegateCommand RefreshSapPlombTable { get; private set; }

        public DelegateCommand GoNextAkt { get; private set; }
        public DelegateCommand GoPrevousAkt { get; private set; }
        public DelegateCommand GoFirstPageAkt { get; private set; }
        public DelegateCommand GoSecongPageAkt { get; private set; }

        public DelegateCommand CreateMailAllPossibleAkt { get; private set; }

        public DelegateCommand DeleteGroup { get; private set; }

        public DelegateCommand OpenFolderMail { get; private set; }

        public DelegateCommand EnterAktInSAP { get; private set; }

        private readonly MainAtpVM mainAtpVm;
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
            Predicate<object> isBysyAddWork = f => checkBysyWork();
            Predicate<object> isCanProcessCompleteAkts = f => CanProcessCompleteAkts();
            Predicate<object> isDatabaseConnectorBusy = f => DataBaseWorker.ConnectorBusy();
            Predicate<object> isNextAkt = f => CanGoNextAkt();
            Predicate<object> isPrevousAkt = f => CanGoPrevousAkt();
            Predicate<object> isGoPage = f => CanGoToPage();
            Predicate<object> isCreateMail = f => CanCreateMail();

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
                else mainAtpVm.WorkinAddAktFromPdf = false;
            }, isBysyAddWork, new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift));
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
            this.EnterAktInSAP = new DelegateCommand("Занести в SAP", async f =>
            {
                mainAtpVm.WorkinAddAktFromPdf = true;
                await Task.Run(() =>
                {
                    try
                    {
                        MainAtpModel.EnterInSapAllPosibleAkts(new Progress<string>(mainAtpVm.SetProgressBarText), new Progress<double>(mainAtpVm.SetProgressBarValue));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Ошибка: " + ex.Message));
                        mainAtpVm.WorkinAddAktFromPdf = false;
                    }
                });
            }, canEnterAktsInSAP, null);
            this.SaveCurrentWork = new DelegateCommand("Сохранить", async f =>
            {
                mainAtpVm.WorkinAddAktFromPdf = true;
                await Task.Run(() =>
                {
                    MainAtpModel.SaveBeforeCloseApp();
                    mainAtpVm.WorkinAddAktFromPdf = false;
                });
            }, isDatabaseConnectorBusy, new KeyGesture(Key.S, ModifierKeys.Control));
            this.CreateMailAllPossibleAkt = new DelegateCommand("Создать сопроводительное (все акты)", f =>
           {
               mainAtpVm.CreateMailAllComplete(new Progress<string>(mainAtpVm.SetProgressBarText));
           }, isCreateMail, null);
            this.OpenFolderMail = new DelegateCommand("Открыть папку",
                         OpenFolderWhisMail
                      , null, null);
            this.RefreshSapPlombTable = new DelegateCommand("Обновить базу пломб", async f =>
           {
               var dlg = new Microsoft.Win32.OpenFileDialog { Title = "Выберите XLSX фаил с выгрузкой поиска SAP", DefaultExt = ".xlsx", Filter = "Excel фаил (.xlsx)|*.xlsx", CheckFileExists = true };
               if (dlg.ShowDialog() == true)
               {
                   await Task.Run(() =>
                   {
                       try
                       {
                           using (FileStream stream = File.Open(dlg.FileName, FileMode.Open, FileAccess.Read))
                           {
                               DataBaseWorker.RefreshSAPPlomb(ExcelWorker.makeDataSetForSAPFL(stream));
                           }
                       }
                       catch (Exception ex)
                       {
                           MessageBox.Show(string.Format("Ошибка: " + ex.Message));
                       }
                   });
               }
           }, isDatabaseConnectorBusy, null);
            this.RefreshSapFlTable = new DelegateCommand("Обновить базу потребителей", async f =>
            {
                var dlg = new Microsoft.Win32.OpenFileDialog { Title = "Выберите XLSX фаил с выгрузкой поиска SAP", DefaultExt = ".xlsx", Filter = "Excel фаил (.xlsx)|*.xlsx", CheckFileExists = true };
                if (dlg.ShowDialog() == true)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            using (FileStream stream = File.Open(dlg.FileName, FileMode.Open, FileAccess.Read))
                            {
                                DataBaseWorker.RefreshSAPFL(ExcelWorker.makeDataSetForSAPFL(stream));
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("Ошибка: " + ex.Message));
                        }
                    });
                }
            }, isDatabaseConnectorBusy, null);
            this.GoNextAkt = new DelegateCommand("Вперед", f =>
           {
               mainAtpVm.GoNextAkt();
           }, isNextAkt, null);
            this.GoPrevousAkt = new DelegateCommand("Назад", f =>
            {
                mainAtpVm.GoPrevousAkt();
            }, isPrevousAkt, null);
            this.GoFirstPageAkt = new DelegateCommand("1 ст.", f =>
            {
                mainAtpVm.GoFirstPageAkt();
            }, isGoPage, null);
            this.GoSecongPageAkt = new DelegateCommand("2 ст.", f =>
            {
                mainAtpVm.GoSecongPageAkt();
            }, isGoPage, null);
            this.DeleteGroup = new DelegateCommand("Удалить задание", deleteGroup, null, null);
        }
        private bool canEnterAktsInSAP(object obj)
        {
            return mainAtpVm.UnEnterSAPAkt > 0;
        }
        private void OpenFolderWhisMail(object obj)
        {
            var gg = (string)obj;
            string path = MainAtpModel.MailDirektory + "\\" + gg + "г. Акты ПР ФЛ";
            System.Diagnostics.Process.Start("explorer", path);
        }
        private void deleteGroup(object obj)
        {

            var res = MessageBox.Show("Удалить задание?", "Удаление задания", MessageBoxButton.YesNo);
            if (obj is CollectionViewGroup)
            {
                if (res == MessageBoxResult.Yes)
                {
                    var gg = (CollectionViewGroup)obj;
                    List<AktTehProverki> tempList = new List<AktTehProverki>();
                    foreach (AktTehProverki item in gg.Items)
                    {
                        tempList.Add(item);
                    }
                    foreach (AktTehProverki item in tempList)
                    {
                        MainAtpModel.AllAktInCurrentWork.Remove(item);
                        mainAtpVm.AllAktInCurrentWork.Remove(item);
                    }
                }
            }
            if (obj is AktTehProverki)
            {

                MainAtpModel.AllAktInCurrentWork.Remove((AktTehProverki)obj);
                mainAtpVm.AllAktInCurrentWork.Remove((AktTehProverki)obj);

            }



        }
        private bool CanGoPrevousAkt()
        {
            bool result = mainAtpVm.ListBoxAktInWork?.SelectedIndex > 0;
            return result;
        }
        private bool CanGoToPage()
        {
            return mainAtpVm.SelectedAkt != null;
        }
        private bool CanGoNextAkt()
        {
            bool result = mainAtpVm.ListBoxAktInWork?.SelectedIndex < (mainAtpVm.ListBoxAktInWork?.Items.Count - 1);

            return result;
        }
        private bool CanCreateMail()
        {
            bool unmailedAkts = mainAtpVm.UnmailedAkt > 0;
            bool number = mainAtpVm.MailNumber != 0;
            bool date = mainAtpVm.MailDate != null;
            return unmailedAkts && number && date;
        }

    }
}
