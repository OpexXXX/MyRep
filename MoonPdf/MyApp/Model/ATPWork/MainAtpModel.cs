using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;
using System.Windows;
using Ionic.Zip;
using System.Text;
using MoonPdf;
using ATPWork.Properties;
using ATPWork.MyApp.Model;

namespace MyApp.Model

{
    static class MainAtpModel
    {
        #region Коллекции для comboBox's
        public delegate void ComboListRefreshHandler();
        public static event ComboListRefreshHandler ComboRefresh;
        private static List<PriborUcheta> _newPuList = new List<PriborUcheta>();
        public static List<PriborUcheta> NewPuList
        {
            get { return _newPuList; }
            set
            {
                _newPuList = value;
            }
        }
        private static List<Agent> _agentList = new List<Agent>();
        public static List<Agent> AgentList
        {
            get { return _agentList; }
            set { _agentList = value; }
        }
        private static List<string> _typePL = new List<string>();
        public static List<string> TypePlobm
        {
            get { return _typePL; }
            set { _typePL = value; }
        }
        private static List<string> _placePL = new List<string>();
        public static List<string> PlacePlomb
        {
            get { return _placePL; }
            set { _placePL = value; }
        }


        #endregion
        #region коллекция актов
        public delegate void AllAtpListRefreshHandler();
        public static event AllAtpListRefreshHandler AllAtpRefreshRefresh;
        public delegate void CurrentWorkListRefreshHandler();
        public static event CurrentWorkListRefreshHandler CurrentWorkRefresh;
        private static List<AktTehProverki> _allAkt = new List<AktTehProverki>();

        internal static List<string[]> GetAktsForVneplan(string numberLS)
        {
            List<string[]> akts = new List<string[]>();
            foreach (var item in AllAkt)
            {
                if (item.NumberLS == numberLS)
                {
                    akts.Add(new string[] { item.Number.ToString(), item.DateWork?.ToString("d") });
                }
            }
            return akts;
        }

        public static List<AktTehProverki> AllAkt
        {
            get { return _allAkt; }
            set { _allAkt = value; }
        }
        private static List<AktTehProverki> _allAktInCurrentWork = new List<AktTehProverki>();
        public static List<AktTehProverki> AllAktInCurrentWork
        {
            get { return _allAktInCurrentWork; }
            set { _allAktInCurrentWork = value; }
        }

        internal static AktTehProverki GetAtpFromComplete(string numberLS, DateTime? dateWork)
        {

            foreach (var item in AllAkt)
            {
                if (item.NumberLS == numberLS && item.DateWork == dateWork) return item;
            }
            return null;
        }

        public static List<AktTehProverki> UnmailedAkt
        {
            get
            {
                List<AktTehProverki> result = new List<AktTehProverki>();
                foreach (AktTehProverki item in _allAkt)
                {
                    if (item.DateMail == null)
                    {
                        result.Add(item);
                    }
                }
                return result;
            }
        }
        #endregion
        private static string _aktDirektory = Environment.CurrentDirectory;
        public static string AktDirektory
        {
            get { return _aktDirektory; }
            set
            {
                _aktDirektory = value;

            }
        }
        private static string _mialDirektory = Environment.CurrentDirectory;
        public static string MailDirektory
        {
            get { return _mialDirektory; }
            set
            {
                _mialDirektory = value;

            }
        }
        #region Инициализация 
        internal static void LoadSettings(Settings settings)
        {
            AktDirektory = settings.DirAktTehPDF;
            MailDirektory = settings.DirAktTehMail;
        }

        public static void InitMainAtpModel()
        {
            InitListsForCombos();
            InitCompleteAktCollection();
            InitAktInWorkCollection();
        }
        public static void InitCompleteAktCollection()
        {
            AllAkt = new List<AktTehProverki>(DataBaseWorker.LoadCompleteATP());
            AllAtpRefreshRefresh?.Invoke();
        }
        public static void InitAktInWorkCollection()
        {
            AllAktInCurrentWork = new List<AktTehProverki>(DataBaseWorker.LoadATPInWork());
            CurrentWorkRefresh?.Invoke();
        }
        public static void InitListsForCombos()
        {
            NewPuList = DataBaseWorker.PUListInit();
            AgentList = DataBaseWorker.AgentListInit();
            TypePlobm = DataBaseWorker.TypePlombListInit();
            PlacePlomb = DataBaseWorker.PlacePlombListInit();
            ComboRefresh?.Invoke();
        }
        #endregion
        /// <summary>
        /// Добавление актов в работу из  PDF
        /// </summary>
        /// <param name="pathOfPdfFile"></param>
        /// <param name="progress"></param>
        public static void CreateWorkFromPdf(string pathOfPdfFile, IProgress<double> progress)
        {
            FileInfo file = new FileInfo(pathOfPdfFile);

            PdfReader iTextPDFReader = new PdfReader(pathOfPdfFile); //Загружаем документ в iTextPdf
            ITextExtractionStrategy strategyOfFinder = new SimpleTextExtractionStrategy();
            List<int> listOfpagesInPDf = new List<int>(); // Временно для хранения массива номеров страниц для каждого акта
            List<AktTehProverki> addedAkt = new List<AktTehProverki>();
            int maxIDAkt = 0;
            if (AllAktInCurrentWork.Count > 0) maxIDAkt = AllAktInCurrentWork.Max(aktATP => aktATP.ID);
            double countAdded = 0;
            long sizeAktPdf = ((file.Length / 1024) / iTextPDFReader.NumberOfPages) * 2;

            for (int i = 0; i < (iTextPDFReader.NumberOfPages / 2); i++) // перебираем страницы
            {
                listOfpagesInPDf.Add((i + 1) * 2 - 2);
                listOfpagesInPDf.Add((i + 1) * 2 - 1);
                addedAkt.Add(new AktTehProverki(i + 1 + maxIDAkt, listOfpagesInPDf, pathOfPdfFile, sizeAktPdf)); //добавляем в лист проверок объекты 
                listOfpagesInPDf.Clear();
                countAdded++;
            }
            int ii = 0;
            foreach (AktTehProverki item in addedAkt)
            {
                string textOfPage = GetTextOfPdfPage(item.NumberOfPagesInSoursePdf[0], iTextPDFReader);
                item.DopuskFlag = (textOfPage.Contains("допуска"));
                foreach (var agent in AgentList)
                {
                    string search_text = agent.SearchString;
                    if (textOfPage.Contains(search_text) && item.Agent_1 == null)
                    {
                        item.Agent_1 = agent;
                        continue;
                    }
                    if (textOfPage.Contains(search_text) && item.Agent_1 != null)
                    {
                        item.Agent_2 = agent;
                    }
                }
                ii++;
                double rep = (100.0 * ((double)ii / (double)addedAkt.Count));
                progress.Report(rep);
            }

            foreach (AktTehProverki item in addedAkt)
            {
                AllAktInCurrentWork.Add(item);
            }

            iTextPDFReader.Close();
            CurrentWorkRefresh?.Invoke();
        }
        /// <summary>
        /// Получение текста страницы PDF (для поиска агентов, типа акта)
        /// </summary>
        /// <param name="indexPageForSearch"></param>
        /// <param name="iTextPDFReader"></param>
        /// <returns></returns>
        private static string GetTextOfPdfPage(int indexPageForSearch, PdfReader iTextPDFReader)
        {

            try
            {
                string currentPageText = PdfTextExtractor.GetTextFromPage(iTextPDFReader, indexPageForSearch + 1);  //извлекаем текст из страницы
                return currentPageText;
            }
            catch (Exception)
            {
                return "";
            }

        }
        /// <summary>
        /// Выдергивает страницы из исходного pdf, сохраняет в AktDirectory, меняет Name PDF File в акте тех проверки (Для обработки заполненных актов) 
        /// </summary>
        /// <param name="akt"></param>
        private static void createAktPdf(AktTehProverki akt)
        {
            string FileName = akt.Number.ToString() + " " + akt.DateWork?.ToString("d") + " " + akt.NumberLS + ".pdf";

            string FilePath = System.IO.Path.Combine(AktDirektory, FileName);

            try
            {
                using (FileStream FStream = new System.IO.FileStream(FilePath, System.IO.FileMode.Create))
                {
                    iTextSharp.text.Document doc = new iTextSharp.text.Document();
                    iTextSharp.text.pdf.PdfReader ReaderDoc1 = new iTextSharp.text.pdf.PdfReader(akt.NamePdfFile);
                    iTextSharp.text.pdf.PdfCopy Writer = new iTextSharp.text.pdf.PdfCopy(doc, FStream);
                    Writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_5);
                    Writer.SetFullCompression();
                    Writer.CompressionLevel = PdfStream.BEST_COMPRESSION;
                    doc.Open();
                    List<int> Pages = new List<int>();
                    for (int ii = 0; ii < akt.NumberOfPagesInSoursePdf.Count; ii++)
                    {
                        Pages.Add(ii);
                        Writer.AddPage(Writer.GetImportedPage(ReaderDoc1, akt.NumberOfPagesInSoursePdf[ii] + 1));
                    }
                    akt.NumberOfPagesInSoursePdf.Clear();
                    foreach (var item in Pages)
                    {
                        akt.NumberOfPagesInSoursePdf.Add(item);
                    }
                    doc.Close();
                    akt.NamePdfFile = FileName;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Склеить акты из List в Допуски.pdf Проверки.pdf
        /// </summary>
        /// <param name="akts"></param>
        /// <param name="folderPath"></param>
        /// <param name="progress"></param>
        private static void blindPdf(List<AktTehProverki> akts, string folderPath, IProgress<string> progress = null)
        {
            List<AktTehProverki> proverki = new List<AktTehProverki>();
            List<AktTehProverki> dopuski = new List<AktTehProverki>();
            foreach (AktTehProverki item in akts)
            {
                if (item.DopuskFlag) dopuski.Add(item);
                else proverki.Add(item);
            }
            string FileName, FilePath;
            if (dopuski.Count > 0)
            {
                progress.Report("Допуски.pdf");
                FileName = "Допуски.pdf";
                FilePath = System.IO.Path.Combine(folderPath, FileName);
                try
                {
                    using (FileStream FStream = new System.IO.FileStream(FilePath, System.IO.FileMode.Create))
                    {

                        iTextSharp.text.Document doc = new iTextSharp.text.Document();
                        iTextSharp.text.pdf.PdfCopy Writer = new iTextSharp.text.pdf.PdfCopy(doc, FStream);
                        Writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_5);
                        Writer.SetFullCompression();
                        Writer.CompressionLevel = PdfStream.BEST_COMPRESSION;
                        doc.Open();
                        foreach (var item in dopuski)
                        {
                            iTextSharp.text.pdf.PdfReader ReaderDoc1 = new iTextSharp.text.pdf.PdfReader(System.IO.Path.Combine(AktDirektory, item.NamePdfFile));
                            Writer.AddDocument(ReaderDoc1);
                        }
                        doc.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (proverki.Count > 0)
            {
                progress.Report("Проверки.pdf");
                FileName = "Проверки.pdf";
                FilePath = System.IO.Path.Combine(folderPath, FileName);
                try
                {
                    using (FileStream FStream = new System.IO.FileStream(FilePath, System.IO.FileMode.Create))
                    {
                        iTextSharp.text.Document doc = new iTextSharp.text.Document();
                        iTextSharp.text.pdf.PdfCopy Writer = new iTextSharp.text.pdf.PdfCopy(doc, FStream);
                        Writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_5);
                        Writer.SetFullCompression();
                        Writer.CompressionLevel = PdfStream.BEST_COMPRESSION;
                        doc.Open();
                        foreach (var item in proverki)
                        {
                            iTextSharp.text.pdf.PdfReader ReaderDoc1 = new iTextSharp.text.pdf.PdfReader(System.IO.Path.Combine(AktDirektory, item.NamePdfFile));
                            Writer.AddDocument(ReaderDoc1);
                        }
                        doc.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        /// <summary>
        /// Создать сопроводительное письмо из листа заполненных актов (только не отправленных ранее)
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="numberMail"></param>
        /// <param name="dateMail"></param>
        /// <param name="akts"></param>
        public static void CreateMailATP(IProgress<string> progress, int numberMail, DateTime dateMail)
        {
            progress.Report(">=======================================<");
            List<AktTehProverki> TempList = new List<AktTehProverki>();
            string mailName = "исх.№91-" + numberMail + " от " + dateMail.ToString("d") + "г. Акты ПР ФЛ";
            string currentMailDirectory = System.IO.Path.Combine(MailDirektory, mailName);
            progress.Report("Создаем папку сопроводительного письма: " + currentMailDirectory);
            if (!Directory.Exists(currentMailDirectory)) Directory.CreateDirectory(currentMailDirectory);
            foreach (AktTehProverki item in AllAkt)
            {
                item.checkToComplete();
                bool mailed = item.DateMail == null;
                if (mailed)
                {
                    string filePath = System.IO.Path.Combine(AktDirektory, item.NamePdfFile);
                    bool PdfExist = File.Exists(filePath);
                    if (PdfExist) TempList.Add(item);
                    else progress.Report("Не найден pdf фаил " + item.NamePdfFile);
                }
            }
            if (TempList.Count > 0)
            {
                progress.Report(TempList.Count + " актов для отправки. ");
                progress.Report("Склеиваем Pdf файлы актов.");
                blindPdf(TempList, currentMailDirectory, progress);
                progress.Report("Создаем Реестр.xlsx");
                ExcelWorker.DataTableToExcel(TempList, currentMailDirectory);
                progress.Report("Создаем Реестр в PDF");
                ExcelWorker.CreatePdfReestr(currentMailDirectory + "\\" + "Reestr.pdf", TempList);
                progress.Report("Создаем сопроводительное письмо");
                string pathDocX = WordShablon.CreateMailForAktsTehProverki(TempList, dateMail, numberMail, 1, currentMailDirectory);
                progress.Report(pathDocX);
                progress.Report("Конвертируем в PDF письмо");
                string pathPdf = WordShablon.ConvertDocxToPdf(pathDocX);
                progress.Report("Клеим письмо с реестром");

                string pathMailPdf =System.IO.Path.Combine(currentMailDirectory, mailName + " .pdf");
                margePdfForMail(pathMailPdf, pathPdf, currentMailDirectory+ "\\Reestr.pdf");
                progress.Report("Архивируем для отправки.");
                using (ZipFile zip = new ZipFile()) // Создаем объект для работы с архивом
                {
                    zip.UseUnicodeAsNecessary = true;
                    zip.ProvisionalAlternateEncoding = System.Text.Encoding.GetEncoding("cp866");
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression; // Задаем максимальную степень сжатия 
                    if (File.Exists(currentMailDirectory + "\\" + "Проверки.pdf")) zip.AddFile(currentMailDirectory + "\\" + "Проверки.pdf", "\\"); // Кладем в архив одиночный файл
                    if (File.Exists(currentMailDirectory + "\\" + "Допуски.pdf")) zip.AddFile(currentMailDirectory + "\\" + "Допуски.pdf", "\\"); // Кладем в архив одиночный файл
                    if (File.Exists(currentMailDirectory + "\\" + "Реестр.xlsx")) zip.AddFile(currentMailDirectory + "\\" + "Реестр.xlsx", "\\"); // Кладем в архив одиночный файл
                    if (File.Exists(pathMailPdf)) zip.AddFile(pathMailPdf, "\\"); 
                    var g = zip.Count;
                    zip.Save(currentMailDirectory + "\\" + mailName + ".zip"); // Создаем архив     
                    progress.Report(zip.Info);
                }
                progress.Report("Обновляем состояние актов");
                foreach (AktTehProverki item in TempList)
                {
                    item.DateMail = dateMail;
                    item.NumberMail = numberMail;
                }
                AllAtpRefreshRefresh?.Invoke();
                progress.Report(">=======================================<");
            }
            else
            {
                progress.Report("Нечего отправлять");
                progress.Report(">=======================================<");
            }
        }

        private static void margePdfForMail(string pathOutPdf, string firstPDF, string secondPDF)
        {
           
            try
            {
                using (FileStream FStream = new System.IO.FileStream(pathOutPdf, System.IO.FileMode.Create))
                {

                    iTextSharp.text.Document doc = new iTextSharp.text.Document();
                    iTextSharp.text.pdf.PdfCopy Writer = new iTextSharp.text.pdf.PdfCopy(doc, FStream);
                    Writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_5);
                    Writer.SetFullCompression();
                    Writer.CompressionLevel = PdfStream.BEST_COMPRESSION;
                    doc.Open();

                    iTextSharp.text.pdf.PdfReader ReaderDoc1 = new iTextSharp.text.pdf.PdfReader(firstPDF);
                    Writer.AddDocument(ReaderDoc1);
                    ReaderDoc1 = new iTextSharp.text.pdf.PdfReader(secondPDF);
                    Writer.AddDocument(ReaderDoc1);
                    doc.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }






        /// <summary>
        /// Создать сопроводительное письмо из листа актов (в том числе не отправленных ранее)
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="numberMail"></param>
        /// <param name="dateMail"></param>
        /// <param name="akts"></param>
        public static void CreateMailATP(IProgress<string> progress, int numberMail, DateTime dateMail, List<AktTehProverki> akts)
        {
            List<AktTehProverki> TempList = new List<AktTehProverki>();
            string mailName = "исх.№91-" + numberMail + " от " + dateMail.ToString("d") + "г. Акты ПР ФЛ";
            string currentMailDirectory = System.IO.Path.Combine(MailDirektory, mailName);
            if (!Directory.Exists(currentMailDirectory)) Directory.CreateDirectory(currentMailDirectory);
            foreach (AktTehProverki item in akts)
            {
                item.checkToComplete();
                string filePath = System.IO.Path.Combine(AktDirektory, item.NamePdfFile);
                bool PdfExist = File.Exists(filePath);
                if (PdfExist) TempList.Add(item);
            }
            if (TempList.Count > 0)
            {
                blindPdf(TempList, currentMailDirectory);
                ExcelWorker.DataTableToExcel(TempList, currentMailDirectory);
                foreach (AktTehProverki item in TempList)
                {
                    item.DateMail = dateMail;
                    item.NumberMail = numberMail;
                    AllAtpRefreshRefresh?.Invoke();
                }
            }

        }
        /// <summary>
        /// Занесение в SAP Актов тех. проверок (в том числе повторно)
        /// </summary>
        /// <param name="akts"></param>
        /// <param name="logProgress"></param>
        /// <param name="progress"></param>
        public static void EnterInSapAkts(List<AktTehProverki> akts, IProgress<string> logProgress = null, IProgress<double> progress = null)
        {
            SAPActive sAP = new SAPActive("ER2");
            try
            {
                sAP.login();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            int i = 0;
            foreach (var item in akts)
            {
                if ((File.Exists(System.IO.Path.Combine(AktDirektory, item.NamePdfFile))))
                {
                    try
                    {
                        sAP.enterAktTehProverki(item, AktDirektory);
                        DataBaseWorker.DromCompliteTable();
                        DataBaseWorker.InsertCompleteAktAPT(AllAkt);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            sAP.CloseApp();
        }
        /// <summary>
        /// Занесение в SAP Акта тех. проверок (в том числе повторно)
        /// </summary>
        /// <param name="akts"></param>
        /// <param name="logProgress"></param>
        /// <param name="progress"></param>
        public static void EnterInSapAkts(AktTehProverki akt, IProgress<string> logProgress = null, IProgress<double> progress = null)
        {
            SAPActive sAP = new SAPActive("ER2");
            try
            {
                sAP.login();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            if ((File.Exists(System.IO.Path.Combine(AktDirektory, akt.NamePdfFile))))
            {
                try
                {
                    sAP.enterAktTehProverki(akt, AktDirektory);
                    DataBaseWorker.DromCompliteTable();
                    DataBaseWorker.InsertCompleteAktAPT(AllAkt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            sAP.CloseApp();
        }
        /// <summary>
        /// Занесение в SAP Актов тех. проверок (только не занесенных)
        /// </summary>
        /// <param name="akts"></param>
        /// <param name="logProgress"></param>
        /// <param name="progress"></param>
        public static void EnterInSapAllPosibleAkts(IProgress<string> logProgress, IProgress<double> progress)
        {
            SAPActive sAP = new SAPActive("ER2");
            try
            {
                sAP.login();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            int i = 0;
            foreach (var item in AllAkt)
            {
                if ((item.SapNumberAkt == "") && (File.Exists(System.IO.Path.Combine(AktDirektory, item.NamePdfFile))))
                {
                    try
                    {
                        sAP.enterAktTehProverki(item, AktDirektory);

                        DataBaseWorker.DromCompliteTable();
                        DataBaseWorker.InsertCompleteAktAPT(AllAkt);

                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                }

            }

            sAP.CloseApp();
        }
        /// <summary>
        /// Обработать заполненные акты
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static int MoveComleteAtp(IProgress<double> progress)
        {
            int i = 0;
            List<AktTehProverki> Complete = new List<AktTehProverki>();

            foreach (AktTehProverki item in AllAktInCurrentWork)
            {
                item.checkToComplete();
                if (item.Complete)
                {
                    Complete.Add(item);
                }
            }

            foreach (AktTehProverki item in Complete)
            {
                createAktPdf(item);
                AllAkt.Add(item);
                AllAktInCurrentWork.Remove(item);
                i++;
                double rep = (100.0 * ((double)i / (double)Complete.Count));
                progress.Report(rep);
            }

            if (i > 0)
            {
                AllAtpRefreshRefresh?.Invoke();
                CurrentWorkRefresh?.Invoke();
            }
            return i;
        }
        /// <summary>
        /// Сохраняет коллекции актов в SQLite
        /// </summary>
        internal static void SaveBeforeCloseApp()
        {
            DataBaseWorker.DromCompliteTable();
            DataBaseWorker.InsertCompleteAktAPT(AllAkt);
            DataBaseWorker.DromInWorkTable();
            DataBaseWorker.InsertAPTInWork(AllAktInCurrentWork);
        }
    }
}


