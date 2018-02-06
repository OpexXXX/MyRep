using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.Model.VnePlan
{
   static class VnePlanModel
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

        /*internal static void CreatePDF(List<VnePlanZayavka> zayvki)
        {
            var gg = ExcelWorker.MakeDataTableForPlan(AbonentList);
            Process.Start(ExcelWorker.CreatePdfReestrForPlan(gg));
        }*/

        private static List<VnePlanZayavka> _zayvki = new List<VnePlanZayavka>();
        public static List<VnePlanZayavka> Zayavki
        {
            get { return _zayvki; }
            set { _zayvki = value; }
        }


        public static void refreshZayavki()
        {
            Zayavki = DataBaseWorker.LoadZayavki();
            AbonentsRefresh?.Invoke();
        }

        internal static void SaveBeforeCloseApp()
        {
            DataBaseWorker.DropFromVnePlanZayvki();
            DataBaseWorker.InsertZayavki(Zayavki);
        }

        internal static int GetLastNumber()
        {
            int result = 0;
            foreach (var item in Zayavki)
            {
                if (item.RegNumber > result) result = item.RegNumber;
            }
            return result + 1;
        }
    }
}
