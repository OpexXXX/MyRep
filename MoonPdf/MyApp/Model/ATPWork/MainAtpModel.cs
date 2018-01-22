﻿using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Threading;
using System.IO;
using System.Windows;

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
        #endregion
        private static string _aktDirektory;
        public static string AktDirektory
        {
            get { return _aktDirektory; }
            set { _aktDirektory = value;

            }
        }
        public static void InitMainAtpModel()
        {
            InitListsForCombos();
            InitAllAktCollection();
        }
        public static void InitAllAktCollection()
        {
            AllAkt = new List<AktTehProverki>(DataBaseWorker.LoadCompleteATP());
            AllAtpRefreshRefresh?.Invoke();
        }
        public static void InitListsForCombos()
        {
            NewPuList = DataBaseWorker.PUListInit();
            AgentList = DataBaseWorker.AgentListInit();
            TypePlobm = DataBaseWorker.TypePlombListInit();
            PlacePlomb = DataBaseWorker.PlacePlombListInit();
            ComboRefresh?.Invoke();
        }
        public static  void CreateWorkFromPdf(string pathOfPdfFile, IProgress<double> progress, bool addedToCurentWork = false)
        {
                PdfReader iTextPDFReader = new PdfReader(pathOfPdfFile); //Загружаем документ в iTextPdf
                ITextExtractionStrategy strategyOfFinder = new SimpleTextExtractionStrategy();
                List<int> listOfpagesInPDf = new List<int>(); // Временно для хранения массива номеров страниц для каждого акта
                List<AktTehProverki> addedAkt = new List<AktTehProverki>();
                int maxIDAkt = 0;
                if ((AllAktInCurrentWork.Count > 0) && addedToCurentWork) maxIDAkt = AllAktInCurrentWork.Max(aktATP => aktATP.ID);
                double countAdded = 0;
                for (int i = 0; i < (iTextPDFReader.NumberOfPages / 2); i++) // перебираем страницы
                {
                    listOfpagesInPDf.Add((i + 1) * 2 - 2);
                    listOfpagesInPDf.Add((i + 1) * 2 - 1);
                    addedAkt.Add(new AktTehProverki(i + 1 + maxIDAkt, listOfpagesInPDf, pathOfPdfFile)); //добавляем в лист проверок объекты 
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
                if (addedToCurentWork)
                {
                    foreach (AktTehProverki item in addedAkt)
                    {
                        AllAktInCurrentWork.Add(item);
                    }
                }
                else
                {
                    AllAktInCurrentWork = addedAkt;
                }
                iTextPDFReader.Close();
                CurrentWorkRefresh?.Invoke();
         }
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
        private static void  createAktPdf(AktTehProverki akt)
        {
            string FileName = akt.Number.ToString() + " " + akt.DateWork?.ToString("d") + "" + akt.NumberLS + ".pdf";
            string FilePath = AktDirektory + "\\" + FileName;
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
        public static void blindPdf(List<AktTehProverki> akts, string folderPath)
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
                FileName = "Допуски.pdf";
                FilePath = folderPath + "\\" + FileName;
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
                            iTextSharp.text.pdf.PdfReader ReaderDoc1 = new iTextSharp.text.pdf.PdfReader(AktDirektory + item.NamePdfFile);
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
                FileName = "Проверки.pdf";
                FilePath = folderPath + "\\" + FileName;
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
                            iTextSharp.text.pdf.PdfReader ReaderDoc1 = new iTextSharp.text.pdf.PdfReader(AktDirektory + item.NamePdfFile);
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
        public static int MoveComleteAtp(IProgress<double> progress)
        {
            int i = 0;
            foreach (AktTehProverki item in AllAktInCurrentWork)
            {
                item.checkToComplete();
                if (item.Complete)
                {
                    if (!AllAkt.Contains(item))
                    {
                        AllAkt.Add(item);
                        createAktPdf(item);
                        if (!DataBaseWorker.chekForContainsCompleteAktATP(item)) DataBaseWorker.InsertCompleteAktAPT(item);
                        i++;
                        double rep = (100.0 * ((double)i / (double)AllAktInCurrentWork.Count));
                        progress.Report(rep);
                    }
                }
            }
            return i;
        }
    }
}


