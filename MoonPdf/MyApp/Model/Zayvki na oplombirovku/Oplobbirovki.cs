using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Model
{
    public class spisokZayvokOplomb : ObservableCollection<ZayvkaOplomb>
    {
        public spisokZayvokOplomb() { }
    }
    public class ZayvkiOplombirovki
    {
        private spisokZayvokOplomb allZayvki;
        public spisokZayvokOplomb AllZayvki
        {
            get { return this.allZayvki; }
            set
            {
                this.allZayvki = value;
            }
        }
        private spisokZayvokOplomb completeZayvki;
        public spisokZayvokOplomb CompleteZayvki
        {
            get { return this.completeZayvki; }
            set
            {
                this.completeZayvki = value;
            }
        }
        public ZayvkiOplombirovki()
        {

        }
    }
    public class ZayvkaOplomb : INotifyPropertyChanged, IEquatable<aktATP>, IComparable<aktATP>
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

        private string title;
        public string Title
        {
            get
            {
                string result = "";
                result = "№" + (Number != null ? Number.ToString() : "") + ((DateWork != null) ? (" от " + ((DateTime)DateWork).ToString("d")) : ("")) + " ," + this.FIO;
                return result;
            }
            set
            {
                this.title = value;
                this.OnPropertyChanged("Title");
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

        private string puNumber;
        public string PuNumber
        {
            get { return this.puNumber; }
            set
            {
                this.puNumber = value; this.OnPropertyChanged("PuNumber");
            }
        }

        private string puType;
        public string PuType
        {
            get { return this.puType; }
            set
            {
                this.puType = value; this.OnPropertyChanged("PuOldType");
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

        public string Adress
        {
            get {
                string result = "";
                result += String.Format("{0}, {1}, д.{2}{3}", this.City, this.Street, this.House, this.Korpus);
                if (this.Kvartira != 0) result += ", кв" + this.Kvartira;
                return this.city; }
           private set
            {    }
        }

        private string city;
        public string City
        {
            get { return this.city; }
            set
            {
                     this.city = value; this.OnPropertyChanged("City");
            }
        }

        private string street;
        public string Street
        {
            get { return this.street; }
            set
            {
                this.street = value; this.OnPropertyChanged("Street");
            }
        }

        private int house;
        public int House
        {
            get { return this.house; }
            set
            {
                this.house = value; this.OnPropertyChanged("House");
            }
        }

        private string korpus;
        public string Korpus
        {
            get { return this.korpus; }
            set
            {
                this.korpus = value; this.OnPropertyChanged("Korpus");
            }
        }

        private int kvartira;
        public int Kvartira
        {
            get { return this.kvartira; }
            set
            {
                this.kvartira = value; this.OnPropertyChanged("Kvartira");
            }
        }

        private string number;
        public string Number
        {
            get { return this.number; }
            set
            {
                this.number = value;
                this.OnPropertyChanged("Number");
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

        private string numberLS;
        public string NumberLS
        {
            get { return this.numberLS; }
            set { this.numberLS = value; this.OnPropertyChanged("NumberLS"); }
        }

        public ZayvkaOplomb(int id, List<int> numbersOfPageInPdf, string pathOfPdfFile)
        {
            this.ID = id;
            this.Complete = false;
        }

        public void setDataByDb(Dictionary<string, string> dict)
        {

            NumberLS = dict["LsNumber"];
            PuType = dict["PuType"];
            PuNumber = dict["PuNumber"];
            FIO = dict["FIO"];
            string tmpadress = "";
            tmpadress = dict["City"] + ", " + dict["Street"] + ", д. " + dict["House"];
            if (dict.ContainsKey("Korpus")) tmpadress += dict["Korpus"];
            if (dict.ContainsKey("Kv")) tmpadress += ", кв." + dict["Kv"];
            Adress = tmpadress;
            Ustanovka = dict["Ustanovka"]; ;
            
        }
        public bool checkToComplete()
        {
            bool result = true;
            if (city == null || city == "") result = false;
            if (dateWork == null) result = false;
            if (fIO == null || fIO == "") result = false;
            if (number == null || number == "") result = false;
            if (numberLS == null || numberLS == "") result = false;
            Complete = result;
            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public bool Equals(aktATP item) //Сравнение
        {
            bool result = false;
           
                result = (item.Number == this.Number) && (item.DateWork == this.DateWork) && (item.NumberLS == this.NumberLS);
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
