﻿using ATPWork.MyApp.Model.VnePlan;
using MyApp;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.ViewModel.VnePlanViewModel
{
     public class Commands
    {
        public DelegateCommand GetDataFromDb { get; private set; }
        public DelegateCommand AddZayavka { get; private set; }
        public DelegateCommand CheckToComplete { get; private set; }
        private VnePlanVM vnePlanVM;

        public Commands(VnePlanVM planVM)
        {
            this.vnePlanVM = planVM;
            Predicate<object> canSearch = f => CanSearch();
            Predicate<object> canAddZayavka = f => CanAddZayavka();

            this.GetDataFromDb = new DelegateCommand("Поиск в базе", f =>
            {
                string number = planVM.SearchString;

                planVM.ZayavkaToAdd.RegNumber = planVM.GetLastRegNumber();
                List<Dictionary<String, String>> resultSearchAbonent;
                resultSearchAbonent = DataBaseWorker.GetAbonentFromLS(number);

                if (resultSearchAbonent.Count == 1)
                {
                    string edob = resultSearchAbonent[0]["EdOborudovania"];
                    planVM.ZayavkaToAdd.setDataByDb(resultSearchAbonent[0]);
                }
                else if (resultSearchAbonent.Count > 1)
                {
                    ResultSearchWindow wndResult = new ResultSearchWindow(resultSearchAbonent);
                    wndResult.ShowDialog();
                    if ((bool)wndResult.DialogResult)
                    {
                        string edob = wndResult.SelectVal["EdOborudovania"];
                        planVM.ZayavkaToAdd.setDataByDb(wndResult.SelectVal);
                    }
                }
                else
                {
                    resultSearchAbonent = DataBaseWorker.GetAbonentFromDbByPU(number);
                    if (resultSearchAbonent.Count == 1)
                    {
                        string edob = resultSearchAbonent[0]["EdOborudovania"];
                        planVM.ZayavkaToAdd.setDataByDb(resultSearchAbonent[0]);
                    }
                    else if (resultSearchAbonent.Count > 1)
                    {
                        ResultSearchWindow wndResult = new ResultSearchWindow(resultSearchAbonent);
                        wndResult.ShowDialog();
                        if ((bool)wndResult.DialogResult)
                        {
                            string edob = wndResult.SelectVal["EdOborudovania"];
                            planVM.ZayavkaToAdd.setDataByDb(wndResult.SelectVal);
                        }
                    }
                    else
                    {
                        resultSearchAbonent = DataBaseWorker.GetAbonentFromDbByName(number);
                        if (resultSearchAbonent.Count == 1)
                        {
                            string edob = resultSearchAbonent[0]["EdOborudovania"];
                            planVM.ZayavkaToAdd.setDataByDb(resultSearchAbonent[0]);
                        }
                        else if (resultSearchAbonent.Count > 1)
                        {
                            ResultSearchWindow wndResult = new ResultSearchWindow(resultSearchAbonent);
                            wndResult.ShowDialog();
                            if ((bool)wndResult.DialogResult)
                            {
                                string edob = wndResult.SelectVal["EdOborudovania"];
                                planVM.ZayavkaToAdd.setDataByDb(wndResult.SelectVal);
                            }
                        }
                        else
                        {
                            resultSearchAbonent = DataBaseWorker.GetAbonentFromStreet(number);
                            if (resultSearchAbonent.Count == 1)
                            {
                                string edob = resultSearchAbonent[0]["EdOborudovania"];
                                planVM.ZayavkaToAdd.setDataByDb(resultSearchAbonent[0]);
                            }
                            else if (resultSearchAbonent.Count > 1)
                            {
                                ResultSearchWindow wndResult = new ResultSearchWindow(resultSearchAbonent);
                                wndResult.ShowDialog();
                                if ((bool)wndResult.DialogResult)
                                {
                                    string edob = wndResult.SelectVal["EdOborudovania"];
                                    planVM.ZayavkaToAdd.setDataByDb(wndResult.SelectVal);
                                }
                            }
                        }
                    }
                }




            }, canSearch, null);
            this.AddZayavka = new DelegateCommand("Дабавить заявку", f =>
            {
                vnePlanVM.AllZayvki.Add(vnePlanVM.ZayavkaToAdd);
                VnePlanModel.AddZayvka(vnePlanVM.ZayavkaToAdd);
                vnePlanVM.ZayavkaToAdd = new VnePlanZayavka();
            }, canAddZayavka, null);
            this.CheckToComplete = new DelegateCommand("Проверить выполнение заявок", f =>
            {
                VnePlanModel.chekCompleteZayavki();
            }, null, null);
        }

        private bool CanAddZayavka()
        {
            return vnePlanVM.ZayavkaToAdd.CanAdd();
        }

        private bool CanSearch()
        {
            return vnePlanVM.SearchString?.Length > 3;
        }
    }
}