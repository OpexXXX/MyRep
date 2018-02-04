using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.ViewModel.VnePlanViewModel
{
     public class Commands
    {
        public DelegateCommand CreatePdf { get; private set; }
        private VnePlanVM vnePlanVM;
        public Commands(VnePlanVM planVM)
        {
            this.vnePlanVM = planVM;
            Predicate<object> isCreatePDF = f => CanCreateMail();

            this.CreatePdf = new DelegateCommand("Открыть в PDF", f =>
           {
               //planVM.CreatePdf();
           }, isCreatePDF, null);
        }

        private bool CanCreateMail()
        {
            return true;
        }
    }
}
