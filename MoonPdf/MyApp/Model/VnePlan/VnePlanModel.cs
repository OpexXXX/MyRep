using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ATPWork.MyApp.Model.VnePlan
{
    internal static class VnePlanModel
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
        public static List<VnePlanZayavka> UnCompleteZayavki
        {
            get
            {

                List<VnePlanZayavka> tempList = new List<VnePlanZayavka>();
                foreach (var item in Zayavki)
                {
                    if (item.NumberAktTehProverki == "")
                        tempList.Add(item);
                }


                return tempList;
            }

        }


        public static void AddZayvka(VnePlanZayavka z)
        {
            Zayavki.Add(z);
            AbonentsRefresh?.Invoke();
        }

        public static void chekCompleteZayavki()
        {

            int ii = 0;
            foreach (var item in Zayavki)
            {
                if (item.NumberLS != "" && item.NumberAktTehProverki == "")
                {
                    List<string[]> akts = new List<string[]>();

                    List<string[]> result = DataBaseWorker.GetPrevusAktFenix(item.NumberLS);
                    List<string[]> aktsInApp = MainAtpModel.GetAktsForVneplan(item.NumberLS);
                    if (result.Count > 0)
                    {
                        akts.AddRange(result);
                    }
                    if (aktsInApp.Count > 0)
                    {
                        akts.AddRange(aktsInApp);
                    }

                    if (akts.Count > 0)
                    {
                        DateTime maxDateAkt = item.DateReg;
                        string numberMaxAkt = "";
                        foreach (var akt in akts)
                        {
                            if (maxDateAkt < DateTime.Parse(akt[1]))
                            {
                                maxDateAkt = DateTime.Parse(akt[1]);
                                numberMaxAkt = akt[0];
                                ii++;
                            }
                        }
                        if (maxDateAkt > item.DateReg)
                        {
                            item.NumberAktTehProverki = "№91-Е-" + numberMaxAkt + " от " + maxDateAkt.ToString("d");
                        }
                    }
                }
            }
            MessageBox.Show("Изменено" + ii);
            AbonentsRefresh?.Invoke();


        }

        internal static string CreatePDF(List<string> city)
        {
            List<VnePlanZayavka> list = new List<VnePlanZayavka>();
            foreach (var item in Zayavki)
            {
                if (city.Contains(item.City) && item.NumberAktTehProverki == "") list.Add(item);
            }
            if (list.Count > 0) return CreatePDF(list);
            else return "";
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

        public static string CreatePDF()
        {
            foreach (var item in UnCompleteZayavki)
            {
                if (item.NumberLS.Length == 12)
                {
                    List<Dictionary<String, String>> resultSearchAbonent;
                    resultSearchAbonent = DataBaseWorker.GetAbonentFromLS(item.NumberLS);

                    if (resultSearchAbonent.Count > 0)
                    {
                        item.setDataByDb(resultSearchAbonent[0]);
                    }
                }
            }
            var d = ExcelWorker.MakeDataTableForVnePlan(VnePlanModel.UnCompleteZayavki);

            return ExcelWorker.CreatePdfReestrForVnePlan(d);
        }

        public static string CreatePDF(List<VnePlanZayavka> zayavki)
        {

            foreach (var item in zayavki)
            {
                if (item.NumberLS.Length == 12)
                {
                    List<Dictionary<String, String>> resultSearchAbonent;
                    resultSearchAbonent = DataBaseWorker.GetAbonentFromLS(item.NumberLS);

                    if (resultSearchAbonent.Count > 0)
                    {
                        item.setDataByDb(resultSearchAbonent[0]);
                    }
                }
            }
            var d = ExcelWorker.MakeDataTableForVnePlan(zayavki);
            return ExcelWorker.CreatePdfReestrForVnePlan(d);
        }


        internal static int GetLastNumber()
        {
            int result = 0;
            foreach (var item in Zayavki)
            {
                if (item.RegNumber > result && item.DateReg.Year == DateTime.Now.Year) result = item.RegNumber;
            }
            return result + 1;
        }
    }
}
