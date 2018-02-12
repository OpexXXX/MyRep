using ATPWork.MyApp.Model.VnePlan;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateEngine.Docx;

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

        public static void CreateMailForAktsTehProverki()
        { }
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

    }
}
