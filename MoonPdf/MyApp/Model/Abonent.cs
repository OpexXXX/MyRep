using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.Model
{
    public class Abonent : INotifyPropertyChanged
    {
        private string ustanovka;
        public string Ustanovka
        {
            get { return this.ustanovka; }
            set
            {
                this.ustanovka = value;
                this.OnPropertyChanged("Ustanovka");
            }
        }
       

       
        private string edOborudovania;
        public string EdOborudovania
        {
            get { return this.edOborudovania; }
            set
            {
                this.edOborudovania = value;
                this.OnPropertyChanged("EdOborudovania");
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
        private string _city;
        public string City
        {
            get { return this._city; }
            set
            {
                this._city = value;
                this.OnPropertyChanged("Adress");
            }
        }
        private string _street;
        public string Street
        {
            get { return this._street; }
            set
            {
                this._street = value;
                this.OnPropertyChanged("Adress");
            }
        }
        private int _house;
        public int House
        {
            get { return _house; }
            set { _house = value; this.OnPropertyChanged("Adress"); }
        }
        private string _korpus;
        public string Korpus
        {
            get { return this._korpus; }
            set
            {
                this._korpus = value;
                this.OnPropertyChanged("Adress");
            }
        }
        private int _kvartira;
        public int Kvartira
        {
            get { return _kvartira; }
            set { _kvartira = value;
                this.OnPropertyChanged("Adress");
            }
        }
        public string Adress
        {
            get
            {
                string result = "";
                if (City?.Length>0)
                {
                    result += City;
                    result += ", " + Street;
                    result += ", д." + House;
                    if (Korpus != "") result += Korpus;
                    if (Kvartira != 0) result += ", кв." + Kvartira;
                }
                return result;
            }

        }
        private string _podkl;
        public string Podkl
        {
            get { return this._podkl; }
            set
            {
                this._podkl = value;
                this.OnPropertyChanged("Podkl");
            }
        }
        private string fIO;
        public string FIO
        {
            get { return this.fIO; }
            set { this.fIO = value; this.OnPropertyChanged("FIO"); }
        }
        private string numberLS;
        public string NumberLS
        {
            get { return this.numberLS; }
            set { this.numberLS = value; this.OnPropertyChanged("NumberLS"); }
        }
        private ObservableCollection<Plomba> _oldPlombs = new ObservableCollection<Plomba>();
        public ObservableCollection<Plomba> OldPlombs
        {
            get { return _oldPlombs; }
            set
            {
                this._oldPlombs = value;
                this.OnPropertyChanged("OldPlombs");
            }
        }
        public virtual void setDataByDb(Dictionary<string, string> dict)
        {
            NumberLS = dict["LsNumber"];
            PuOldType = dict["PuType"];
            PuOldNumber = dict["PuNumber"];
            FIO = dict["FIO"];
            City = dict["City"];
            Street = dict["Street"];
            House = int.Parse(dict["House"]);
            if (dict.ContainsKey("Korpus") && dict["Korpus"].ToString() != "") Korpus = dict["Korpus"];
            if (dict.ContainsKey("Kv") && dict["Kv"].ToString() != "") Kvartira = int.Parse(dict["Kv"]);
            Ustanovka = dict["Ustanovka"];
            EdOborudovania = dict["EdOborudovania"];
            Podkl = dict["Podkluchenie"];
            OldPlombs.Clear();
            List<Dictionary<string, string>> plombs = DataBaseWorker.GetPlombsFromEdOb(EdOborudovania);
            foreach (Dictionary<string, string> item in plombs)
            {
                string plomb_Type, plomb_Number, plomb_Place, plomb_Status, plomb_DateInstall;
                plomb_Type = item["Type"].ToString();
                plomb_Number = item["Number"].ToString();
                plomb_Place = item["Place"].ToString();
                plomb_Status = item["Status"].ToString();
                plomb_DateInstall = item["InstallDate"].ToString();
                OldPlombs.Add(new Plomba(plomb_Type, plomb_Number, plomb_Place, false, true, plomb_Status, plomb_DateInstall));
            }
            this.OnPropertyChanged("BriefInformation");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
