using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyApp.Model
{
    public class AktTehProverki
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
            }
        }
        public List<int> NumberOfPagesInSoursePdf = new List<int>();
        public bool ProverkaFlag
        {
            get { return !this.dopuskFlag; }

            set
            {
                this.DopuskFlag = !value;
            }
        }
        private bool dopuskFlag;
        public bool DopuskFlag
        {
            get { return this.dopuskFlag; }
            set
            {
                this.dopuskFlag = value;
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
            }
        }
        private string puOldType;
        public string PuOldType
        {
            get { return this.puOldType; }
            set
            {
                this.puOldType = value; 
            }
        }
        private string puNewNumber;
        public string PuNewNumber
        {
            get { return this.puNewNumber; }
            set
            {
                this.puNewNumber = value;
            }
        }
        private PriborUcheta puNewType;
        public PriborUcheta PuNewType
        {
            get { return this.puNewType; }
            set
            {
                this.puNewType = value;
            }
        }
        private string puOldPokaz;
        public string PuOldPokazanie
        {
            get { return this.puOldPokaz; }
            set
            {
                this.puOldPokaz = value; 
            }
        }
        private bool puOldMPI;
        public bool PuOldMPI
        {
            get { return this.puOldMPI; }
            set
            {
                this.puOldMPI = value;
            }
        }
        private bool complete;
        public bool Complete
        {
            get { return this.complete; }
            private set
            {
                this.complete = value;
            }
        }
        private string puNewPokazanie;
        public string PuNewPokazanie
        {
            get { return this.puNewPokazanie; }
            set
            {
                this.puNewPokazanie = value; 
            }
        }
        private string puNewPoverkaEar;
        public string PuNewPoverkaEar
        {
            get { return this.puNewPoverkaEar; }
            set
            {
                this.puNewPoverkaEar = value; 
            }
        }
        private string puNewPoverkaKvartal;
        public string PuNewPoverKvartal
        {
            get { return this.puNewPoverkaKvartal; }
            set
            {
                this.puNewPoverkaKvartal = value;
            }
        }
        private string adress;
        public string Adress
        {
            get { return this.adress; }
            set
            {
                this.adress = value;
            }
        }
        private string number;
        public string Number
        {
            get { return this.number; }
            set
            {
                this.number = value;
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
            set { this.dateWork = value;  }
        }
        private string fIO;
        public string FIO
        {
            get { return this.fIO; }
            set { this.fIO = value; }
        }
        private int iD;
        public int ID
        {
            get { return this.iD; }
            set { this.iD = value;  }
        }
        private List<Plomba> _plombs = new List<Plomba>();
        public IEnumerable<Plomba> plombs {
            get { return new List<Plomba>(_plombs); }
        }
        private Agent agent_1;
        public Agent Agent_1
        {
            get { return this.agent_1; }
            set
            {
                this.agent_1 = value; 
            }
        }
        private Agent agent_2;
        public Agent Agent_2
        {
            get { return this.agent_2; }
            set
            {
                this.agent_2 = value; 
            }
        }
        private string numberLS;
        public string NumberLS
        {
            get { return this.numberLS; }
            set { this.numberLS = value;  }
        }
        private string namePdfFile;
        public string NamePdfFile
        {
            get { return this.namePdfFile; }
            set { this.namePdfFile = value;  }
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
            if (number == null || number == "") result = false;
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
    }
}
