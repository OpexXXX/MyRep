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
using ATPWork.MyApp.Model.AktBuWork;

namespace ATPWork.MyApp.Model
{
    public static class WordShablon
    {
        private static string _shablonDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Shabloni");
        public static string ShablonDirectory
        {
            get { return _shablonDirectory; }
            set { _shablonDirectory = value; }
        }
        private static string _tempDocxDirectory = Path.Combine(Directory.GetCurrentDirectory(), "vnePlan");
        public static string TempDocxDirectory
        {
            get { return _tempDocxDirectory; }
            set { _tempDocxDirectory = value; }
        }
        public static string CreateVnePlanZayvka(VnePlanZayavka zayav)
        {
            if (!Directory.Exists(TempDocxDirectory)) Directory.CreateDirectory(TempDocxDirectory);
            string fileName = System.IO.Path.GetRandomFileName();
            string filePath;
            filePath = Path.Combine(TempDocxDirectory, fileName + ".docx");
            File.Copy(Path.Combine(ShablonDirectory, "Zayavka.docx"), filePath);
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
        public static string CreateMailForBuAkts(AktBu akt)
        {
            if (!Directory.Exists(TempDocxDirectory)) Directory.CreateDirectory(TempDocxDirectory);
            string fileName = System.IO.Path.GetRandomFileName() + ".docx";
            string filePath = System.IO.Path.Combine(TempDocxDirectory, fileName);
            string shablonName, shablonPath;
            switch (akt.TypeNarushenie)
            {
                case VidNarusheniya.NoPower: { shablonName = "mail_62NoPower.docx"; break; }
                case VidNarusheniya.Power: { shablonName = "mail_62.docx"; break; }
                case VidNarusheniya.Vmeshatelstvo: { shablonName = "mail_81-11.docx"; break; }
                default: { shablonName = "mail_81-11.docx"; break; }
            }
            shablonPath = Path.Combine(ShablonDirectory, "BU", shablonName);
            File.Copy(shablonPath, filePath);
            Content valuesToFill;
            valuesToFill = new Content(
                  new FieldContent("DateMail", akt.DateMail?.ToString("d")),
                    new FieldContent("NumberMail", akt.NumberMail.ToString()),
new FieldContent("FIO", akt.FIO),
new FieldContent("NumberAktBu", akt.Number.ToString()),
new FieldContent("DateAktBu", akt.DateWork?.ToString("d")),
new FieldContent("ValueBu", akt.TypeNarushenie == VidNarusheniya.Power ? akt.BuValuePower.ToString() : akt.BuValueNormativ.ToString()),
new FieldContent("Adress", akt.Adress),
new FieldContent("NumberLs", akt.NumberLS),
new FieldContent("MounthYearPo", GetMounth(akt.DateMail?.Month) + " " + akt.DateMail?.Year.ToString())
);

            using (var outputDocument = new TemplateProcessor(filePath)
                 .SetRemoveContentControls(true))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();
            }
            return filePath;
        }
        public static string CreateRaschetForBuAkt(AktBu akt)
        {
            if (!Directory.Exists(TempDocxDirectory)) Directory.CreateDirectory(TempDocxDirectory);
            string fileName = System.IO.Path.GetRandomFileName() + ".docx";
            string filePath = System.IO.Path.Combine(TempDocxDirectory, fileName);
            string shablonName, shablonPath;
            switch (akt.TypeNarushenie)
            {
                case VidNarusheniya.NoPower: { shablonName = "CalcNoPower.docx"; break; }
                case VidNarusheniya.Power: { shablonName = "CalcPower.docx"; break; }
                case VidNarusheniya.Vmeshatelstvo: { shablonName = "CalcNormativVmishatelstvo.docx"; break; }
                default: { shablonName = "CalcNoPower.docx"; break; }
            }
            shablonPath = Path.Combine(ShablonDirectory, "BU", shablonName);
            File.Copy(shablonPath, filePath);
            Content valuesToFill;
            switch (akt.TypeNarushenie)
            {
                case VidNarusheniya.Power:
                    {
                        valuesToFill = new Content(
    new FieldContent("numberBu", akt.Number.ToString()),
    new FieldContent("DateBu", akt.DateWork?.ToString("d")),
     new FieldContent("StartDate", akt.StartDate?.ToString("d")),
    new FieldContent("CountDay", akt.CountDay.ToString()),
     new FieldContent("CountHours", (akt.CountDay * 24).ToString()),
      new FieldContent("Power", akt.Power.ToString()),
    new FieldContent("ValueBu", akt.BuValuePower.ToString())
);
                        break;
                    }
                default:
                    {
                        valuesToFill = new Content(
    new FieldContent("numberBu", akt.Number.ToString()),
    new FieldContent("DateBu", akt.DateWork?.ToString("d")),
      new FieldContent("StartDate", akt.StartDate?.ToString("d")),
    new FieldContent("CountDay", akt.CountDay.ToString()),
     new FieldContent("CountPeople", akt.PeopleCount.ToString()),
      new FieldContent("CountRooms", akt.RoomCount.ToString()),
      new FieldContent("ElecttroBoller", akt.NormativKat == 4 ? "Да" : "Нет"),
      new FieldContent("Normativ", akt.Normativ.ToString()),
    new FieldContent("ValueBu", akt.BuValueNormativ.ToString())
); break;
                    }
            }
            using (var outputDocument = new TemplateProcessor(filePath)
                 .SetRemoveContentControls(true))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();
            }
            return filePath;
        }

        public static string CreatePoliceMailForBuAkt(AktBu akt)
        {
            if (!Directory.Exists(TempDocxDirectory)) Directory.CreateDirectory(TempDocxDirectory);
            string fileName = System.IO.Path.GetRandomFileName() + ".docx";
            string filePath = System.IO.Path.Combine(TempDocxDirectory, fileName);
            string shablonName = "Police.docx", shablonPath;
            shablonPath = Path.Combine(ShablonDirectory, "BU", shablonName);
            File.Copy(shablonPath, filePath);
            Content valuesToFill;

            string sotrudnikami = "";
            if (akt.Agent_2 != null)
            {
                /*сотрудники филиала ПАО «МРСК Сибири»-«Красноярскэнерго» электромонтер УТЭЭ  Глушков Александр Сергеевич, электромонтер УТЭЭ  Назаров Дмитрий Анатольевич ,*/
                sotrudnikami = "сотрудники филиала ПАО «МРСК Сибири»-«Красноярскэнерго» " + akt.Agent_1.Post + " " + akt.Agent_1.Surname + ", " + akt.Agent_2.Post + " " + akt.Agent_2.Surname + ",";
            }
            else
            {
                sotrudnikami = "сотрудник филиала ПАО «МРСК Сибири»-«Красноярскэнерго» " + akt.Agent_1.Post + " " + akt.Agent_1.Surname + " ";
            }

            valuesToFill = new Content(
new FieldContent("DateWork", akt.DateWork?.ToString("d")),
new FieldContent("PotrFIO", akt.FIO),
new FieldContent("Adress", akt.Adress),
new FieldContent("BuValue", akt.TypeNarushenie == VidNarusheniya.Power ? akt.BuValuePower.ToString() : akt.BuValueNormativ.ToString()),
new FieldContent("NumberBU", akt.Number.ToString()),
new FieldContent("StartDate", akt.StartDate?.ToString("d")),
new FieldContent("PunktPP", akt.TypeNarushenie == VidNarusheniya.Power ? "62" : "81(11)"),
new FieldContent("CountPageOb", akt.Agent_2 == null ? "1" : "2"),
new FieldContent("Li", akt.Agent_2 == null ? "л" : "ли"),

new FieldContent("Sotrudniki", sotrudnikami)

);

            using (var outputDocument = new TemplateProcessor(filePath)
                 .SetRemoveContentControls(true))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();
            }
            return filePath;
        }
        public static string CreateObysnenyaForBuAkt(AktBu akt, Agent agent)
        {

            if (!Directory.Exists(TempDocxDirectory)) Directory.CreateDirectory(TempDocxDirectory);
            string fileName = System.IO.Path.GetRandomFileName() + ".docx";
            string filePath = System.IO.Path.Combine(TempDocxDirectory, fileName);
            string shablonName = "Obesneniya.docx", shablonPath;
            shablonPath = Path.Combine(ShablonDirectory, "BU", shablonName);
            File.Copy(shablonPath, filePath);
            Content valuesToFill;
            valuesToFill = new Content(
new FieldContent("DateWork", akt.DateWork?.ToString("d")),
new FieldContent("AbonentFIO", akt.FIO),
new FieldContent("FIO", agent.Surname),
new FieldContent("Adress", akt.Adress),
new FieldContent("NumberBU", akt.Number.ToString()),
new FieldContent("TextNarushenia", akt.Narushenie),
new FieldContent("Post", agent.Post),

new FieldContent("DateB", agent.Surname),//////////////////////////
new FieldContent("PlaceB", agent.Surname)//////////////////////////
);

            using (var outputDocument = new TemplateProcessor(filePath)
                 .SetRemoveContentControls(true))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();
            }
            return filePath;
        }
        public static void CreateContentForBuAkt()
        { }
        public static void CreatePZForBuAkt()
        { }



        internal static string CreateMailForAktsTehProverki(List<AktTehProverki> tempList, DateTime dateMail,
            int numberMail, int pageCountReestr, string directory)
        {
            int pageCountAkts = 0;
            foreach (var item in tempList)
            {
                pageCountAkts += item.NumberOfPagesInSoursePdf.Count;
            }

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            string fileName = "исх.№91-" + numberMail + " от " + dateMail.ToString("d") + "г. Акты ПР ФЛ";
            string filePath = System.IO.Path.Combine(directory, fileName + ".docx");
            File.Copy("Shabloni\\mail.docx", filePath);

            var valuesToFill = new Content(
    new FieldContent("NumberMail", numberMail.ToString()),
    new FieldContent("DateMail", dateMail.ToString("d")),
    new FieldContent("CountPageReestr", pageCountReestr.ToString()),
     new FieldContent("CountAkts", tempList.Count.ToString()),
      new FieldContent("CountPageAkts", pageCountAkts.ToString()),
    new FieldContent("MounthAkts", GetMounth(tempList[0].DateWork?.Month)),
 new FieldContent("YearAkts", tempList[0].DateWork?.Year.ToString())
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
                case 1: return "Январь";
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
                default: return "";

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
