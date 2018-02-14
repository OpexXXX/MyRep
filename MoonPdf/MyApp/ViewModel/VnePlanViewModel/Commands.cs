using ATPWork.MyApp.Model;
using ATPWork.MyApp.Model.VnePlan;
using MyApp;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ATPWork.MyApp.ViewModel.VnePlanViewModel
{
     public class Commands
    {
        public DelegateCommand GetDataFromDb { get; private set; }
        public DelegateCommand AddZayavka { get; private set; }
        public DelegateCommand CheckToComplete { get; private set; }
        public DelegateCommand CreateReestr { get; private set; }
        public DelegateCommand CreateWordDoc { get; private set; }

        private VnePlanVM vnePlanVM;

        public Commands(VnePlanVM vnePlanVM)
        {
            this.vnePlanVM = vnePlanVM;
            Predicate<object> canSearch = f => CanSearch();
            Predicate<object> canAddZayavka = f => CanAddZayavka();

            this.GetDataFromDb = new DelegateCommand("Поиск в базе", f =>
            {
                string number = vnePlanVM.SearchString;

                vnePlanVM.ZayavkaToAdd.RegNumber = vnePlanVM.GetLastRegNumber();
                List<Dictionary<String, String>> resultSearchAbonent;
                resultSearchAbonent = DataBaseWorker.GetAbonentFromLS(number);

                if (resultSearchAbonent.Count == 1)
                {
                    string edob = resultSearchAbonent[0]["EdOborudovania"];
                    vnePlanVM.ZayavkaToAdd.setDataByDb(resultSearchAbonent[0]);
                }
                else if (resultSearchAbonent.Count > 1)
                {
                    ResultSearchWindow wndResult = new ResultSearchWindow(resultSearchAbonent);
                    wndResult.ShowDialog();
                    if ((bool)wndResult.DialogResult)
                    {
                        string edob = wndResult.SelectVal["EdOborudovania"];
                        vnePlanVM.ZayavkaToAdd.setDataByDb(wndResult.SelectVal);
                    }
                }
                else
                {
                    resultSearchAbonent = DataBaseWorker.GetAbonentFromDbByPU(number);
                    if (resultSearchAbonent.Count == 1)
                    {
                        string edob = resultSearchAbonent[0]["EdOborudovania"];
                        vnePlanVM.ZayavkaToAdd.setDataByDb(resultSearchAbonent[0]);
                    }
                    else if (resultSearchAbonent.Count > 1)
                    {
                        ResultSearchWindow wndResult = new ResultSearchWindow(resultSearchAbonent);
                        wndResult.ShowDialog();
                        if ((bool)wndResult.DialogResult)
                        {
                            string edob = wndResult.SelectVal["EdOborudovania"];
                            vnePlanVM.ZayavkaToAdd.setDataByDb(wndResult.SelectVal);
                        }
                    }
                    else
                    {
                        resultSearchAbonent = DataBaseWorker.GetAbonentFromDbByName(number);
                        if (resultSearchAbonent.Count == 1)
                        {
                            string edob = resultSearchAbonent[0]["EdOborudovania"];
                            vnePlanVM.ZayavkaToAdd.setDataByDb(resultSearchAbonent[0]);
                        }
                        else if (resultSearchAbonent.Count > 1)
                        {
                            ResultSearchWindow wndResult = new ResultSearchWindow(resultSearchAbonent);
                            wndResult.ShowDialog();
                            if ((bool)wndResult.DialogResult)
                            {
                                string edob = wndResult.SelectVal["EdOborudovania"];
                                vnePlanVM.ZayavkaToAdd.setDataByDb(wndResult.SelectVal);
                            }
                        }
                        else
                        {
                            resultSearchAbonent = DataBaseWorker.GetAbonentFromStreet(number);
                            if (resultSearchAbonent.Count == 1)
                            {
                                string edob = resultSearchAbonent[0]["EdOborudovania"];
                                vnePlanVM.ZayavkaToAdd.setDataByDb(resultSearchAbonent[0]);
                            }
                            else if (resultSearchAbonent.Count > 1)
                            {
                                ResultSearchWindow wndResult = new ResultSearchWindow(resultSearchAbonent);
                                wndResult.ShowDialog();
                                if ((bool)wndResult.DialogResult)
                                {
                                    string edob = wndResult.SelectVal["EdOborudovania"];
                                    vnePlanVM.ZayavkaToAdd.setDataByDb(wndResult.SelectVal);
                                }
                            }
                        }
                    }
                }




            }, canSearch, null);
            this.AddZayavka = new DelegateCommand("Дабавить заявку", f =>
            {
                VnePlanModel.AddZayvka(this.vnePlanVM.ZayavkaToAdd);
                vnePlanVM.refreshAbonentList();
                vnePlanVM.SelectedZayvka = vnePlanVM.ZayavkaToAdd;
                createWord(null);
                this.vnePlanVM.ZayavkaToAdd = new VnePlanZayavka();
            }, canAddZayavka, null);
            this.CheckToComplete = new DelegateCommand("Проверить выполнение заявок", f =>
            {
                VnePlanModel.chekCompleteZayavki();
            }, null, null);
            this.CreateReestr = new DelegateCommand("Открыть в PDF", createPdf, null, null);
            this.CreateWordDoc = new DelegateCommand("Открыть в Word", createWord, null, null);
            
        }


        #region Методы контекстного меню группы

        private void createWord(object obj)
        {
            Process.Start(WordShablon.CreateVnePlanZayvka(vnePlanVM.ZayavkaToAdd)); 
        }

        private void createPdf(object obj)
        {
            var gg = (CollectionViewGroup)obj;
            List<VnePlanZayavka> ggg = new List<VnePlanZayavka>();
            foreach (VnePlanZayavka item in gg.Items)
            {
                ggg.Add(item);
            }
            if (ggg.Count > 0)
            {
                Process.Start(VnePlanModel.CreatePDF(ggg));
            }
        }
        #endregion
        #region Методы CanExec
        private bool CanAddZayavka()
        {
            return vnePlanVM.ZayavkaToAdd.CanAdd();
        }

        private bool CanSearch()
        {
            return vnePlanVM.SearchString?.Length > 3;
        }
        #endregion
    }
}
