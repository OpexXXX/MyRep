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
            Predicate<object> findByLs = f => AtpEdit.CheckFindByLs(); // 
            Predicate<object> findByPu= f => AtpEdit.CheckFindByPu(); // 
            this.GetDataFromDbByNumberLs = new DelegateCommand("Поиск по номеру лицевого счета", f =>
            {
                string number = AtpEdit.AktInWork.NumberLS;
                List<Dictionary<String, String>> resultSearch;
                resultSearch = DataBaseWorker.GetAbonentFromLS(number);
                
                if(resultSearch.Count == 1)
                {
                    AtpEdit.AktInWork.setDataByDb(resultSearch[0]);
                } else if (resultSearch.Count > 1)
                {
                    ResultSearchWindow wndResult = new ResultSearchWindow(resultSearch);
                    wndResult.ShowDialog();
                    if ((bool)wndResult.DialogResult)
                    {
                        AtpEdit.AktInWork.setDataByDb(wndResult.SelectVal);
                    }
                }
             }
            , findByLs, new KeyGesture(Key.Left));
            this.GetDataFromDbByNumberPu = new DelegateCommand("Поиск по номеру прибора учета", f => 
            {
                string number = AtpEdit.AktInWork.PuOldNumber;
                List<Dictionary<String, String>> resultSearch;
                resultSearch = DataBaseWorker.GetAbonentFromDbByPU(number);
                if (resultSearch.Count == 1)
                {
                    AtpEdit.AktInWork.setDataByDb(resultSearch[0]);
                }
                else if (resultSearch.Count > 1)
                {
                    ResultSearchWindow wndResult = new ResultSearchWindow(resultSearch);
                    wndResult.ShowDialog();
                    if ((bool)wndResult.DialogResult)
                    {
                        AtpEdit.AktInWork.setDataByDb(wndResult.SelectVal);
                    }
                }
            }, findByPu, new KeyGesture(Key.Right));
            }
        }
}
