﻿using MyApp.Model;
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
        public static int GetAvveragePO(string ear,string numberLS)
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
            difMounth =((endDate.Year - startDate.Year) * 12) - startDate.Month + endDate.Month; // количество месяцев между датами
            if (difMounth == 0 ) // Если даты в одном месяце
            {
                float day = norm / (DateTime.DaysInMonth(startDate.Year, startDate.Month));
                result += day * startDate.Day;
                int resultt = (int)Math.Round(result * 10);
                return resultt;
            }
            else
            {
                for (int i = 0; i <=difMounth; i++)
                {
                    DateTime date1, date2;
                    if (i ==0)
                    {
                        date1 = startDate;
                        date2 = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
                        difDay = date2 - date1;
                        float day1 = norm / (DateTime.DaysInMonth(startDate.Year, startDate.Month));
                        result += day1 * (difDay.Days+1);
                        continue;
                    }
                    if (i == difMounth)
                    {
                        date1 = new DateTime(endDate.Year, endDate.Month,1);
                        date2 = endDate;
                        difDay = date2 - date1;
                        float day1 = norm/(DateTime.DaysInMonth(endDate.Year, endDate.Month));
                        result += day1 * (difDay.Days+1);
                        continue;
                    }
                   ;
                    date1 =startDate.AddMonths(i);
                    float day = norm / (DateTime.DaysInMonth(date1.Year, date1.Month));
                    result += day * DateTime.DaysInMonth(date1.Year, date1.Month);
                }
            }
            int result1 = (int)Math.Round(result*10);
            return result1;
        }

        internal static void CreatePDF(DateTime selectedDate)
        {
            var gg = ExcelWorker.MakeDataTableForPlan(AbonentList);
            Process.Start(ExcelWorker.CreatePdfReestrForPlan(gg));
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
