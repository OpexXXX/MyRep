using MyApp;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.ViewModel.MainAktBu.BuEditor
{
    public class Commands
    {
        private BuEditorViewModel BuEditVM;

        public DelegateCommand GetDataFromDbByNumberLs { get; private set; }
        public DelegateCommand GetDataFromDbByNumberPu { get; private set; }

        public DelegateCommand NumberUp { get; private set; }
        public DelegateCommand NumberDown { get; private set; }
        public DelegateCommand DateUp { get; private set; }
        public DelegateCommand DateDown { get; private set; }

        public DelegateCommand RemoveSecondAgent { get; private set; }

        private bool CanNumberCange()
        {
            bool result = (BuEditVM.AktInWork != null);
            return result;
        }
        private bool CanDateCange()
        {
            bool result = (BuEditVM.AktInWork != null);
            return result;
        }
        private bool CanRemAgent()
        {

            bool result = (BuEditVM.AktInWork?.Agent_2 != null);
            return result;
        }



        public Commands(BuEditorViewModel AtpEdit)
        {
            this.BuEditVM = AtpEdit;
            Predicate<object> findByLs = f => AtpEdit.CheckFindByLs() && DataBaseWorker.ConnectorBusy(); // 
            Predicate<object> findByPu = f => AtpEdit.CheckFindByPu() && DataBaseWorker.ConnectorBusy(); // 
            Predicate<object> canNumberCange = f => CanNumberCange();// 
            Predicate<object> canDateCange = f => CanDateCange();// 
            Predicate<object> canRemoveAgent = f => CanRemAgent();// 

            this.GetDataFromDbByNumberLs = new DelegateCommand("Поиск по номеру лицевого счета", f =>
            {
                string number = AtpEdit.AktInWork.NumberLS;
                List<Dictionary<String, String>> resultSearchAbonent;
                resultSearchAbonent = DataBaseWorker.GetAbonentFromLS(number);

                if (resultSearchAbonent.Count == 1)
                {
                    string edob = resultSearchAbonent[0]["EdOborudovania"];


                    AtpEdit.AktInWork.setDataByDb(resultSearchAbonent[0]);
                }
                else if (resultSearchAbonent.Count > 1)
                {
                    ResultSearchWindow wndResult = new ResultSearchWindow(resultSearchAbonent);
                    wndResult.ShowDialog();
                    if ((bool)wndResult.DialogResult)
                    {
                        string edob = wndResult.SelectVal["EdOborudovania"];
                        AtpEdit.AktInWork.setDataByDb(wndResult.SelectVal);
                    }
                }
            }, findByLs, null);
            this.GetDataFromDbByNumberPu = new DelegateCommand("Поиск по номеру прибора учета", f =>
            {
                string number = AtpEdit.AktInWork.PuOldNumber;
                List<Dictionary<String, String>> resultSearchAbonent;
                resultSearchAbonent = DataBaseWorker.GetAbonentFromDbByPU(number);
                if (resultSearchAbonent.Count == 1)
                {
                    string edob = resultSearchAbonent[0]["EdOborudovania"];


                    AtpEdit.AktInWork.setDataByDb(resultSearchAbonent[0]);
                }
                else if (resultSearchAbonent.Count > 1)
                {
                    ResultSearchWindow wndResult = new ResultSearchWindow(resultSearchAbonent);
                    wndResult.ShowDialog();
                    if ((bool)wndResult.DialogResult)
                    {
                        string edob = wndResult.SelectVal["EdOborudovania"];
                        AtpEdit.AktInWork.setDataByDb(wndResult.SelectVal);
                    }
                }
            }, findByPu, null);
            this.NumberUp = new DelegateCommand("+1", f =>
            {
                AtpEdit.AktInWork.Number++;
            }, canNumberCange, null);
            this.NumberDown = new DelegateCommand("-1", f =>
            {
                AtpEdit.AktInWork.Number--;
            }, canNumberCange, null);
           
            this.RemoveSecondAgent = new DelegateCommand("Очистить", f =>
            {
                AtpEdit.AktInWork.Agent_2 = null;
            }, canRemoveAgent, null);
        }

    }
}
