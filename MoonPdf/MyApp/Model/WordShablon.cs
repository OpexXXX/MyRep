using ATPWork.MyApp.Model.VnePlan;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateEngine.Docx;
using MyApp.Model;
using Microsoft.Office.Interop.Word;

namespace ATPWork.MyApp.Model
{
    public static class WordShablon
    {
        public static string CreateVnePlanZayvka(VnePlanZayavka zayav)
        {
            if (!Directory.Exists("vnePlan")) Directory.CreateDirectory("vnePlan");
            string fileName = System.IO.Path.GetRandomFileName();
            string filePath = "vnePlan\\" + fileName + ".docx";
            File.Copy("Zayavka.docx", filePath);
            var valuesToFill = new Content(
    new FieldContent("RegNumber", zayav.RegNumber.ToString()),
    new FieldContent("DateReg", zayav.DateReg.ToString("d")),
    new FieldContent("Adress", zayav.Adress),
    new FieldContent("FIO", zayav.FIO != null ? zayav.FIO : ""),
     new FieldContent("PhoneNumber", zayav.PhoneNumbers != null ? zayav.PhoneNumbers : ""),
    new FieldContent("NumberLS", zayav.NumberLS != null ? zayav.NumberLS.ToString() : ""),
    new FieldContent("DopuskFlag", zayav.DopuskFlag ? "V" : ""),
    new FieldContent("ProverkaFlag", zayav.ProvFlag ? "V" : ""),
    new FieldContent("DemontageFlag", zayav.DemontageFlag ? "V" : ""),
    new FieldContent("Prichina", zayav.Prichina != null ? zayav.Prichina : ""),
    new FieldContent("PuType", zayav.DopuskFlag ? "" : zayav.PuOldType != null ? zayav.PuOldType : ""),
    new FieldContent("PuNumber", zayav.DopuskFlag ? "" : zayav.PuOldNumber != null ? zayav.PuOldNumber : "")
);
            using (var outputDocument = new TemplateProcessor(filePath)
                .SetRemoveContentControls(true))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();
            }
            return filePath;
        }

       
        public static void CreateMailForBuAkts()
        { }
        public static void CreateRaschetForBuAkt()
        { }
        public static void CreatePoliceMailForBuAkt()
        { }
        public static void CreateObysnenyaForBuAkt()
        { }
        public static void CreateContentForBuAkt()
        { }
        public static void CreatePZForBuAkt()
        { }

       

        internal static string CreateMailForAktsTehProverki(List<AktTehProverki> tempList, DateTime dateMail, 
            int numberMail, int pageCountReestr,string directory)
        {
            int pageCountAkts = 0;
            foreach (var item in tempList)
            {
                pageCountAkts += item.NumberOfPagesInSoursePdf.Count;
            }

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            string fileName ="исх.№91-"+ numberMail + " от "+ dateMail.ToString("d")+ "г. Акты ПР ФЛ";
            string filePath = System.IO.Path.Combine(directory, fileName + ".docx");
            File.Copy("Shabloni\\mail.docx", filePath);

            var valuesToFill = new Content(
    new FieldContent("NumberMail", numberMail.ToString()),
    new FieldContent("DateMail", dateMail.ToString("d")),
    new FieldContent("CountPageReestr", pageCountReestr.ToString()),
     new FieldContent("CountAkts", tempList.Count.ToString()),
      new FieldContent("CountPageAkts", pageCountAkts.ToString()),
    new FieldContent("MounthAkts", GetMounth(tempList[0].DateWork?.Month)),
 new FieldContent("YearAkts",tempList[0].DateWork?.Year.ToString())
);
            using (var outputDocument = new TemplateProcessor(filePath)
                .SetRemoveContentControls(true))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();
            }
            return filePath;

        }

        private static string GetMounth(int? month)
        {
            switch (month)
            {
               case 1:return "Январь";
                case 2: return "Февраль";
                case 3: return "Март";
                case 4: return "Апрель";
                case 5: return "Май";
                case 6: return "Июнь";
                case 7: return "Июль";
                case 8: return "Август"; 
                case 9: return "Сентябрь";
                case 10: return "Октябрь";
                case 11: return "Ноябрь";
                case 12: return "Декабрь";
                default:return "";
                   
            }
        }

        internal static string ConvertDocxToPdf(string docxPath)
        {
            Microsoft.Office.Interop.Word.Document wordDocument;
            Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
            string pdfPath = Path.ChangeExtension(docxPath, ".pdf");
            wordDocument = appWord.Documents.Open(docxPath);
            wordDocument.ExportAsFixedFormat(pdfPath, WdExportFormat.wdExportFormatPDF);
            return pdfPath;
        }
    }
}
