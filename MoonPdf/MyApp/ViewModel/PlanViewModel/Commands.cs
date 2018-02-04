using MyApp.Model;
using System;
using System.Collections.Generic;
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
        private PlanVM planVM;
        public Commands(PlanVM planVM)
        {
            this.planVM = planVM;
            Predicate<object> isCreatePDF = f => CanCreateMail();
            this.CreatePdf = new DelegateCommand("Создать PDF", f =>
           {
               planVM.CreatePdf();
           }, isCreatePDF, null);
        }
        private bool CanCreateMail()
        {
            return planVM.AllAbonent.Count > 0;
        }
    }
}
