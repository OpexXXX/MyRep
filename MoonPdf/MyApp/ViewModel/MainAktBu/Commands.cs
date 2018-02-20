using ATPWork.MyApp.Model.AktBuWork;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ATPWork.MyApp.ViewModel.MainAktBu
{
    public class Commands
    {


        public DelegateCommand ProcessCompleteAct { get; private set; }
        public DelegateCommand SaveCurrentWork { get; private set; }

        public DelegateCommand GoNextAkt { get; private set; }
        public DelegateCommand GoPrevousAkt { get; private set; }

        public DelegateCommand GoFirstPageAkt { get; private set; }


        public DelegateCommand DeleteGroup { get; private set; }
        public DelegateCommand OpenFolderMail { get; private set; }
        public DelegateCommand EnterAktInSAP { get; private set; }


        private AktsBuViewModel mainAktBUVm;

        public Commands(AktsBuViewModel mainAktBUVm)
        {
            this.mainAktBUVm = mainAktBUVm;

            Predicate<object> isDatabaseConnectorBusy = f => DataBaseWorker.ConnectorBusy();
            Predicate<object> isNextAkt = f => CanGoNextAkt();
            Predicate<object> isPrevousAkt = f => CanGoPrevousAkt();
            Predicate<object> isGoPage = f => CanGoToPage();


            this.EnterAktInSAP = new DelegateCommand("Занести в SAP", async f =>
            {
                mainAktBUVm.WorkinAddAktFromPdf = true;
                await Task.Run(() =>
                {
                    try
                    {
                        AktsBuModel.EnterInSapAllPosibleAkts(new Progress<string>(mainAktBUVm.SetProgressBarText), new Progress<double>(mainAktBUVm.SetProgressBarValue));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Ошибка: " + ex.Message));
                        mainAktBUVm.WorkinAddAktFromPdf = false;
                    }
                });
            }, canEnterAktsInSAP, null);


            this.SaveCurrentWork = new DelegateCommand("Сохранить", async f =>
            {
                mainAktBUVm.WorkinAddAktFromPdf = true;
                await Task.Run(() =>
                {
                    AktsBuModel.SaveBeforeCloseApp();
                    mainAktBUVm.WorkinAddAktFromPdf = false;
                });
            }, isDatabaseConnectorBusy, new KeyGesture(Key.S, ModifierKeys.Control));

            this.OpenFolderMail = new DelegateCommand("Открыть папку",
                         OpenFolderWhisMail
                      , null, null);

            this.GoNextAkt = new DelegateCommand("Вперед", f =>
            {
                mainAktBUVm.GoNextAkt();
            }, isNextAkt, null);
            this.GoPrevousAkt = new DelegateCommand("Назад", f =>
            {
                mainAktBUVm.GoPrevousAkt();
            }, isPrevousAkt, null);
            this.GoFirstPageAkt = new DelegateCommand("1 ст.", f =>
            {
                mainAktBUVm.GoFirstPageAkt();
            }, isGoPage, null);

            this.DeleteGroup = new DelegateCommand("Удалить задание", deleteGroup, null, null);
        }

        private bool canEnterAktsInSAP(object obj)
        {
            return mainAktBUVm.UnEnterSAPAkt > 0;
        }

        private void OpenFolderWhisMail(object obj)
        {
            var gg = (string)obj;
            string path = MainAtpModel.MailDirektory + "\\" + gg + "г. Акты ПР ФЛ";
            System.Diagnostics.Process.Start("explorer", path);
        }

        private bool CanGoPrevousAkt()
        {
            bool result = mainAktBUVm.ListBoxAktInWork?.SelectedIndex > 0;
            return result;
        }
        private bool CanGoToPage()
        {
            return mainAktBUVm.SelectedAkt != null;
        }
        private bool CanGoNextAkt()
        {
            bool result = mainAktBUVm.ListBoxAktInWork?.SelectedIndex < (mainAktBUVm.ListBoxAktInWork?.Items.Count - 1);

            return result;
        }
        private bool CanCreateMail()
        {
            bool unmailedAkts = mainAktBUVm.UnmailedAkt > 0;
            bool number = mainAktBUVm.MailNumber != 0;
            bool date = mainAktBUVm.MailDate != null;
            return unmailedAkts && number && date;
        }
        private void deleteGroup(object obj)
        {

            var res = MessageBox.Show("Удалить задание?", "Удаление задания", MessageBoxButton.YesNo);
            if (obj is CollectionViewGroup)
            {
                if (res == MessageBoxResult.Yes)
                {
                    var gg = (CollectionViewGroup)obj;
                    List<AktBu> tempList = new List<AktBu>();
                    foreach (AktBu item in gg.Items)
                    {
                        tempList.Add(item);
                    }
                    foreach (AktBu item in tempList)
                    {
                        AktsBuModel.AllAkt.Remove(item);
                        mainAktBUVm.AllAkt.Remove(item);
                    }
                }
            }
            if (obj is AktBu)
            {
                AktsBuModel.AllAkt.Remove((AktBu)obj);
                mainAktBUVm.AllAkt.Remove((AktBu)obj);
            }
        }
    }
}
