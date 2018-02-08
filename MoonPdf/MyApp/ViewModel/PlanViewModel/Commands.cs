using ATPWork.MyApp.Model.Plan;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ATPWork.MyApp.ViewModel.PlanViewModel
{
    public class Commands
    {
        public DelegateCommand CreatePdf { get; private set; }
        public DelegateCommand CreatePdfFromGroup { get; private set; }
        private PlanVM planVM;
        public Commands(PlanVM planVM)
        {
            this.planVM = planVM;
            Predicate<object> isCreatePDF = f => CanCreateMail();
            this.CreatePdf = new DelegateCommand("Открыть в PDF", f =>
           {
               string path = PlanWorkModel.CreatePDF();
               Process.Start(path);
           }, isCreatePDF, null);
            this.CreatePdfFromGroup = new DelegateCommand("Открыть в PDF", createPdf, isCreatePDF, null);

        }
        private bool CanCreateMail()
        {
            return planVM.AllAbonent.Count > 0;
        }


        private void createPdf(object obj)
        {
            var gg = (ReadOnlyObservableCollection<PlanAbonent>) obj;
            List<PlanAbonent> ggg = new List<PlanAbonent>();
           /* foreach (PlanAbonent item in gg)
            {
                ggg.Add(item);
            }
            if (ggg.Count > 0)
            {
               string path = PlanWorkModel.CreatePDF(ggg);
                Process.Start(path);

            }*/
        }
    }
}
