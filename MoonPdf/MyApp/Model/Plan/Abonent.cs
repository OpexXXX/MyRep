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
    public class PlanAbonent:Abonent
    {
        public PlanAbonent(string numberLS, DateTime dateWork)
        {
            NumberLS = numberLS;
            DateWork = dateWork;
            List<Dictionary<string, string>> searchResult = DataBaseWorker.GetAbonentFromLS(NumberLS);
            if (searchResult.Count == 1)
            {
                Dictionary<string, string> dict = searchResult[0];
                setDataByDb(dict);
                getNormativ();//
                getPrevousAkt();//
                getPrevousPlan();//
                getAvveragePO();
            }
        }

        private void getAvveragePO()
        {
        AvveragePO=    PlanWorkModel.GetAvveragePO("2017", NumberLS);
            AvverageP = PlanWorkModel.GetAvveragePO("2016", NumberLS);

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
        private int _avveragePO;
        public int AvveragePO
        {
            get { return _avveragePO; }
            set { _avveragePO = value; }
        }
        private int _avverageP;
        public int AvverageP
        {
            get { return _avverageP; }
            set { _avverageP = value; }
        }
        public string Raschet
        {
            get {
                string result = "";
                List<DateTime> startDate = new List<DateTime>();
                if (PrevProverki.Count > 0)
                {
                    result += "Проверки: ";
                    foreach (var item in PrevProverki)
                    {
                        startDate.Add(DateTime.Parse(item[1]));
                        result += "№" + item[0] + " от " + item[1] + "г.; ";
                    }
                }
                else
                {
                    result += "Нет данных о последней проверке. ";
                }
                if (PrevPlan.Count > 0)
                {
                    result += "Был в плане на ";
                    foreach (var item in PrevPlan)
                    {
                        startDate.Add(item);
                        result += item.ToString("d") + ";";
                    }
                }
                List<DateTime> startDateRes = new List<DateTime>();
                foreach (var item in startDate)
                {
                    if (item < DateWork) startDateRes.Add(item);
                }

                DateTime? start = null;
                if (startDateRes.Count > 0)
                {
                    start = startDateRes.Max();
                    if (start < DateWork.AddMonths(-3)) start = DateWork.AddMonths(-3); // если между датами более трех месяцев устанавливаем стартовую дату
                }
                else
                {
                    start = DateWork.AddMonths(-3);
                }
                TimeSpan difDay = DateWork - (DateTime)start;
                result += " Ср. за 2017г. " + AvveragePO + "кВт*ч/мес; 2016г. " + AvverageP+ "кВт*ч/мес; ";
                result +=(difDay.Days + 1) + " дней к расчету, ";
                result += "норматив:" + Normativ + "кВт*ч/мес. БУ: " + PlanWorkModel.GetValueBuNormativ((DateTime)start, DateWork, Normativ).ToString() + "кВт*ч";
                return result;
            }
           
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

        private string _vneplan;
        public string Vneplan
        {
            get { return this._vneplan; }
            set
            {
                this._vneplan = value;
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
