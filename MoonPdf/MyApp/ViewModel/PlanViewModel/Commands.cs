using ATPWork.MyApp.Model.Plan;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Data;

namespace ATPWork.MyApp.ViewModel.PlanViewModel
{
    public class Commands
    {
        public DelegateCommand CreatePdf { get; private set; }
        public DelegateCommand CreatePdfForVnePlan { get; private set; }

        public DelegateCommand CreatePdfFromGroup { get; private set; }
        private readonly PlanVM planVM;
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
            this.CreatePdfForVnePlan = new DelegateCommand("Открыть внеплан в PDF", createPdfForVneplan, isCreatePDF, null);
        }

        private void createPdfForVneplan(object obj)
        {
            string path = PlanWorkModel.CreatePDFforVneplan();
            if (path != "") Process.Start(path);
        }

        private bool CanCreateMail()
        {
            return planVM.AllAbonent.Count > 0;
        }


        private void createPdf(object obj)
        {
            var gg = (CollectionViewGroup)obj;
            List<PlanAbonent> ggg = new List<PlanAbonent>();
            foreach (PlanAbonent item in gg.Items)
            {
                ggg.Add(item);
            }
            if (ggg.Count > 0)
            {
                string path = PlanWorkModel.CreatePDF(ggg);
                Process.Start(path);

            }
        }
    }
}
