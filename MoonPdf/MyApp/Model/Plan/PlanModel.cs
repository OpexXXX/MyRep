using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.Model.Plan
{
    public class PlanWorkModel
    {
        public static int GetValueBuNormativ(DateTime startDate, DateTime endDate, int normativ)
        {
            TimeSpan difDay;
            int difMounth;
            float result = 0;
            float norm = normativ;
            difMounth=((endDate.Year - startDate.Year) * 12) - startDate.Month + endDate.Month;
            if (startDate >= endDate) return 0;
            if (difMounth == 0 )
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
        private List<Abonent> _abonentList;
        public List<Abonent> AbonentList
        {
            get { return _abonentList; }
            set { _abonentList = value; }
        }


        public PlanWorkModel(DateTime dateWork)
        {
           List<string> Abonents = new List<string>( DataBaseWorker.FindAbonentPlan(dateWork));
            AbonentList = new List<Abonent>();
            foreach (var item in Abonents)
            {
                AbonentList.Add(new Abonent(item, dateWork));
            }

        } 
    }
}
