using MyApp;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ATPWork.MyApp.ViewModel.AtpEditor
{
        public class Commands
        {
            public DelegateCommand GetDataFromDbByNumberLs { get; private set; }
            public DelegateCommand GetDataFromDbByNumberPu { get; private set; }
            public Commands(AtpEditorVM AtpEdit)
            {
            Predicate<object> findByLs = f => AtpEdit.CheckFindByLs()&& DataBaseWorker.ConnectorBusy(); // 
            Predicate<object> findByPu= f => AtpEdit.CheckFindByPu() && DataBaseWorker.ConnectorBusy(); // 

            this.GetDataFromDbByNumberLs = new DelegateCommand("Поиск по номеру лицевого счета", f =>
            {
                string number = AtpEdit.AktInWork.NumberLS;
                List<Dictionary<String, String>> resultSearchAbonent;
                resultSearchAbonent = DataBaseWorker.GetAbonentFromLS(number);
                
                if(resultSearchAbonent.Count == 1)
                {
                    string edob = resultSearchAbonent[0]["EdOborudovania"];
                    List<Dictionary<String, String>> resultSearchPlombs;
                    resultSearchPlombs = DataBaseWorker.GetPlombsFromEdOb(edob);

                    AtpEdit.AktInWork.setDataByDb(resultSearchAbonent[0],resultSearchPlombs);
                } else if (resultSearchAbonent.Count > 1)
                {
                    ResultSearchWindow wndResult = new ResultSearchWindow(resultSearchAbonent);
                    wndResult.ShowDialog();
                    if ((bool)wndResult.DialogResult)
                    {
                        string edob = wndResult.SelectVal["EdOborudovania"];
                        List<Dictionary<String, String>> resultSearchPlombs;
                        resultSearchPlombs = DataBaseWorker.GetPlombsFromEdOb(edob);
                        AtpEdit.AktInWork.setDataByDb(wndResult.SelectVal, resultSearchPlombs);
                    }
                }
             }
            , findByLs, new KeyGesture(Key.Left));
            this.GetDataFromDbByNumberPu = new DelegateCommand("Поиск по номеру прибора учета", f => 
            {
                string number = AtpEdit.AktInWork.PuOldNumber;
                List<Dictionary<String, String>> resultSearchAbonent;
                resultSearchAbonent = DataBaseWorker.GetAbonentFromDbByPU(number);
                if (resultSearchAbonent.Count == 1)
                {
                    string edob = resultSearchAbonent[0]["EdOborudovania"];
                    List<Dictionary<String, String>> resultSearchPlombs;
                    resultSearchPlombs = DataBaseWorker.GetPlombsFromEdOb(edob);

                    AtpEdit.AktInWork.setDataByDb(resultSearchAbonent[0], resultSearchPlombs);
                }
                else if (resultSearchAbonent.Count > 1)
                {
                    ResultSearchWindow wndResult = new ResultSearchWindow(resultSearchAbonent);
                    wndResult.ShowDialog();
                    if ((bool)wndResult.DialogResult)
                    {
                        string edob = wndResult.SelectVal["EdOborudovania"];
                        List<Dictionary<String, String>> resultSearchPlombs;
                        resultSearchPlombs = DataBaseWorker.GetPlombsFromEdOb(edob);
                        AtpEdit.AktInWork.setDataByDb(wndResult.SelectVal, resultSearchPlombs);
                    }
                }
            }, findByPu, new KeyGesture(Key.Right));
            }
        }
}
