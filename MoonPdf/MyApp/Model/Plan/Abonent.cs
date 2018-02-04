using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.Model.Plan
{
    public enum Kategorya
    {
        NoEquipment = 1,
        ElectricStove,
        Electroboiler
    }
    public class Abonent
    {
        public Abonent(string numberLS, DateTime dateWork)
        {
            NumberLS = numberLS;
            DateWork = dateWork;
            this.setDataByDb();
        }
        private int _rooms;
        public int Rooms
        {
            get { return _rooms; }
            set { _rooms = value; }
        }
        private int _peopleCount;
        public int PeopleCount
        {
            get { return _peopleCount; }
            set { _peopleCount = value; }
        }
        private int _normativKat;
        public int NormativKat
        {
            get { return _normativKat; }
            set { _normativKat = value; }
        }
        private int _normativ;
        public int Normativ
        {
            get { return _normativ; }
            set { _normativ = value; }
        }
        private List<DateTime> _prevPlan = new List<DateTime>();
        public List<DateTime> PrevPlan
        {
            get { return _prevPlan; }
            set { _prevPlan = value; }
        }
        private List<string[]> _prevProverki = new List<string[]>();
        public List<string[]> PrevProverki
        {
            get { return _prevProverki; }
            set { _prevProverki = value; }
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
        private string _vneplan;
        public string Vneplan
        {
            get { return this._vneplan; }
            set
            {
                this._vneplan = value;
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
        private string adress;
        public string Adress
        {
            get { return this.adress; }
            set
            {
                this.adress = value;
            }
        }
        private string _podkl;
        public string Podkl
        {
            get { return this._podkl; }
            set
            {
                this._podkl = value;
            }
        }
        private DateTime dateWork;
        public DateTime DateWork
        {
            get { return this.dateWork; }
            set
            {
                this.dateWork = value;
            }
        }
        private string fIO;
        public string FIO
        {
            get { return this.fIO; }
            set { this.fIO = value; }
        }
        private List<Plomba> _oldPlombs = new List<Plomba>();
        public List<Plomba> OldPlombs
        {
            get { return _oldPlombs; }
            set
            {
                this._oldPlombs = value;
            }
        }
        private string numberLS;
        public string NumberLS
        {
            get { return this.numberLS; }
            set { this.numberLS = value; }
        }
        public void setDataByDb()
        {
            List<Dictionary<string, string>> searchResult = DataBaseWorker.PlanGetAbonentFromDb(numberLS);
            if (searchResult.Count == 1)
            {
                Dictionary<string, string> dict = searchResult[0];
                PuOldType = dict["PuType"];
                PuOldNumber = dict["PuNumber"];
                FIO = dict["FIO"];
                string tmpadress = "";
                tmpadress = dict["City"] + ", " + dict["Street"] + ", д. " + dict["House"];
                if (dict.ContainsKey("Korpus")) tmpadress += dict["Korpus"];
                if (dict.ContainsKey("Kv")) tmpadress += ", кв." + dict["Kv"];
                Adress = tmpadress;
                Ustanovka = dict["Ustanovka"];
                EdOborudovania = dict["EdOborudovania"];
             
                addPlombs();//
                getNormativ();//
                getPrevousAkt();//
               getPrevousPlan();//

                List<DateTime> startDate = new List<DateTime>();
                if (PrevProverki.Count > 0)
                {
                    Podkl += "Проверки ";
                    foreach (var item in PrevProverki)
                    {
                        startDate.Add(DateTime.Parse(item[1]));
                        Podkl += "№" + item[0] + " от " + item[1] + "г.\n";
                    }
                }
                else
                {
                    Podkl += "Нет данных о последнейй проверке.\n";
                }

                if (PrevPlan.Count > 0)
                {
                    Podkl += "Был в плане на ";
                    foreach (var item in PrevPlan)
                    {
                        startDate.Add(item);
                        Podkl +=  item.ToString("d") + "\n";
                    }
                }

                List<DateTime> startDateRes = new List<DateTime>();
                foreach (var item in startDate)
                {
                    if (item < DateWork) startDateRes.Add(item);
                }

                DateTime? start = null ;
                if (startDateRes.Count > 0)
                {
                     start = startDateRes.Max();
                    if (start < DateWork.AddMonths(-3)) start = DateWork.AddMonths(-3); // если между датами более трех месяцев устанавливаем стартовую дату
                }
                else
                {
                    start = DateWork.AddMonths(-3);
                }

               
                TimeSpan difDay = DateWork-(DateTime)start;
                Podkl += (difDay.Days+1)+ " дней к расчету\n";
                Podkl += "Норматив:"+ Normativ+ "кВт*ч/мес, БУ: " + PlanWorkModel.GetValueBuNormativ((DateTime)start, DateWork, Normativ).ToString()+"кВт*ч";
            }
        }

        private void getPrevousPlan()
        {
          var dat =   DataBaseWorker.FindAbonentPlan(NumberLS);
            if (dat.Count > 0)
            {
                if (dat.Contains(DateWork.ToString("d"))) dat.Remove(DateWork.ToString("d"));

                foreach (string item in dat)
                {
                    PrevPlan.Add(DateTime.Parse(item));
                }
               
            }
        }

        private void getPrevousAkt()
        {
            List<string[]> result= DataBaseWorker.GetPrevusAktFenix(NumberLS);
            if(result.Count>0)
            {
                foreach (var item in result)
                {
                    PrevProverki.Add(item);
                }
            }
        }

        private void getNormativ()
        {
            Dictionary<string, string> info = DataBaseWorker.GetInfoForNormativ(NumberLS);
            Rooms = Int32.Parse(info["Rooms"].ToString());
            PeopleCount = Int32.Parse(info["People"].ToString());
            NormativKat = Int32.Parse(info["Kategorya"].ToString());
           Normativ = DataBaseWorker.GetNormativ(PeopleCount, Rooms, NormativKat);
        }
        private void addPlombs()
        {
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
        }
    }
}
