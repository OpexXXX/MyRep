using ATPWork.MyApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace MyApp.Model
{
    public class AktTehProverki:Abonent ,IComparer<AktTehProverki>
    {
        private int numberMail;
        public int NumberMail
        {
            get
            {
                return this.numberMail;
            }
            set
            {
                this.numberMail = value;
                this.OnPropertyChanged("NumberMail");
                this.OnPropertyChanged("MailName");
            }
        }
        private long _sizePDF;
        public long SizePDF
        {
            get
            {
                return this._sizePDF;
            }
            set
            {
                this._sizePDF = value;
                this.OnPropertyChanged("SizePDF");
            }
        }
        public string MailName
        {
            get {
                string result;
                if ((numberMail > 0) && DateMail != null) result = "исх.№91-" + numberMail + " от " + DateMail?.ToString("d");
                else result = "Неотправлено";
                return result;
            }
            
        }
        public string AktName
        {
            get
            {
                string result;
                if (number>0 && DateWork!=null) result = "№91-" + number + " от " + DateWork?.ToString("d");
                else result = NumberOfPagesInSoursePdf[0] + "стр.; "+ NumberOfPagesInSoursePdf[1] + "стр.;  незаполнен";
               
                return result;
            }
        }

        public List<int> _numberOfPagesInSoursePdf = new List<int>();
        public List<int> NumberOfPagesInSoursePdf
        {
            get { return this._numberOfPagesInSoursePdf; }
            set
            {
                this._numberOfPagesInSoursePdf = value;
                this.OnPropertyChanged("NumberOfPagesInSoursePdf");
                
            }
        }
        public bool ProverkaFlag
        {
            get { return !this.dopuskFlag; }

            set
            {
                this.DopuskFlag = !value;
                this.OnPropertyChanged("ProverkaFlag");
                this.OnPropertyChanged("DopuskFlag");
            }
        }
        private bool dopuskFlag;
        public bool DopuskFlag
        {
            get { return this.dopuskFlag; }
            set
            {
                this.dopuskFlag = value;
                this.OnPropertyChanged("ProverkaFlag");
                this.OnPropertyChanged("DopuskFlag");
            }
        }
       
        private string sapNumberAkt;
        public string SapNumberAkt
        {
            get { return this.sapNumberAkt; }
            set
            {
                this.sapNumberAkt = value;
                this.OnPropertyChanged("SapNumberAkt");
            }
        }
        
        private string puNewNumber;
        public string PuNewNumber
        {
            get { return this.puNewNumber; }
            set
            {
                this.puNewNumber = value;
                this.OnPropertyChanged("PuNewNumber");
            }
        }
        private PriborUcheta puNewType;
        public PriborUcheta PuNewType
        {
            get { return this.puNewType; }
            set
            {
                this.puNewType = value;
                this.OnPropertyChanged("PuNewType");
            }
        }
       
        private bool puOldMPI;
        public bool PuOldMPI
        {
            get { return this.puOldMPI; }
            set
            {
                this.puOldMPI = value;
                this.OnPropertyChanged("PuOldMPI");
            }
        }
        private bool complete;
        public bool Complete
        {
            get { return this.complete; }
            private set
            {
                this.complete = value;
                this.OnPropertyChanged("Complete");
            }
        }
        private string puNewPokazanie;
        public string PuNewPokazanie
        {
            get { return this.puNewPokazanie; }
            set
            {
                this.puNewPokazanie = value;
                this.OnPropertyChanged("PuNewPokazanie");
            }
        }
        private string puNewPoverkaEar;
        public string PuNewPoverkaEar
        {
            get { return this.puNewPoverkaEar; }
            set
            {
                this.puNewPoverkaEar = value;
                this.OnPropertyChanged("PuNewPoverkaEar");
            }
        }
        private string puNewPoverkaKvartal;
        public string PuNewPoverKvartal
        {
            get { return this.puNewPoverkaKvartal; }
            set
            {
                this.puNewPoverkaKvartal = value;
                this.OnPropertyChanged("PuNewPoverKvartal");
            }
        }
        private int number;
        public int Number
        {
            get { return this.number; }
            set
            {
                this.number = value;
                this.OnPropertyChanged("Number");
                this.OnPropertyChanged("AktName");
            }
        }
        private DateTime? dateMail;
        public DateTime? DateMail
        {
            get { return this.dateMail; }
            set
            {
                this.dateMail = value;
                this.OnPropertyChanged("DateMail");
                this.OnPropertyChanged("MailName"); 
            }
        }
        private DateTime? dateWork;
        public DateTime? DateWork
        {
            get { return this.dateWork; }
            set { this.dateWork = value;
                this.OnPropertyChanged("DateWork");
                this.OnPropertyChanged("AktName");
            }
        }
        private int iD;
        public int ID
        {
            get { return this.iD; }
            set { this.iD = value; this.OnPropertyChanged("ID"); }
        }
        private ObservableCollection<Plomba> _newPlombs = new ObservableCollection<Plomba>();
        public ObservableCollection<Plomba> NewPlombs {
            get { return _newPlombs; }
            set { this._newPlombs =  value;
                this.OnPropertyChanged("NewPlombs");
            }
        }
        
        private Agent agent_1;
        public Agent Agent_1
        {
            get { return this.agent_1; }
            set
            {
                this.agent_1 = value; this.OnPropertyChanged("Agent_1");
            }
        }
        private Agent agent_2;
        public Agent Agent_2
        {
            get { return this.agent_2;  }
            set
            {
                this.agent_2 = value; this.OnPropertyChanged("Agent_2");
            }
        }
      
        private string namePdfFile;
        public string NamePdfFile
        {
            get { return this.namePdfFile; }
            set { this.namePdfFile = value; this.OnPropertyChanged("NamePdfFile"); }
        }
        public AktTehProverki(int id, List<int> numbersOfPageInPdf, string pathOfPdfFile, long size)
        {
            this.ID = id;
            this.Complete = false;
            this.namePdfFile = pathOfPdfFile;
            this.SizePDF = size;
            foreach (var page in numbersOfPageInPdf)
            {
                NumberOfPagesInSoursePdf.Add(page);
            }
        }
        
        public bool checkToComplete()
        {
            bool result = true;
            if (agent_1 == null) result = false;
            if (dateWork == null) result = false;
            if (FIO == null || FIO == "") result = false;
            if (number ==0) result = false;
            if (NumberLS == null || NumberLS == "") result = false;
            if (PuOldNumber == null || PuOldNumber == "") result = false;
            if (PuOldPokazanie == null || PuOldPokazanie == "") result = false;
            if (PuOldType == null || PuOldType == "") result = false;
            if (dopuskFlag)
            {
                if (puNewNumber == null || puNewNumber == "") result = false;
                if (puNewPokazanie == null || puNewPokazanie == "") result = false;
                if (puNewPoverkaEar == null || puNewPoverkaEar == "") result = false;
                if (puNewPoverkaKvartal == null || puNewPoverkaKvartal == "") result = false;
                if (puNewType == null) return false;
            }

            foreach (Plomba item in NewPlombs)
            {
                if (item.Number == "" || item.Type == "") result = false;
            }

            Complete = result;
            return result;
        }

        public int Compare(AktTehProverki x, AktTehProverki y)
        {
            if (x.Number > y.Number)
            {
                return 1;
            }
            else if (x.Number < y.Number)
            {
                return -1;
            }

            return 0;
        }
    }
}
