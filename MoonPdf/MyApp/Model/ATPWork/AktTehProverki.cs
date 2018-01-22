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
    public class AktTehProverki:INotifyPropertyChanged 
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
            }
        }
        public List<int> NumberOfPagesInSoursePdf = new List<int>();
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
        private string ustanovka;
        public string Ustanovka
        {
            get { return this.ustanovka; }
            set
            {
                this.ustanovka = value;
            }
        }
        private string edOborudovania;
        public string EdOborudovania
        {
            get { return this.edOborudovania; }
            set
            {
                this.edOborudovania = value;
            }
        }
        private string sapNumberAkt;
        public string SapNumberAkt
        {
            get { return this.sapNumberAkt; }
            set
            {
                this.sapNumberAkt = value;
            }
        }
        private string puOldNumber;
        public string PuOldNumber
        {
            get { return this.puOldNumber; }
            set
            {
                this.puOldNumber = value;
                this.OnPropertyChanged("PuOldNumber");
            }
        }
        private string puOldType;
        public string PuOldType
        {
            get { return this.puOldType; }
            set
            {
                this.puOldType = value;
                this.OnPropertyChanged("PuOldType");
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
        private string puOldPokaz;
        public string PuOldPokazanie
        {
            get { return this.puOldPokaz; }
            set
            {
                this.puOldPokaz = value;
                this.OnPropertyChanged("PuOldPokazanie");
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
        private string adress;
        public string Adress
        {
            get { return this.adress; }
            set
            {
                this.adress = value;
                this.OnPropertyChanged("Adress");
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
            }
        }
        private string dateMail;
        public string DateMail
        {
            get { return this.dateMail; }
            set
            {
                this.dateMail = value;
            }
        }
        private DateTime? dateWork;
        public DateTime? DateWork
        {
            get { return this.dateWork; }
            set { this.dateWork = value; this.OnPropertyChanged("DateWork"); }
        }
        private string fIO;
        public string FIO
        {
            get { return this.fIO; }
            set { this.fIO = value; this.OnPropertyChanged("FIO"); }
        }
        private int iD;
        public int ID
        {
            get { return this.iD; }
            set { this.iD = value; this.OnPropertyChanged("ID"); }
        }
        private ObservableCollection<Plomba> _plombs = new ObservableCollection<Plomba>();
        public ObservableCollection<Plomba> Plombs {
            get { return _plombs; }
            set { this._plombs =  value; }
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
        private  string numberLS;
        public  string NumberLS
        {
            get { return this.numberLS; }
            set { this.numberLS = value; this.OnPropertyChanged("NumberLS"); }
        }
        private string namePdfFile;
        public string NamePdfFile
        {
            get { return this.namePdfFile; }
            set { this.namePdfFile = value; this.OnPropertyChanged("NamePdfFile"); }
        }
        private string groupOfMail;
        public AktTehProverki(int id, List<int> numbersOfPageInPdf, string pathOfPdfFile)
        {
            this.ID = id;
            this.Complete = false;
            this.namePdfFile = pathOfPdfFile;
            foreach (var page in numbersOfPageInPdf)
            {
                NumberOfPagesInSoursePdf.Add(page);
            }
        }
       public void setDataByDb(Dictionary<string, string> dict)
        {
            NumberLS = dict["LsNumber"];
            PuOldType = dict["PuType"];
            PuOldNumber = dict["PuNumber"];
            FIO = dict["FIO"];
            string tmpadress = "";
            tmpadress = dict["City"] + ", " + dict["Street"] + ", д. " + dict["House"];
            if (dict.ContainsKey("Korpus")) tmpadress += dict["Korpus"];
            if (dict.ContainsKey("Kv")) tmpadress += ", кв." + dict["Kv"];
            Adress = tmpadress;
            Ustanovka = dict["Ustanovka"]; ;
            EdOborudovania = dict["EdOborudovania"]; ;
        }

        public bool checkToComplete()
        {
            bool result = true;
            if (agent_1 == null) result = false;
            if (adress == null || adress == "") result = false;
            if (dateWork == null) result = false;
            if (fIO == null || fIO == "") result = false;
            if (number ==0) result = false;
            if (numberLS == null || numberLS == "") result = false;
            if (puOldNumber == null || puOldNumber == "") result = false;
            if (puOldPokaz == null || puOldPokaz == "") result = false;
            if (puOldType == null || puOldType == "") result = false;
            if (dopuskFlag)
            {
                if (puNewNumber == null || puNewNumber == "") result = false;
                if (puNewPokazanie == null || puNewPokazanie == "") result = false;
                if (puNewPoverkaEar == null || puNewPoverkaEar == "") result = false;
                if (puNewPoverkaKvartal == null || puNewPoverkaKvartal == "") result = false;
                if (puNewType == null) return false;
            }
            Complete = result;
            return result;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

       
    }
}
