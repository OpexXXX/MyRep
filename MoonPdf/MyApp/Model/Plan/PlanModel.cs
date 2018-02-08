using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.Model.Plan
{
    public static class PlanWorkModel
    {
        public delegate void AbonentsRefreshHandler();
        public static event AbonentsRefreshHandler AbonentsRefresh;


        /// <summary>
        /// Расчет объема безучетного потребления
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="normativ"></param>
        /// <returns></returns>
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
                result += day * startDate.Day;
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

        public static int CalculationPremiumActBu(double buValue)
        {
            double tarif = 1.70303;
            double ecoEffect = 0,premialFond;
            double ecoValue = tarif * buValue / 1000;
            double RK = 30, SN = 30, SV = 30.4;
            if (ecoValue <= 5) ecoEffect = ecoValue * 0.5;
            if ((5 < ecoValue) && (ecoValue <= 10)) ecoEffect = 2.5 + (ecoValue - 5) * 0.4;
            if ((10 < ecoValue) && (ecoValue <= 20)) ecoEffect = 4.5 + (ecoValue - 10) * 0.35;
            if ((20 < ecoValue) && (ecoValue <= 30)) ecoEffect = 8 + (ecoValue - 20) * 0.30;
            if ((30 < ecoValue) && (ecoValue <= 40)) ecoEffect = 11 + (ecoValue - 30) * 0.25;
            if ((40 < ecoValue) && (ecoValue <= 50)) ecoEffect = 13.5 + (ecoValue - 40) * 0.20;
            if ((50 < ecoValue) && (ecoValue <= 100)) ecoEffect = 15.5 + (ecoValue - 50) * 0.15;
            if ((100 < ecoValue) && (ecoValue <= 200)) ecoEffect = 23 + (ecoValue - 100) * 0.08;
            if ((200 < ecoValue) && (ecoValue <= 300)) ecoEffect = 31 + (ecoValue - 200) * 0.07;
            if ((300 < ecoValue) && (ecoValue <= 400)) ecoEffect = 38 + (ecoValue - 300) * 0.06;
            if ((400 < ecoValue) && (ecoValue <= 500)) ecoEffect = 44 + (ecoValue - 400) * 0.05;
            if ((500 < ecoValue) && (ecoValue <= 1000)) ecoEffect = 49 + (ecoValue - 500) * 0.04;
            if ((1000 < ecoValue) && (ecoValue <= 2000)) ecoEffect = 69 + (ecoValue - 1000) * 0.03;
            if ((2000 < ecoValue)) ecoEffect = 99 + (ecoValue - 2000) * 0.02;
            premialFond = ecoEffect / ((1 + RK / 100 + SN / 100) * (1 + SV / 100));
            int result = (int)Math.Round(premialFond * 1000); 
            return result;
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
        internal static string CreatePDF()
        {
            var gg = ExcelWorker.MakeDataTableForPlan(AbonentList);
            string result = ExcelWorker.CreatePdfReestrForPlan(gg);
            return result;
        }
        internal static string CreatePDF(List<PlanAbonent> planList)
        {
            var gg = ExcelWorker.MakeDataTableForPlan(planList);
           string result =  ExcelWorker.CreatePdfReestrForPlan(gg);
            return result;
        }


        private static List<PlanAbonent> _abonentList = new List<PlanAbonent>();
        public static List<PlanAbonent> AbonentList
        {
            get { return _abonentList; }
            set { _abonentList = value; }
        }

        public static void refreshAbonentList(DateTime dateWork)
        {
            List<string> Abonents = new List<string>(DataBaseWorker.FindAbonentPlan(dateWork));
            AbonentList = new List<PlanAbonent>();
            foreach (var item in Abonents)
            {
                AbonentList.Add(new PlanAbonent(item, dateWork));
            }
            AbonentsRefresh?.Invoke();
        }


    }
}
