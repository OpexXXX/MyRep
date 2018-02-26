using ATPWork.MyApp.Model;
using ATPWork.MyApp.View;
using MyApp;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public DelegateCommand OpenImage { get; private set; }
        public DelegateCommand CalcRaschet { get; private set; }
        public DelegateCommand GetNormativ { get; private set; }
        public DelegateCommand NumberUp { get; private set; }
        public DelegateCommand NumberDown { get; private set; }
        public DelegateCommand DateUp { get; private set; }
        public DelegateCommand DateDown { get; private set; }
        public DelegateCommand FindCurrentProverka { get; private set; }
        public DelegateCommand RemoveSecondAgent { get; private set; }

        public DelegateCommand  OpenRaschetDocx { get; private set; }
        public DelegateCommand OpenMailDocx { get; private set; }
        public DelegateCommand OpenPoliceMailDocx { get; private set; }
        public DelegateCommand OpenAgent1Docx { get; private set; }
        public DelegateCommand OpenAgent2Docx { get; private set; }

        public DelegateCommand OpenRaschetPDF { get; private set; }
        public DelegateCommand OpenMailPDF { get; private set; }
        public DelegateCommand OpenPoliceMailPDF { get; private set; }
        public DelegateCommand OpenAgent1PDF { get; private set; }
        public DelegateCommand OpenAgent2PDF { get; private set; }

        public DelegateCommand OpenPoliceMailDocPDF { get; private set; }
        public DelegateCommand OpenPhotoInPDF { get; private set; }

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



        public Commands(BuEditorViewModel BuEditorVM)
        {
            this.BuEditVM = BuEditorVM;
            Predicate<object> findByLs = f => BuEditorVM.CheckFindByLs() && DataBaseWorker.ConnectorBusy(); // 
            Predicate<object> findByPu = f => BuEditorVM.CheckFindByPu() && DataBaseWorker.ConnectorBusy(); // 
            Predicate<object> canNumberCange = f => CanNumberCange();// 
            Predicate<object> canDateCange = f => CanDateCange();// 
            Predicate<object> canRemoveAgent = f => CanRemAgent();// 
            this.OpenPhotoInPDF = new DelegateCommand("Открыть фото в  PDF", f =>
            {
                string filePathPDF = WordShablon.CreatePdfWithPhoto(BuEditVM.AktInWork.PhotoFile);
                Process.Start(filePathPDF);

            }, null, null);

            this.CalcRaschet = new DelegateCommand("Расчитать", f =>
            {
                
                BuEditVM.AktInWork.calcBu();

            }, canCalcRaschetBUCange, null);

            this.OpenAgent1PDF = new DelegateCommand("Открыть объяснения в PDF", f =>
            {
                string filePathDocx = WordShablon.CreateObysnenyaForBuAkt(BuEditVM.AktInWork, BuEditVM.AktInWork.Agent_1);
                string filePathPDF= WordShablon.ConvertDocxToPdf(filePathDocx);
                Process.Start(filePathPDF);
            }, canCalcRaschetBUCange, null);

            this.OpenAgent2PDF = new DelegateCommand("Открыть объяснения в PDF", f =>
            {
                string filePathDocx = WordShablon.CreateObysnenyaForBuAkt(BuEditVM.AktInWork, BuEditVM.AktInWork.Agent_2);
                string filePathPDF = WordShablon.ConvertDocxToPdf(filePathDocx);
                Process.Start(filePathPDF);
            }, canCalcRaschetBUCange, null);
            this.OpenRaschetPDF = new DelegateCommand("Открыть расчет в PDF", f =>
            {
                string filePathDocx = WordShablon.CreateRaschetForBuAkt(BuEditVM.AktInWork);
                string filePathPDF = WordShablon.ConvertDocxToPdf(filePathDocx);
                Process.Start(filePathPDF);
            }, canCalcRaschetBUCange, null);
            this.OpenPoliceMailPDF = new DelegateCommand("Открыть письмо в полицию в PDF", f =>
            {
                string filePathDocx = WordShablon.CreatePoliceMailForBuAkt(BuEditVM.AktInWork);
                string filePathPDF = WordShablon.ConvertDocxToPdf(filePathDocx);
                Process.Start(filePathPDF);
            }, canCalcRaschetBUCange, null);

            this.OpenPoliceMailDocPDF = new DelegateCommand("Открыть полное письмо в полицию в PDF", f =>
            {
                string filePathPDF = WordShablon.CreatePdfPoliceMail(BuEditVM.AktInWork);
                Process.Start(filePathPDF);
            }, canCalcRaschetBUCange, null);

            this.OpenMailPDF = new DelegateCommand("Открыть письмо в PDF", f =>
            {
                string filePathDocx = WordShablon.CreateMailForBuAkts(BuEditVM.AktInWork);
                string filePathPDF = WordShablon.ConvertDocxToPdf(filePathDocx);
                Process.Start(filePathPDF);
            }, canCalcRaschetBUCange, null);

            this.OpenAgent1Docx = new DelegateCommand("Открыть объяснения в Word", f =>
            {
                Process.Start(WordShablon.CreateObysnenyaForBuAkt(BuEditVM.AktInWork, BuEditVM.AktInWork.Agent_1));
            }, canCalcRaschetBUCange, null);

            this.OpenAgent2Docx = new DelegateCommand("Открыть объяснения в Word", f =>
            {
                Process.Start(WordShablon.CreateObysnenyaForBuAkt(BuEditVM.AktInWork, BuEditVM.AktInWork.Agent_2));
            }, canCalcRaschetBUCange, null);


            this.OpenRaschetDocx = new DelegateCommand("Открыть расчет в Word", f =>
            {
                Process.Start(WordShablon.CreateRaschetForBuAkt(BuEditVM.AktInWork));
            }, canCalcRaschetBUCange, null);

            this.OpenPoliceMailDocx = new DelegateCommand("Открыть письмо в полицию в Word", f =>
            {
                Process.Start(WordShablon.CreatePoliceMailForBuAkt(BuEditVM.AktInWork));
            }, canCalcRaschetBUCange, null);

            this.OpenMailDocx = new DelegateCommand("Открыть письмо в Word", f =>
            {
                Process.Start(WordShablon.CreateMailForBuAkts(BuEditVM.AktInWork));
            }, canCalcRaschetBUCange, null);

            this.GetNormativ = new DelegateCommand("Получить норматив", f =>
            {
                BuEditVM.AktInWork.getNormativ();
            }, null, null);
            this.FindCurrentProverka = new DelegateCommand("Найти в разнесенных актах", f =>
            {
                BuEditVM.AktInWork.FindCurrentAktProverki();
            }, canSearchPderAkt, null); 

            this.GetDataFromDbByNumberLs = new DelegateCommand("Поиск по номеру лицевого счета", f =>
            {
                string number = BuEditorVM.AktInWork.NumberLS;
                List<Dictionary<String, String>> resultSearchAbonent;
                resultSearchAbonent = DataBaseWorker.GetAbonentFromLS(number);

                if (resultSearchAbonent.Count == 1)
                {
                    string edob = resultSearchAbonent[0]["EdOborudovania"];


                    BuEditorVM.AktInWork.setDataByDb(resultSearchAbonent[0]);
                }
                else if (resultSearchAbonent.Count > 1)
                {
                    ResultSearchWindow wndResult = new ResultSearchWindow(resultSearchAbonent);
                    wndResult.ShowDialog();
                    if ((bool)wndResult.DialogResult)
                    {
                        string edob = wndResult.SelectVal["EdOborudovania"];
                        BuEditorVM.AktInWork.setDataByDb(wndResult.SelectVal);
                    }
                }
            }, findByLs, null);
            this.GetDataFromDbByNumberPu = new DelegateCommand("Поиск по номеру прибора учета", f =>
            {
                string number = BuEditorVM.AktInWork.PuOldNumber;
                List<Dictionary<String, String>> resultSearchAbonent;
                resultSearchAbonent = DataBaseWorker.GetAbonentFromDbByPU(number);
                if (resultSearchAbonent.Count == 1)
                {
                    string edob = resultSearchAbonent[0]["EdOborudovania"];


                    BuEditorVM.AktInWork.setDataByDb(resultSearchAbonent[0]);
                }
                else if (resultSearchAbonent.Count > 1)
                {
                    ResultSearchWindow wndResult = new ResultSearchWindow(resultSearchAbonent);
                    wndResult.ShowDialog();
                    if ((bool)wndResult.DialogResult)
                    {
                        string edob = wndResult.SelectVal["EdOborudovania"];
                        BuEditorVM.AktInWork.setDataByDb(wndResult.SelectVal);
                    }
                }
            }, findByPu, null);
            this.NumberUp = new DelegateCommand("+1", f =>
            {
                BuEditorVM.AktInWork.Number++;
            }, canNumberCange, null);
            this.NumberDown = new DelegateCommand("-1", f =>
            {
                BuEditorVM.AktInWork.Number--;
            }, canNumberCange, null);

            this.RemoveSecondAgent = new DelegateCommand("Очистить", f =>
            {
                BuEditorVM.AktInWork.Agent_2 = null;
            }, canRemoveAgent, null);
            this.OpenImage = new DelegateCommand("Открыть изображение", openPhoto, null, null);
        }

        private bool canSearchPderAkt(object obj)
        {
            return BuEditVM.AktInWork.NumberLS != null && BuEditVM.AktInWork.DateWork != null;
        }

        private bool canCalcRaschetBUCange(object obj)
        {
            bool result = false;
            if (BuEditVM.AktInWork.FIO != null) result = true;
            return result;
        }

        private void openPhoto(object obj)
        {
            if (obj is string)
            {
                var windowPhoto = new PhotoView(BuEditVM.SelectedPhoto);

                windowPhoto.Show();
            }
        }


    }
}
