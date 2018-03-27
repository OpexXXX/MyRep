using ATPWork.MyApp.Model;
using ATPWork.MyApp.View;
using MyApp;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public DelegateCommand FindPrevousProverka { get; private set; }
        public DelegateCommand OpenRaschetDocx { get; private set; }
        public DelegateCommand OpenMailDocx { get; private set; }
        public DelegateCommand OpenPoliceMailDocx { get; private set; }
        public DelegateCommand OpenAgent1Docx { get; private set; }
        public DelegateCommand OpenAgent2Docx { get; private set; }

        public DelegateCommand OpenRaschetPDF { get; private set; }
        public DelegateCommand OpenMailPDF { get; private set; }
        public DelegateCommand OpenPoliceMailPDF { get; private set; }
        public DelegateCommand OpenAgent1PDF { get; private set; }
        public DelegateCommand OpenAgent2PDF { get; private set; }
        public DelegateCommand OpenFullMailDocPDF { get; private set; }
        public DelegateCommand OpenPoliceMailDocPDF { get; private set; }
        public DelegateCommand OpenPhotoInPDF { get; private set; }
        public DelegateCommand SaveOnPublic { get; private set; }

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
            this.SaveOnPublic = new DelegateCommand("Сохранить пакет документов на паблик", async f =>
            {
                string SelectedPath = "";
                FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                DialogResult result = folderBrowser.ShowDialog();
                if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            SelectedPath = folderBrowser.SelectedPath;
                            //Создаем директории
                            string nameDirectory = "Акт БУ№" + BuEditVM.AktInWork.Number + " от " + BuEditVM.AktInWork.DateWork?.ToString("d") +" "+ BuEditVM.AktInWork.FIO + " Ермаковский РЭС";
                            string folderPath = Path.Combine(SelectedPath, nameDirectory);
                            string photoFolderPath = Path.Combine(SelectedPath, nameDirectory, "Фото");
                            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath); //Рабочая директория
                            if (!Directory.Exists(photoFolderPath)) Directory.CreateDirectory(photoFolderPath); //Директория с фото
                            //Готовим файлы
                            //Расчет
                            string filePathRaschetDocx = WordShablon.CreateRaschetForBuAkt(BuEditVM.AktInWork);
                            string filePathRaschetPDF = WordShablon.ConvertDocxToPdf(filePathRaschetDocx);
                            //Сопроводиловка в сбыт
                            string filePathMailDocx = WordShablon.CreateMailForBuAkts(BuEditVM.AktInWork);
                            string filePathMailPDF = WordShablon.ConvertDocxToPdf(filePathMailDocx);
                            //Копируем
                            //Фото
                            int i = 1;
                            foreach (var item in BuEditVM.AktInWork.PhotoFile)
                            {
                                string ext = Path.GetExtension(item);
                                File.Copy(item, Path.Combine(photoFolderPath, i + "." + ext));
                                i++;
                            }
                            //3. Извещение потребителя о предстоящей проверке.pdf
                            File.Copy(BuEditVM.AktInWork.IzvesheniePDF, Path.Combine(folderPath, "3. Извещение потребителя о предстоящей проверке.pdf"));
                            //4. Акт проверки №91-Е-186 от 15.02.2018г.
                            File.Copy(BuEditVM.AktInWork.AktPredProverkiPdf, Path.Combine(folderPath, "4. Акт проверки " + BuEditVM.AktInWork.AktPedProverki + ".pdf"));
                            // 5.Акт БУ №241603578 от 15.02.2018г.
                            File.Copy(BuEditVM.AktInWork.AktBuPdf, Path.Combine(folderPath, "5.Акт БУ №" + BuEditVM.AktInWork.Number + " от " + BuEditVM.AktInWork.DateWork?.ToString("d") + ".pdf"));
                            //6. Расчет к акту БУ №241603578 от 15.02.2018г
                            File.Copy(filePathRaschetPDF, Path.Combine(folderPath, "6. Расчет к акту БУ №" + BuEditVM.AktInWork.Number + " от " + BuEditVM.AktInWork.DateWork?.ToString("d") + ".pdf"));
                            //7. исх.№91-68 от 19.02.2018г Акт БУ ФЛ
                            File.Copy(filePathMailPDF, Path.Combine(folderPath, "7. исх.№91-" + BuEditVM.AktInWork.NumberMail + " от " + BuEditVM.AktInWork.DateMail?.ToString("d") + "г Акт БУ ФЛ.pdf"));
                            //11. Акт последней проверки
                            if (BuEditVM.AktInWork.AktPredidProverkiPdf != null & BuEditVM.AktInWork.AktPredidProverkiPdf != "")
                            {
                                File.Copy(BuEditVM.AktInWork.AktPredidProverkiPdf, Path.Combine(folderPath, "11. Акт последней проверки " + BuEditVM.AktInWork.AktPedidProverki + ".pdf"));
                            }
                            //15. ПЗ
                            //16. Содержание
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("123" + ex.Message);
                        }
                    });
                }
            }, null, null);
            this.OpenPhotoInPDF = new DelegateCommand("Открыть фото в  PDF", async f =>
            {
                await Task.Run(() =>
                {
                    string filePathPDF = WordShablon.CreatePdfWithPhoto(BuEditVM.AktInWork);
                    Process.Start(filePathPDF);
                });


            }, null, null);
            this.CalcRaschet = new DelegateCommand("Расчитать", f =>
            {

                BuEditVM.AktInWork.calcBu();

            }, canCalcRaschetBUCange, null);
            this.OpenAgent1PDF = new DelegateCommand("Открыть объяснения в PDF", f =>
            {
                string filePathDocx = WordShablon.CreateObysnenyaForBuAkt(BuEditVM.AktInWork, BuEditVM.AktInWork.Agent_1);
                string filePathPDF = WordShablon.ConvertDocxToPdf(filePathDocx);
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
            this.OpenPoliceMailDocPDF = new DelegateCommand("Открыть полное письмо в полицию в PDF", async f =>
            {
                await Task.Run(() =>
                {
                    string filePathPDF = WordShablon.CreatePdfPoliceMail(BuEditVM.AktInWork);
                    Process.Start(filePathPDF);
                });
            }, canCalcRaschetBUCange, null);
            this.OpenFullMailDocPDF = new DelegateCommand("Открыть полное письмо в сбыт в PDF", async f =>
            {
                await Task.Run(() =>
                {
                    string filePathPDF = WordShablon.CreateFullPdfMail(BuEditVM.AktInWork);
                    Process.Start(filePathPDF);
                });
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

            this.FindPrevousProverka = new DelegateCommand("Найти предыдущую проверку", f =>
            {
                BuEditVM.AktInWork.FindPrevousAktProverki();
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
