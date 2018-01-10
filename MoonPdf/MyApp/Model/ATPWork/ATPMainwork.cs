using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Win32;
using MoonPdfLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using winForms = System.Windows.Forms;
using System.Windows.Media.Animation;


namespace MyApp.Model
{
    public class SpisokPUObserv : ObservableCollection<PriborUcheta>
    {
        public SpisokPUObserv() { }
    }
    public class PriborUcheta : INotifyPropertyChanged, IEquatable<PriborUcheta>
    {

        private string sapNumberPU;
        public string SapNumberPU
        {
            get { return this.sapNumberPU; }
            set { this.sapNumberPU = value; this.OnPropertyChanged("SapNumberPU"); }
        }
        private string nazvanie;
        public string Nazvanie
        {
            get { return this.nazvanie; }
            set { this.nazvanie = value; this.OnPropertyChanged("Nazvanie"); }
        }
        private int poverka;
        public int Poverka
        {
            get { return this.poverka; }
            set { this.poverka = value; this.OnPropertyChanged("Poverka"); }
        }
        private string znachnost;
        public string Znachnost
        {
            get { return this.znachnost; }
            set { this.znachnost = value; this.OnPropertyChanged("Znachnost"); }
        }


        public PriborUcheta(string SapNumberPU, string Nazvanie, int Poverka, string Znachnost)
        {
            this.SapNumberPU = SapNumberPU;
            this.Nazvanie = Nazvanie;
            this.Poverka = Poverka;
            this.Znachnost = Znachnost;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string info) // На изменение полей
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        public bool Equals(PriborUcheta item) //Сравнение
        {
            return item.sapNumberPU == this.sapNumberPU;
        }
        public override string ToString()
        {
            return this.nazvanie.ToString();
        }

    }
    public class AllATPObserv : ObservableCollection<aktATP>
    {
        public AllATPObserv() { }
    }
    public class AgentList : ObservableCollection<Agent>
    {
        public AgentList() { }
    }
    public class Agent : INotifyPropertyChanged, IEquatable<Agent>
    {
        private string sapNumber;
        public string SapNumber
        {
            get { return this.sapNumber; }
            set { this.sapNumber = value; this.OnPropertyChanged("SapNumber"); }
        }
        private string surname;
        public string Surname
        {
            get { return this.surname; }
            set { this.surname = value; this.OnPropertyChanged("Surname"); }
        }
        private string post;
        public string Post
        {
            get { return this.post; }
            set { this.post = value; this.OnPropertyChanged("Post"); }
        }
        private string searchString;
        public string SearchString
        {
            get { return this.searchString; }
            set { this.searchString = value; this.OnPropertyChanged("searchString"); }
        }
        public Agent(string SapNumber, string Post, string Surname, string SearchString)
        {
            this.SapNumber = SapNumber;
            this.Surname = Surname;
            this.Post = Post;
            this.SearchString = SearchString;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string info) // На изменение полей
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        public bool Equals(Agent item) //Сравнение
        {
            return item.sapNumber == this.sapNumber;
        }
        public override string ToString()
        {
            return this.surname.ToString();
        }
    }
    public class ATPWorker : INotifyPropertyChanged
    {
        public DataBaseWorker DbWork;
        private MoonPdfPanel pdfReader; // Ссылка на вьювер формы //???
        private PdfReader iTextPDFReader; // Конструктор пдф
        private ITextExtractionStrategy strategyOfFinder;
        /// <summary>
        /// Список агентов 
        /// </summary>
        public AgentList agents;
        /// <summary>
        /// Текущая отображаемая страница в вьювере
        /// </summary>
        private int currentPageInPDFViewer;
        /// <summary>
        /// Лист со всеми проверками 
        /// </summary>
        /// 
        private AllATPObserv completeAtpWorkList;
        public AllATPObserv CompleteAtpWorkList
        {
            get { return this.completeAtpWorkList; }
            set
            {
                this.completeAtpWorkList = value;
            }
        }

        private AllATPObserv allAtpInWorkList;
        public AllATPObserv AllAtpInWorkList
        {
            get { return this.allAtpInWorkList; }
            set
            {
                this.allAtpInWorkList = value;
            }
        }
        /// <summary>
        /// Список приборов учета
        /// </summary>
        public SpisokPUObserv SpisokPU = new SpisokPUObserv();
        /// <summary>
        /// Ссылка на текущий редактируемый акт
        /// </summary>
        private aktATP aktATPInWork;
        public aktATP AktATPInWork
        {
            get { return this.aktATPInWork; }
            set
            {
                if (aktATPInWork != null)
                {
                    AktATPInWork.checkToComplete();
                }
                if (aktATPInWork != null && value != null)
                {
                    int index_val, index_cur, delta;
                    index_cur = AllAtpInWorkList.IndexOf(aktATPInWork);
                    index_val = AllAtpInWorkList.IndexOf(value);
                    delta = index_val - index_cur;

                    if (delta == 1 || delta == (-1))
                    {


                        if (value.DateWork.Year == 0001 && aktATPInWork.DateWork.Year != 0001) value.DateWork = aktATPInWork.DateWork; //Если вносили дату в акт и в следующем она пустая - копируем дату

                        if (value.Number == null && aktATPInWork.Number != null) //Если вносили номер в акт и в следующем он пустая - пробуем добавить 1 
                        {
                            int numParse;
                            if (Int32.TryParse(aktATPInWork.Number, out numParse))
                            {
                                numParse++;
                                value.Number = numParse.ToString();
                            }
                        }

                    }
                }
                if (value != null)
                {
                    if (pdfReader.CurrentSource == null)
                    {
                        pdfReader.OpenFile(value.PathOfPdfFile);
                    }
                    MoonPdfLib.MuPdf.FileSource File_name;
                    File_name = (MoonPdfLib.MuPdf.FileSource)pdfReader.CurrentSource;
                    if (File_name.Filename == value.PathOfPdfFile)
                    {
                        pdfReader.GotoPage(value.NumberOfPagesInSoursePdf[0] + 1);
                    }
                    else
                    {
                       // pdfReader.Unload();
                        pdfReader.OpenFile(value.PathOfPdfFile);
                        pdfReader.GotoPage(value.NumberOfPagesInSoursePdf[0] + 1);
                    }
                }
                //Переходим на первую страницу акта в вьювере на форме
                this.aktATPInWork = value;
                this.OnPropertyChanged("AktATPInWork"); //евент нна изменение
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// На изменение полей
        /// </summary>
        /// <param name="info"></param>
        protected void OnPropertyChanged(string info)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        /*
                /// <summary>
                /// Переход к следующему акту
                /// </summary>
                 public void GoNextATP()
                 {

                     int currentIndex;
                     int setIndex;
                     currentIndex = AllAtpInWorkList.IndexOf(AktATPInWork);
                     setIndex = currentIndex + 1;
                     if (setIndex < AllAtpInWorkList.Count)
                     {
                         AktATPInWork = AllAtpInWorkList[setIndex];
                         if (AllAtpInWorkList[currentIndex].DateWork != null && AktATPInWork == null) AktATPInWork.DateWork = AllAtpInWorkList[currentIndex].DateWork;

                     }
                 }
                 public void GoToATP(int indexOfATPakt)
                 {
                     int currentIndex;
                     currentIndex = AllAtpInWorkList.IndexOf(AktATPInWork);
                     if (indexOfATPakt < AllAtpInWorkList.Count)
                     {
                         AktATPInWork = AllAtpInWorkList[indexOfATPakt];
                         pdfReader.GotoPage(AktATPInWork.NumberOfPagesInSoursePdf[0] + 1);
                     }
                 }

                /// <summary>
                /// Переход к предыдущему акту
                /// </summary>
                  public void GoPrevousATP()
                  {
                      int currentIndex;
                      int setIndex;
                      currentIndex = AllAtpInWorkList.IndexOf(AktATPInWork);
                      if (currentIndex != 0)
                      {
                          setIndex = currentIndex - 1;
                          AktATPInWork = AllAtpInWorkList[setIndex];
                          pdfReader.GotoPage(AktATPInWork.NumberOfPagesInSoursePdf[0] + 1);
                      }
                  }

                  */
        /// <summary>
        /// Свойство для автоматической смены текущего в работе акта исходя из стриницы в вьювере
        /// НУЖНО НАЙТИ ЕВЕНТ НА ИЗМЕНЕНИЕ СТРАНИЦЫ В ВЬЮВЕРЕ
        /// </summary>
        public int CurrentPageInPDFViewer
        {
            get
            {
                return currentPageInPDFViewer;
            }

            set
            {
                foreach (aktATP item in AllAtpInWorkList)                      //прогоняем все акты на наличие текущей страницы
                {
                    if (item.NumberOfPagesInSoursePdf.Contains(value) && AktATPInWork != item)   //Если страница найденна в акте и он не текущий
                    {
                        AktATPInWork = item;
                    }
                }
                currentPageInPDFViewer = value;
            }
        }
        /// <summary>
        /// Путь с открытому файлу ПДФ
        /// </summary>
        public string pathOfFile { get; private set; }
        private string aktDirektory;
        public string AktDirektory
        {
            get { return this.aktDirektory; }
            set
            {
                this.aktDirektory = value;
                this.OnPropertyChanged("AktDirektory");
            }
        }
        private string pathOfMailFolder;
        public string PathOfMailFolder
        {
            get { return this.pathOfMailFolder; }
            set
            {
                this.pathOfMailFolder = value;
                this.OnPropertyChanged("PathOfMailFolder");
            }
        }
        private string pathOfPDFOutFolder;
        public string PathOfPDFOutFolder
        {
            get { return this.pathOfPDFOutFolder; }
            set
            {
                this.pathOfPDFOutFolder = value;
                this.OnPropertyChanged("PathOfPDFOutFolder");
            }
        }
        private string currentSaveFileAtp;
        public string CurrentSaveFileAtp
        {
            get { return this.currentSaveFileAtp; }
            set
            {
                this.currentSaveFileAtp = value;
                this.OnPropertyChanged("CurrentSaveFileAtp");
            }
        }
        private void createAktPdf(aktATP akt)
        {
            string FileName = akt.Number.ToString() + " " + akt.DateWork.ToString("d") + "" + akt.NumberLS + ".pdf";
            string FilePath = AktDirektory + "\\" + FileName;
            try
            {
                using (FileStream FStream = new System.IO.FileStream(FilePath, System.IO.FileMode.Create))
                {
                    iTextSharp.text.Document doc = new iTextSharp.text.Document();
                    iTextSharp.text.pdf.PdfReader ReaderDoc1 = new iTextSharp.text.pdf.PdfReader(akt.PathOfPdfFile);
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
                    //  Writer.Close();
                    // ReaderDoc1.Close();
                   akt.PathOfPdfFile = FileName;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void blindPdf(AllATPObserv allATP, string folderPath)
        {
            AllATPObserv proverki = new AllATPObserv();
            AllATPObserv dopuski = new AllATPObserv();
            foreach (aktATP item in allATP)
            {
                if (item.DopuskFlag) dopuski.Add(item);
                else proverki.Add(item);
            }
            string FileName, FilePath;
            if (dopuski.Count>0)
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
                            iTextSharp.text.pdf.PdfReader ReaderDoc1 = new iTextSharp.text.pdf.PdfReader(AktDirektory+item.PathOfPdfFile);
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
            if (proverki.Count>0)
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
                            iTextSharp.text.pdf.PdfReader ReaderDoc1 = new iTextSharp.text.pdf.PdfReader(AktDirektory+item.PathOfPdfFile);
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
        public int MoveComleteAtp()
        {
            if (this.AktDirektory == "")
            {
                if (!this.selectAktFolder()) return 0;
            }
            int i = 0;
            foreach (aktATP item in AllAtpInWorkList)
            {
                item.checkToComplete();
                if (item.Complete)
                {
                    if (!CompleteAtpWorkList.Contains(item))
                    {
                        CompleteAtpWorkList.Add(item);
                        createAktPdf(item);
                        if (!DbWork.chekForContainsCompleteAktATP(item)) DbWork.InsertCompleteAktAPT(item);
                        i++;
                    }
                }
            }
            return i;
        }
        public int addAtpToWork(string pathOfPdfFile)
        {
            iTextPDFReader = new PdfReader(pathOfPdfFile); //Загружаем документ в iTextPdf
            strategyOfFinder = new SimpleTextExtractionStrategy();
            List<int> listOfpagesInPDf = new List<int>(); // Временно для хранения массива номеров страниц для каждого акта
            AllATPObserv addedAkt = new AllATPObserv();


            int maxIDAkt = 0;
            if (AllAtpInWorkList.Count > 0) maxIDAkt = AllAtpInWorkList.Max(aktATP => aktATP.ID);


            int countAdded = 0;
            for (int i = 0; i < (iTextPDFReader.NumberOfPages / 2); i++) // перебираем страницы
            {
                listOfpagesInPDf.Add((i + 1) * 2 - 2);
                listOfpagesInPDf.Add((i + 1) * 2 - 1);
                addedAkt.Add(new aktATP(i + 1 + maxIDAkt, listOfpagesInPDf, pathOfPdfFile)); //добавляем в лист проверок объекты 
                listOfpagesInPDf.Clear();
                countAdded++;
            }

            foreach (aktATP item in addedAkt)
            {
                string textOfPage = GetTextOfPdfPage(item.NumberOfPagesInSoursePdf[0]);

                item.DopuskFlag = (textOfPage.Contains("допуска"));

                foreach (var agent in agents)
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

            }
            foreach (aktATP added in addedAkt)
            {
                this.AllAtpInWorkList.Add(added);
            }
            if (AktATPInWork == null) AktATPInWork = AllAtpInWorkList[0];    // устанавливаем текущий акт проверки
            //currentPageInPDFViewer = 1;
            iTextPDFReader.Close();
            return countAdded;
        }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="pdfReader"> экземпляк PDF вьювера на форме</param>
        /// <param name="pathOfPdfFile">путь к файлу PDF</param>
        /// 
        public ATPWorker(MoonPdfPanel pdfReader) // Конструктор 
        {
            this.pdfReader = pdfReader;
            DbWork = new DataBaseWorker();
            aktDirektory = Environment.CurrentDirectory+"\\";
            PathOfMailFolder = Environment.CurrentDirectory+"\\";
            AllAtpInWorkList = new AllATPObserv();
            agents = new AgentList(); //
            SpisokPU = new SpisokPUObserv();
            CompleteAtpWorkList = new AllATPObserv();
            //DbWork.LoadCompleteATP(this);
        }
        /// <summary>
        /// Возвращает текст  на заданной странице PDF
        /// </summary>
        /// <param name="searthText">"Искомый текст"</param>
        /// <param name="indexPageForSearch">Индекс страницы</param>
        /// <returns></returns>
        private string GetTextOfPdfPage(int indexPageForSearch) //функция поиска текста на странице
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
        private void agentsListInit()
        {
            agents.Add(new Agent("40004461", "электромонтер УТЭЭ ", "Глушков Александр Сергеевич", "лушков"));
            agents.Add(new Agent("40222271", "электромонтер УТЭЭ ", "Дубинин Сергей Геннадьевич", "убинин"));
            agents.Add(new Agent("40000562", "электромонтер УТЭЭ ", "Кропочева Анна Сергеевна", "ропочева"));
            agents.Add(new Agent("40004895", "началиник УТЭЭ ", "Лавренова Светлана Николаевна", "авренова"));
            agents.Add(new Agent("40221533", "электромонтер УТЭЭ ", "Назаров Дмитрий Анатольевич", "азаров"));
            agents.Add(new Agent("40002703", "Инспектор УТЭЭ ", "Сяткина Елена Эдуардовна", "яткина"));
        }
        private void PUListInit()
        {
            SpisokPU.Add(new PriborUcheta("15", "Нева 101", 16, "5"));
            SpisokPU.Add(new PriborUcheta("16", "Меркурий 201", 16, "5"));
            SpisokPU.Add(new PriborUcheta("17", "СЕ 101", 16, "5"));
            SpisokPU.Add(new PriborUcheta("18", "ЦЭ 6803", 16, "6"));
            SpisokPU.Add(new PriborUcheta("19", "СКАТ101", 16, "5"));
            SpisokPU.Add(new PriborUcheta("20", "NP73-1.10.1", 10, "7"));
        }

        public void createMailPath(string path)
        {
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    return;
                }
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
                // Delete the directory.
            }
            catch (Exception e)
            {
                MessageBox.Show("The process failed: {0}", e.ToString());
            }
        }
        public bool selectMailFolder()
        {
            winForms.FolderBrowserDialog open = new winForms.FolderBrowserDialog();
            if (open.ShowDialog() == winForms.DialogResult.OK)
            {
                this.PathOfMailFolder = open.SelectedPath;
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool selectAktFolder()
        {
            winForms.FolderBrowserDialog open = new winForms.FolderBrowserDialog();
            if (open.ShowDialog() == winForms.DialogResult.OK)
            {
                this.AktDirektory = open.SelectedPath;
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
