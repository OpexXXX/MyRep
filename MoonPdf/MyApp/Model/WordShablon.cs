using ATPWork.MyApp.Model.AktBuWork;
using ATPWork.MyApp.Model.VnePlan;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Office.Interop.Word;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using TemplateEngine.Docx;

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

        internal static string CreatePdfWithPhoto(AktBu akt)
        {
            ObservableCollection<string> photoFile = akt.PhotoFile;

            if (!Directory.Exists(TempDocxDirectory)) Directory.CreateDirectory(TempDocxDirectory);
            string fileName = System.IO.Path.GetRandomFileName() + ".pdf";
            string filePath = System.IO.Path.Combine(TempDocxDirectory, fileName);
            iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4);
            //
            var output = new FileStream(filePath, FileMode.Create);
            var writer = PdfWriter.GetInstance(doc, output);
            doc.Open();

            foreach (var item in photoFile)
            {
                string photoPathEn = ResizeImage(item);


                var logo = iTextSharp.text.Image.GetInstance(photoPathEn);


                if (logo.Height > logo.Width)
                {
                    //Maximum height is 800 pixels.

                    Chunk c2 = new Chunk("");
                    doc.SetPageSize(PageSize.A4);
                    doc.NewPage();
                    float percentage = 0.0f;
                    percentage = 700 / logo.Height;
                    logo.ScalePercent(percentage * 100);


                    doc.Add(c2);
                    doc.Add(logo);
                }
                else
                {



                    Chunk c2 = new Chunk("");
                    doc.SetPageSize(PageSize.A4.Rotate());
                    doc.NewPage();

                    //Maximum width is 600 pixels.
                    float percentage = 0.0f;
                    percentage = 700 / logo.Width;
                    logo.ScalePercent(percentage * 100);

                    doc.Add(c2);
                    doc.Add(logo);
                }


            }
            doc.Close();
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

new FieldContent("DateB", agent.DateB),
new FieldContent("PlaceB", agent.PlaceB)
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

        internal static Microsoft.Office.Interop.Word.Application appWord;

        internal static string ConvertDocxToPdf(string docxPath)
        {
            if (appWord == null) appWord = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document wordDocument;
            string pdfPath = Path.ChangeExtension(docxPath, ".pdf");
            wordDocument = appWord.Documents.Open(docxPath);
            wordDocument.ExportAsFixedFormat(pdfPath, WdExportFormat.wdExportFormatPDF);
            wordDocument.Close();
            return pdfPath;
        }
        internal static string CreatePdfPoliceMail(AktBu akt)
        {
            string resultPdfPath = "", pathMail, pathOb1, pathOb2, photo;

            List<string> FileToBlind = new List<string>();
            pathMail = ConvertDocxToPdf(WordShablon.CreatePoliceMailForBuAkt(akt));
            FileToBlind.Add(pathMail);
            FileToBlind.Add(akt.AktBuPdf);
            pathOb1 = ConvertDocxToPdf(WordShablon.CreateObysnenyaForBuAkt(akt, akt.Agent_1));
            FileToBlind.Add(pathOb1);
            if (akt.Agent_2 != null)
            {
                pathOb2 = ConvertDocxToPdf(WordShablon.CreateObysnenyaForBuAkt(akt, akt.Agent_2));
                FileToBlind.Add(pathOb2);
            }

            if (akt.PhotoFile.Count > 0)
            {
                photo = CreatePdfWithPhoto(akt);
                FileToBlind.Add(photo);
            }

            resultPdfPath = margePdf(FileToBlind);
            return resultPdfPath;
        }
        internal static string CreateFullPdfMail(AktBu akt)
        {
            string resultPdfPath = "", pathMail, pathRaschet, photo;

            List<string> FileToBlind = new List<string>();
            pathMail = ConvertDocxToPdf(WordShablon.CreateMailForBuAkts(akt));
            FileToBlind.Add(pathMail);

            pathRaschet = ConvertDocxToPdf(WordShablon.CreateRaschetForBuAkt(akt));
            FileToBlind.Add(pathRaschet);
            FileToBlind.Add(akt.AktBuPdf);
            FileToBlind.Add(akt.AktProverkiPdf);
            FileToBlind.Add(akt.IzvesheniePDF);

            if (akt.PhotoFile.Count > 0)
            {
                photo = CreatePdfWithPhoto(akt);
                FileToBlind.Add(photo);
            }
            resultPdfPath = margePdf(FileToBlind);
            return resultPdfPath;
        }



        private static readonly int MaxImageSize = 1280;

        private static string ResizeImage(string imagePath)
        {
            var image = System.Drawing.Image.FromFile(imagePath);
            if (image.Height <= MaxImageSize && image.Width <= MaxImageSize)
                return imagePath;

            var ratio = image.Width >= image.Height
                ? (float)MaxImageSize / image.Width
                : (float)MaxImageSize / image.Height;
            var nWidth = ratio * image.Width;
            var nHeight = ratio * image.Height;

            var bm = new System.Drawing.Bitmap((int)(nWidth), (int)(nHeight), PixelFormat.Format16bppRgb565);
            using (var gx = System.Drawing.Graphics.FromImage(bm))
            {
                gx.Clear(System.Drawing.Color.White);
                gx.CompositingQuality = CompositingQuality.HighQuality;
                gx.SmoothingMode = SmoothingMode.HighQuality;
                gx.InterpolationMode = InterpolationMode.HighQualityBicubic;

                gx.DrawImage(image,
                    new System.Drawing.RectangleF(0, 0, nWidth, nHeight),
                    new System.Drawing.Rectangle(0, 0, image.Width, image.Height), System.Drawing.GraphicsUnit.Pixel);

            }
            //
            string savePath = Path.Combine(TempDocxDirectory, Path.GetRandomFileName() + ".jpg");

            var jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            var myEncoder = System.Drawing.Imaging.Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, 40L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bm.Save(savePath, jgpEncoder, myEncoderParameters);

            image.Dispose();
            return savePath;
        }
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        private static string margePdf(List<string> filePDF)
        {
            if (!Directory.Exists(TempDocxDirectory)) Directory.CreateDirectory(TempDocxDirectory);
            string fileName = System.IO.Path.GetRandomFileName() + ".pdf";
            string filePath = System.IO.Path.Combine(TempDocxDirectory, fileName);


            using (FileStream FStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {

                iTextSharp.text.Document doc = new iTextSharp.text.Document();
                iTextSharp.text.pdf.PdfCopy Writer = new iTextSharp.text.pdf.PdfCopy(doc, FStream);
                Writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_5);
                Writer.SetFullCompression();
                Writer.CompressionLevel = PdfStream.BEST_COMPRESSION;
                doc.Open();
                iTextSharp.text.pdf.PdfReader ReaderDoc1;
                foreach (var filePathPdf in filePDF)
                {
                    if (filePathPdf != null)
                    {
                        ReaderDoc1 = new iTextSharp.text.pdf.PdfReader(filePathPdf);
                        Writer.AddDocument(ReaderDoc1);
                    }
                }
                doc.Close();
                return filePath;
            }


        }
    }
}
