using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.Model.AktBuWork
{
   public enum VidNarusheniya
    {
        Vmeshatelstvo,
        Power,
        NoPower
    }
    public class AktBu : Abonent
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
            get
            {
                string result;
                if ((numberMail > 0) && DateMail != null) result = "исх.№91-" + numberMail + " от " + DateMail?.ToString("d");
                else result = "Неотправлено";
                return result;
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
            set
            {
                this.dateWork = value;
                this.OnPropertyChanged("DateWork");
                this.OnPropertyChanged("AktName");
            }
        }
        private Agent agent_1;

        internal void FindCurrentAktProverki()
        {
          AktTehProverki akt =  MainAtpModel.GetAtpFromComplete(NumberLS, DateWork);
            if (akt != null)
            {
                AktPedProverki = "№91/Е-" + akt.Number + " от " + akt.DateWork?.ToString("d") + "г.";
                AktProverkiPdf = System.IO.Path.Combine(MainAtpModel.AktDirektory, akt.NamePdfFile);
            }
        }

        internal void FindPrevousAktProverki()
        {
            if(PrevProverki.Count>0)
            {
                AktPedidProverki =  "№91/" + PrevProverki[(PrevProverki.Count - 1)][0]+" от "+ PrevProverki[(PrevProverki.Count - 1)][1]+ "г.";
            }
        }

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
        private string _aktBuPdf;
        public string AktBuPdf
        {
            get { return _aktBuPdf; }
            set { _aktBuPdf = value; this.OnPropertyChanged("AktBuPdf"); }
        }

        private string _aktPedidProverkiPdf;
        public string AktPredidProverkiPdf
        {
            get { return _aktPedidProverkiPdf; }
            set {
               
                _aktPedidProverkiPdf = value;
                this.OnPropertyChanged("AktPredidProverkiPdf");
            }
        }
        private string _aktPedidProverki;
        public string AktPedidProverki
        {
            get { return _aktPedidProverki; }
            set {
                foreach (char ch in Path.GetInvalidFileNameChars())
                {
                    value =  value.Replace(ch, '-');
                }
                _aktPedidProverki = value; 
                this.OnPropertyChanged("AktPedidProverki"); }
        }

        private string _aktProverkiPdf;
        public string AktProverkiPdf
        {
            get { return _aktProverkiPdf; }
            set { _aktProverkiPdf = value; this.OnPropertyChanged("AktProverkiPdf"); }
        }
        private string _aktPedProverki;
        public string AktPedProverki
        {
            get { return _aktPedProverki; }
            set {
                foreach (char ch in Path.GetInvalidFileNameChars())
                {
                    value = value.Replace(ch, '-');
                }
                _aktPedProverki = value; 
                this.OnPropertyChanged("AktPedProverki"); 
            }
        }

        private string _izveshenie;
        public string IzvesheniePDF
        {
            get { return _izveshenie; }
            set { _izveshenie = value; this.OnPropertyChanged("IzvesheniePDF"); }
        }
        private string _narushenie;
        public string Narushenie
        {
            get { return _narushenie; }
            set { _narushenie = value; this.OnPropertyChanged("Narushenie"); }
        }
        private VidNarusheniya _typeNarushenie;
        public VidNarusheniya TypeNarushenie
        {
            get { return _typeNarushenie; }
            set { _typeNarushenie = value; this.OnPropertyChanged("TypeNarushenie"); }
        }
        private ObservableCollection<string> _photoFile = new ObservableCollection<string>();
        public ObservableCollection<string> PhotoFile
        {
            get { return _photoFile; }
            set { _photoFile = value; this.OnPropertyChanged("PhotoFile"); }
        }
        #region Расчеты объема
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
        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get { return this._startDate; }
            set
            {
                this._startDate = value;
                this.OnPropertyChanged("StartDate");

            }
        }
        private int _countDay;
        public int CountDay
        {
            get { return this._countDay; }
            set
            {

                this._countDay = value;

                this.OnPropertyChanged("CountDay");
            }
        }
        private int _buValueNormativ;
        public int BuValueNormativ
        {
            get { return this._buValueNormativ; }
            set
            {

                this._buValueNormativ = value;

                this.OnPropertyChanged("BuValueNormativ");
            }
        }
        private int _buValuePower;
        public int BuValuePower
        {
            get { return this._buValuePower; }
            set
            {

                this._buValuePower = value;

                this.OnPropertyChanged("BuValuePower");
            }
        }
        private float _power;
        public float Power
        {
            get { return this._power; }
            set
            {

                this._power = value;

                this.OnPropertyChanged("Power");
            }
        }
        private int _peopleCount;
        public int PeopleCount
        {
            get { return this._peopleCount; }
            set
            {
                this._peopleCount = value;
                this.OnPropertyChanged("PeopleCount");
            }
        }
        private int _roomCount;
        public int RoomCount
        {
            get { return this._roomCount; }
            set
            {
                this._roomCount = value;
                this.OnPropertyChanged("RoomCount");
            }
        }

       
        public string ConsoleRaschet
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
                        result += "№" + item[0] + " от " + item[1] + "г.; \n";
                    }
                }
                else
                {
                    result += "Нет данных о последней проверке.\n";
                }
                if (PrevPlan.Count > 0)
                {
                    result += "Был в плане на ";
                    foreach (var item in PrevPlan)
                    {
                        startDate.Add(item);
                        result += item.ToString("d") + ";\n";
                    }
                }

                result += " Ср. за 2017г. " + AvveragePO + "кВт*ч/мес; 2016г. " + AvverageP + "кВт*ч/мес; \n";
                result += CountDay + " дней к расчету, \n";
                return result; }
            
        }
        public static int GetAvveragePO(string ear, string numberLS)
        {
            int result = 0;
            var ls = DataBaseWorker.GetAbonentPO(ear, numberLS);
            if (ls.Count > 0)
            {
                int summ = 0;
                foreach (var item in ls)
                {
                    int i = 0;
                    bool flag = int.TryParse(item, out i);
                    if (flag)
                    {
                        summ += i;
                    }
                }

                result = summ / ls.Count;

            }
            return result;
        }
        private void getAvveragePO()
        {
            AvveragePO = GetAvveragePO("2017", NumberLS);
            AvverageP = GetAvveragePO("2016", NumberLS);
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
        private string _normativKatStr;
        public string NormativKatStr
        {
            get { return _normativKatStr; }
            set
            {

                //{ "Без электроплиты", "С электроплитой","Водонагреватель", "Электробойлер" };
                if (value == "Без электроплиты") _normativKat = 1;
                if (value == "С электроплитой") _normativKat = 2;
                if (value == "Водонагреватель") _normativKat = 3;
                if (value == "Электробойлер") _normativKat = 4;
                _normativKatStr = value;
                this.OnPropertyChanged("NormativKatStr");
            }
        }

        private int _normativKat;
        public int NormativKat
        {
            get { return _normativKat; }
            set
            {
                if (value == 1) NormativKatStr = "Без электроплиты";
                if (value == 2) NormativKatStr = "С электроплитой";
                if (value == 3) NormativKatStr = "Водонагреватель";
                if (value == 4) NormativKatStr = "Электробойлер";
                this.OnPropertyChanged("NormativKatStr");
                _normativKat = value;
            }
        }
        private int _normativ;
        public int Normativ
        {
            get { return this._normativ; }
            set
            {

                this._normativ = value;

                this.OnPropertyChanged("Normativ");
            }
        }
        private void getPrevousPlan()
        {
            var dat = DataBaseWorker.FindAbonentPlan(NumberLS);
            if (dat.Count > 0)
            {
                PrevPlan.Clear();
                if (dat.Contains(DateWork?.ToString("d"))) dat.Remove(DateWork?.ToString("d"));

                foreach (string item in dat)
                {
                    PrevPlan.Add(DateTime.Parse(item));
                }

            }
        }
        private void getPrevousAkt()
        {
            List<string[]> result = DataBaseWorker.GetPrevusAktFenix(NumberLS);

            if (result.Count > 0)
            {
                PrevProverki.Clear();
                foreach (var item in result)
                {
                    PrevProverki.Add(item);
                }
            }
        }
        /// <summary>
        /// Выгрузка информации по прописанным и количеству комнат
        /// </summary>
        private void getInfoForNormativFromNumberLS()
        {
            Dictionary<string, string> info = DataBaseWorker.GetInfoForNormativ(NumberLS);
            RoomCount = Int32.Parse(info["Rooms"].ToString());
            PeopleCount = Int32.Parse(info["People"].ToString());
            NormativKat = Int32.Parse(info["Kategorya"].ToString());
        }
        /// <summary>
        /// Выгрузка норматива из базы
        /// </summary>
        public void getNormativ()
        {
            Normativ = DataBaseWorker.GetNormativ(PeopleCount, RoomCount, NormativKat);
        }
        private void calcCountDay()
        {
            List<DateTime> startDate = new List<DateTime>();
            if (PrevProverki.Count > 0)
            {
                foreach (var item in PrevProverki)
                {
                    startDate.Add(DateTime.Parse(item[1]));
                }
            }
            if (PrevPlan.Count > 0)
            {
                foreach (var item in PrevPlan)
                {
                    startDate.Add(item);
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
                if (start < DateWork?.AddMonths(-3)) start = DateWork?.AddMonths(-3); // если между датами более трех месяцев устанавливаем стартовую дату
            }
            else
            {
                start = DateWork?.AddMonths(-3);
            }
            StartDate = start;
            TimeSpan difDay = (DateTime)DateWork - (DateTime)start;
            CountDay = (difDay.Days + 1);
        }
        public static int GetValueBuNormativ(DateTime startDate, DateTime endDate, int normativ)
        {
            TimeSpan difDay;
            int difMounth;
            float result = 0;
            float norm = normativ;
            if (startDate >= endDate) return 0; //Если начальная дата больше конечной
            if (startDate < endDate.AddMonths(-3)) startDate = endDate.AddMonths(-3); // если между датами более трех месяцев устанавливаем стартовую дату
            difMounth = ((endDate.Year - startDate.Year) * 12) - startDate.Month + endDate.Month; // количество месяцев между датами
            if (difMounth == 0) // Если даты в одном месяце
            {
                float day = norm / (DateTime.DaysInMonth(startDate.Year, startDate.Month));
                difDay = (endDate - startDate);
                result += day * (difDay.Days + 1);
                int resultt = (int)Math.Round(result * 10);
                return resultt;
            }
            else
            {
                for (int i = 0; i <= difMounth; i++)
                {
                    DateTime date1, date2;
                    if (i == 0)
                    {
                        date1 = startDate;
                        date2 = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
                        difDay = date2 - date1;
                        float day1 = norm / (DateTime.DaysInMonth(startDate.Year, startDate.Month));
                        result += day1 * (difDay.Days + 1);
                        continue;
                    }
                    if (i == difMounth)
                    {
                        date1 = new DateTime(endDate.Year, endDate.Month, 1);
                        date2 = endDate;
                        difDay = date2 - date1;
                        float day1 = norm / (DateTime.DaysInMonth(endDate.Year, endDate.Month));
                        result += day1 * (difDay.Days + 1);
                        continue;
                    }
                   ;
                    date1 = startDate.AddMonths(i);
                    float day = norm / (DateTime.DaysInMonth(date1.Year, date1.Month));
                    result += day * DateTime.DaysInMonth(date1.Year, date1.Month);
                }
            }
            int result1 = (int)Math.Round(result * 10);
            return result1;
        }
        public void calcBu()
        {
            TimeSpan difDay;
            difDay =(DateTime) DateWork - (DateTime)StartDate;
            CountDay = difDay.Days + 1;

          

            BuValuePower = (int)Math.Round(CountDay * 24 * Power);

            BuValueNormativ = GetValueBuNormativ((DateTime)StartDate, (DateTime)DateWork, Normativ);
            this.OnPropertyChanged("ConsoleRaschet");
        }
        public override void setDataByDb(Dictionary<string, string> dict)
        {
            base.setDataByDb(dict);
            getInfoForNormativFromNumberLS();//
            getNormativ();
            getPrevousAkt();//
            getPrevousPlan();//
            getAvveragePO();
            if (dateWork != null)
            {
                calcCountDay();
                calcBu();
            }
        }
        #endregion


    }
}
