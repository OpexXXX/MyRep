using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MoonPdf
{
    public class PlobmListOserv : ObservableCollection<plomba>
    {
        public PlobmListOserv() { }
    }
    public class plomba : INotifyPropertyChanged, IEquatable<plomba>
    {
        private string type;
        public string Type
        {
            get { return this.type; }
            set { this.type = value; this.OnPropertyChanged("Type"); }
        }
        private string number;
        public string Number
        {
            get { return this.number; }
            set { this.number = value; this.OnPropertyChanged("Number"); }
        }
        private string place;
        public string Place
        {
            get { return this.place; }
            set { this.place = value; this.OnPropertyChanged("Place"); }
        }
        private bool remove;
        public bool Remove
        {
            get { return this.remove; }
            set { this.remove = value; this.OnPropertyChanged("Remove"); }
        }
        public plomba(string type, string number, string place, bool remove)
        {
            this.Type = type;
            this.Number = number;
            this.Place = place;
            this.Remove = remove;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string info) // На изменение полей
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        public bool Equals(plomba item) //Сравнение
        {
            return item.number == this.number;
        }
    }
    public class aktATP : INotifyPropertyChanged, IEquatable<aktATP>, IComparable<aktATP>
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

        public Dictionary<Enum, String> MainData;
        public List<int> NumberOfPagesInSoursePdf = new List<int>();
        private string title;
        public string Title
        {
            get
            {
                string pagenum = "";
                string result = "";
                result = "№" + (Number != null ? Number.ToString() : "") + ((DateWork.Year != 0001) ? (" от " + DateWork.ToString("d")) : ("")) + " , id:" + ID.ToString() + ", ";
                foreach (int row in NumberOfPagesInSoursePdf)
                {
                    pagenum = pagenum + row + "ст. ";
                }
                return result + pagenum;
            }
            set
            {
                this.title = value; this.OnPropertyChanged("Title");
            }
        }
        public bool ProverkaFlag
        {
            get { return !this.dopuskFlag; }

            set
            {
                this.DopuskFlag = !value;
                this.OnPropertyChanged("DopuskFlag");
                this.OnPropertyChanged("ProverkaFlag");
            }
        }
        private bool dopuskFlag;
        public bool DopuskFlag
        {
            get { return this.dopuskFlag; }
            set
            {
                if (value)
                {
                    TypeOfWork = "Допуск";
                }
                else
                {
                    TypeOfWork = "Проверка";
                }

                this.dopuskFlag = value;
                this.OnPropertyChanged("DopuskFlag");
                this.OnPropertyChanged("ProverkaFlag");

            }
        }
        private string ustanovka;
        public string Ustanovka
        {
            get { return this.ustanovka; }
            set
            {
                this.ustanovka = value; this.OnPropertyChanged("Ustanovka");
            }
        }
        private string edOborudovania;
        public string EdOborudovania
        {
            get { return this.edOborudovania; }
            set
            {
                this.edOborudovania = value; this.OnPropertyChanged("EdOborudovania");
            }
        }
        private string sapNumberAkt;
        public string SapNumberAkt
        {
            get { return this.sapNumberAkt; }
            set
            {
                this.sapNumberAkt = value; this.OnPropertyChanged("SapNumberAkt");
            }
        }
        private string typeOfWork;
        public string TypeOfWork
        {
            get { return this.typeOfWork; }
            set
            {
                this.typeOfWork = value; this.OnPropertyChanged("TypeOfWork");
            }
        }
        private string puOldNumber;
        public string PuOldNumber
        {
            get { return this.puOldNumber; }
            set
            {
                this.puOldNumber = value; this.OnPropertyChanged("PuOldNumber");
            }
        }
        private string puOldType;
        public string PuOldType
        {
            get { return this.puOldType; }
            set
            {
                this.puOldType = value; this.OnPropertyChanged("PuOldType");
            }
        }
        private string puNewNumber;
        public string PuNewNumber
        {
            get { return this.puNewNumber; }
            set
            {
                this.puNewNumber = value; this.OnPropertyChanged("PuNewNumber");
            }
        }
        private PriborUcheta puNewType;
        public PriborUcheta PuNewType
        {
            get { return this.puNewType; }
            set
            {
                this.puNewType = value; this.OnPropertyChanged("PuNewType");
            }
        }
        private string puOldPokaz;
        public string PuOldPokazanie
        {
            get { return this.puOldPokaz; }
            set
            {
                this.puOldPokaz = value; this.OnPropertyChanged("PuOldPokazanie");
            }
        }
        private bool puOldMPI;
        public bool PuOldMPI
        {
            get { return this.puOldMPI; }
            set
            {
                this.puOldMPI = value; this.OnPropertyChanged("PuOldMPI");
            }
        }
        private bool complete;
        public bool Complete
        {
            get { return this.complete; }
            private set
            {
                this.complete = value; this.OnPropertyChanged("Complete");
            }
        }
        private string puNewPokazanie;
        public string PuNewPokazanie
        {
            get { return this.puNewPokazanie; }
            set
            {
                this.puNewPokazanie = value; this.OnPropertyChanged("PuNewPokazanie");
            }
        }
        private string puNewPoverkaEar;
        public string PuNewPoverkaEar
        {
            get { return this.puNewPoverkaEar; }
            set
            {
                this.puNewPoverkaEar = value; this.OnPropertyChanged("PuNewPoverkaEar");
            }
        }
        private string puNewPoverkaKvartal;
        public string PuNewPoverKvartal
        {
            get { return this.puNewPoverkaKvartal; }
            set
            {
                this.puNewPoverkaKvartal = value; this.OnPropertyChanged("PuNewPoverKvartal");
            }
        }
        private string adress;
        public string Adress
        {
            get { return this.adress; }
            set
            {
                //TypeOfWorkBool = (value == "Допуск");
                this.adress = value; this.OnPropertyChanged("Adress");





            }
        }
        private string number;
        public string Number
        {
            get { return this.number; }
            set
            {

                this.number = value;
                this.Title = value.ToString();
                this.OnPropertyChanged("Type");
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
                this.OnPropertyChanged("DateMail");
            }
        }
        private DateTime dateWork;
        public DateTime DateWork
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
        public PlobmListOserv plomb;
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
            get { return this.agent_2; }
            set
            {
                this.agent_2 = value; this.OnPropertyChanged("Agent_2");
            }
        }
        private string numberLS;
        public string NumberLS
        {
            get { return this.numberLS; }
            set { this.numberLS = value; this.OnPropertyChanged("NumberLS"); }
        }
        private string pathOfPdfFile;
        public string PathOfPdfFile
        {
            get { return this.pathOfPdfFile; }
            set { this.pathOfPdfFile = value; this.OnPropertyChanged("PathOfPdfFile"); }
        }
        private string groupOfMail;
        public string GroupOfMail
        {
            get {
                string result;
                result = "исх. №" + this.NumberMail.ToString() + " от " + this.DateMail + " г.";
                return result;
            }
            private set {
                this.groupOfMail = value; this.OnPropertyChanged("GroupOfMail");
            }
        }
        public aktATP(int id, List<int> numbersOfPageInPdf, string pathOfPdfFile)
        {
            this.ID = id;
            this.Complete = false;
            plomb = new PlobmListOserv();
            // plomb.Add(new plomba("2404", "240501801456", "Клуммная крышка", true));
            this.pathOfPdfFile = pathOfPdfFile;
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
            if (dateWork.Year == 0001) result = false;
            if (fIO == null || fIO == "") result = false;
            if (number == null || number == "") result = false;
            if (numberLS == null || numberLS == "") result = false;
            if (puOldNumber == null || puOldNumber == "") result = false;
            if (puOldPokaz == null || puOldPokaz == "") result = false;
            if (puOldType == null || puOldType == "") result = false;
            if (typeOfWork == "Допуск")
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
        public bool Equals(aktATP item) //Сравнение
        {
            bool result = false;
            if (!this.complete)
            {
                result = (item.ID == this.ID) && (item.Number == this.Number) && (item.PuOldPokazanie == this.PuOldPokazanie) && (item.DateWork == this.DateWork);
            }
            else
            {
                result = (item.Number == this.Number) && (item.DateWork == this.DateWork) && (item.NumberLS == this.NumberLS);
            }
            return result;
        }
        protected void OnPropertyChanged(string info) // На изменение полей
        {
            if (info != "Complete") this.checkToComplete();
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        public int CompareTo(aktATP other)
        {
            if (other == null)
                return 1;
            else
            {
                int res = 0;
                Int32.TryParse(other.Number, out res);

                return this.Number.CompareTo(other.Number);

            }
            
        }
    }
}
