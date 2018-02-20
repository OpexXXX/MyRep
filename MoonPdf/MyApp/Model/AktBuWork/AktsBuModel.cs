using ATPWork.MyApp.View;
using iTextSharp.text.pdf;
using MoonPdf;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ATPWork.MyApp.Model.AktBuWork
{
    static class  AktsBuModel
    {
        #region коллекция актов
        public delegate void AllAtpListRefreshHandler();
        public static event AllAtpListRefreshHandler AllAtpRefreshRefresh;
        private static List<AktBu> _allAkt = new List<AktBu>();
        public static List<AktBu> AllAkt
        {
            get { return _allAkt; }
            set { _allAkt = value; }
        }
        public static List<AktBu> UnmailedAkt
        {
            get
            {
                List<AktBu> result = new List<AktBu>();
                foreach (AktBu item in _allAkt)
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
        internal static void LoadSettings(ATPWork.Properties.Settings settings)
        {
            AktDirektory = settings.DirAktBuPDF;
            MailDirektory = settings.DirAktBuMail;
        }
        public static void InitAktBuModel()
        {
            var prop = new ATPWork.Properties.Settings();
            LoadSettings(prop);
            InitAktCollection();
        }
        public static void InitAktCollection()
        {
            AllAkt = new List<AktBu>(DataBaseWorker.LoadAktsBu());
            AllAtpRefreshRefresh?.Invoke();
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

                string pathMailPdf = System.IO.Path.Combine(currentMailDirectory, mailName + " .pdf");
                margePdfForMail(pathMailPdf, pathPdf, currentMailDirectory + "\\Reestr.pdf");
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
                    FileInfo f = new FileInfo(currentMailDirectory + "\\" + mailName + ".zip");
                    long filesize = f.Length / (1024); // file size in bytes  
                    progress.Report("Размер архива: " + filesize.ToString() + "кБ");
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
/// <summary>
/// Склеивает две PDF
/// </summary>
/// <param name="pathOutPdf"></param>
/// <param name="firstPDF"></param>
/// <param name="secondPDF"></param>
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
        public static void CreateMail(IProgress<string> progress,AktBu akt)
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
        /// Сохраняет коллекции актов в SQLite
        /// </summary>
        internal static void SaveBeforeCloseApp()
        {
            DataBaseWorker.DromTableAktsBU();
            DataBaseWorker.InsertAktsBU(AllAkt);
        }
    }
}
