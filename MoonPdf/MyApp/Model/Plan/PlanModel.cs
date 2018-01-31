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
