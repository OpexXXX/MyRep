using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Data;
using OfficeOpenXml;
using System.Drawing;
using System.IO;
using OfficeOpenXml.Style;
using ExcelCOM = Microsoft.Office.Interop.Excel;
using ExcelDataReader;
using iTextSharp.text.pdf;
using iTextSharp.text;
using ATPWork.MyApp.Model.Plan;

namespace MyApp.Model
{
    public static class ExcelWorker
    {
        private static List<string[]> HeaderColumn = new List<string[]>();
        private static List<string[]> HeaderColumnPlan = new List<string[]>();
        public enum AktType
        {
            Proverka,
            Dopusk
        }
        public static void InitHeader()
        {
            HeaderColumn = new List<string[]>(2);
            HeaderColumn.Add(new string[] { "№ пп", "id" });
            HeaderColumn.Add(new string[] { "№ акта", "Number" });
            HeaderColumn.Add(new string[] { "Дата составления акта", "DateWork" });
            HeaderColumn.Add(new string[] { "Наименование потребителя", "FIO" });
            HeaderColumn.Add(new string[] { "№ точки учета", "NumbeLS" });
            HeaderColumn.Add(new string[] { "Адрес местонахождения электроустновки", "Adress" });
            HeaderColumn.Add(new string[] { "Тип ПУ", "PuType" });
            HeaderColumn.Add(new string[] { "Номер ПУ", "PuNumber" });
            HeaderColumn.Add(new string[] { "Показание", "Pokazanie" });

            HeaderColumnPlan = new List<string[]>(2);
            HeaderColumnPlan.Add(new string[] { "№ пп", "id" });
            HeaderColumnPlan.Add(new string[] { "№ Л/С", "NumberLS" });
            HeaderColumnPlan.Add(new string[] { "Дата проверки", "DateWork" });
            HeaderColumnPlan.Add(new string[] { "Ф.И.О", "FIO" });
            HeaderColumnPlan.Add(new string[] { "Адрес", "Adress" });
            HeaderColumnPlan.Add(new string[] { "Тип ПУ", "PuType" });
            HeaderColumnPlan.Add(new string[] { "Номер ПУ", "PuNumber" });
            HeaderColumnPlan.Add(new string[] { "Показание", "Pokazanie" });
            HeaderColumnPlan.Add(new string[] { "Подключение", "Podkluchenie" });
            HeaderColumnPlan.Add(new string[] { "Пломбы", "Plombs" });
            HeaderColumnPlan.Add(new string[] { "Расчет", "Raschet" });
            HeaderColumnPlan.Add(new string[] { "Заявки внеплан", "Vneplan" });
        }
        private static DataSet MakeDataSet(List<AktTehProverki> akti)
        {
            DataSet Result = new DataSet("Reestr");
            List<AktTehProverki> proverki = new List<AktTehProverki>();
            List<AktTehProverki> dopuski = new List<AktTehProverki>();
            List<AktTehProverki> demontag = new List<AktTehProverki>();
            DataTable table;
            foreach (AktTehProverki item in akti)
            {
                if (item.DopuskFlag) dopuski.Add(item);
                else proverki.Add(item);
            }
            if (proverki.Count > 0)
            {
                table = MakeDataTable(proverki);
                table.TableName = "Акты Проверок ПУ";
                Result.Tables.Add(table);
            }
            if (dopuski.Count > 0)
            {
                table = MakeDataTable(dopuski);
                table.TableName = "Акты Допуска ПУ";
                Result.Tables.Add(table);
            }
            if (demontag.Count > 0)
            {
                table = MakeDataTable(demontag);
                table.TableName = "Акты Демонтажа ПУ";
                Result.Tables.Add(table);
            }
            return Result;
        }
        private static DataTable MakeDataTable(List<AktTehProverki> Temp_akti)
        {
            DataTable table = new DataTable("Reestr");
            DataColumn column;
            DataRow row;
            List<AktTehProverki> akti = new List<AktTehProverki>();
            foreach (AktTehProverki item in Temp_akti)
            {
                akti.Add(item);
            }
            akti.Sort(delegate (AktTehProverki akt1, AktTehProverki akt2)
            { return akt1.Number.CompareTo(akt2.Number); });

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "id";
            column.Caption = "№ пп";
            column.ReadOnly = true;
            column.AutoIncrement = true;
            column.Unique = true;
            table.Columns.Add(column);
            for (int i = 1; i < HeaderColumn.Count; i++)
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = HeaderColumn[i][1];
                column.AutoIncrement = false;
                column.Caption = HeaderColumn[i][0];
                column.ReadOnly = false;
                column.Unique = false;
                table.Columns.Add(column);
            }
            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = table.Columns["id"];
            table.PrimaryKey = PrimaryKeyColumns;
            foreach (AktTehProverki item in akti)
            {
                row = table.NewRow();

                row["Number"] = item.Number;
                row["DateWork"] = item.DateWork?.ToString("d");
                row["NumbeLS"] = item.NumberLS;
                row["FIO"] = item.FIO;
                row["Adress"] = item.Adress;
                row["PuType"] = item.DopuskFlag ? item.PuNewType.Nazvanie : item.PuOldType;
                row["PuNumber"] = item.DopuskFlag ? item.PuNewNumber : item.PuOldNumber;
                row["Pokazanie"] = item.DopuskFlag ? item.PuNewPokazanie : item.PuOldPokazanie;
                table.Rows.Add(row);
            }
            return table;
        }
        private static void SetDefaultOptions(Dictionary<string, string> options)
        {
            if (!options.ContainsKey("Worksheet.name")) options["Worksheet.Name"] = "Реестр";
            if (!options.ContainsKey("Worksheet.TabColor")) options["Worksheet.TabColor"] = "Blue";
            if (!options.ContainsKey("Worksheet.DefaultColWidth")) options["Worksheet.DefaultColWidth"] = "3";
            if (!options.ContainsKey("Worksheet.DefaultRowHeight")) options["Worksheet.DefaultRowHeight"] = "12";
            if (!options.ContainsKey("Th.Style.Font.Bold")) options["Th.Style.Font.Bold"] = "true";
            if (!options.ContainsKey("Th.Style.Fill.BackgroundColor")) options["Th.Style.Fill.BackgroundColor"] = "White";
            if (!options.ContainsKey("Th.Style.Fill.Color")) options["Th.Style.Fill.Color"] = "Black";
            if (!options.ContainsKey("Td.Style.Font.Bold")) options["Td.Style.Font.Bold"] = "false";
            if (!options.ContainsKey("Td.Style.Fill.BackgroundColor")) options["Td.Style.Fill.BackgroundColor"] = "White";
            if (!options.ContainsKey("Td.Style.Fill.Color")) options["Td.Style.Fill.Color"] = "Black";
        }
        private static void SetWorkSheetStyles(ExcelWorksheet worksheet, Dictionary<string, string> options)
        {
            worksheet.TabColor = (Color)typeof(Color).GetProperty(options["Worksheet.TabColor"]).GetValue(null, null);
            worksheet.DefaultRowHeight = Int32.Parse(options["Worksheet.DefaultRowHeight"]);
            worksheet.DefaultColWidth = Int32.Parse(options["Worksheet.DefaultColWidth"]);
            worksheet.HeaderFooter.FirstFooter.LeftAlignedText = string.Format("Generated: {0}", DateTime.Now.ToShortDateString());
            worksheet.Row(2).Height = 45;
        }
        private static void SetThStyle(ExcelRange range, Dictionary<string, string> options)
        {
            range.AutoFilter = true;
            range.Style.Font.Bold = Boolean.Parse(options["Th.Style.Font.Bold"]);
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor((Color)typeof(Color).GetProperty(options["Th.Style.Fill.BackgroundColor"]).GetValue(null, null));
            range.Style.Font.Color.SetColor((Color)typeof(Color).GetProperty(options["Th.Style.Fill.Color"]).GetValue(null, null));
            range.Style.Font.Size = 12;
            range.Style.ShrinkToFit = false;
            range.Style.WrapText = true;

            foreach (var item in range)
            {
                item.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
        }
        private static void SetHeaderStyle(ExcelRange range, Dictionary<string, string> options)
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor((Color)typeof(Color).GetProperty(options["Td.Style.Fill.BackgroundColor"]).GetValue(null, null));
            range.Style.Font.Color.SetColor((Color)typeof(Color).GetProperty(options["Td.Style.Fill.Color"]).GetValue(null, null));
            range.Style.Font.Size = 14;
            range.Style.ShrinkToFit = false;
            range.Style.WrapText = false;

        }
        private static void SetTdStyle(ExcelRange range, Dictionary<string, string> options)
        {
            range.Style.Font.Bold = Boolean.Parse(options["Td.Style.Font.Bold"]);
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor((Color)typeof(Color).GetProperty(options["Td.Style.Fill.BackgroundColor"]).GetValue(null, null));
            range.Style.Font.Color.SetColor((Color)typeof(Color).GetProperty(options["Td.Style.Fill.Color"]).GetValue(null, null));
            range.Style.ShrinkToFit = false;

            foreach (var item in range)
            {
                item.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
        }

  
        public static DataTable MakeDataTableForPlan(List<Abonent> Temp_abonents)
        {
            DataTable table = new DataTable("Reestr");
            DataColumn column;
            DataRow row;
            List<Abonent> abonents = new List<Abonent>(Temp_abonents);

           // akti.Sort(delegate (AktTehProverki akt1, AktTehProverki akt2)
           // { return akt1.Number.CompareTo(akt2.Number); });

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "id";
            column.Caption = "№ пп";
            column.ReadOnly = true;
            column.AutoIncrement = true;
            column.Unique = true;
            table.Columns.Add(column);
            for (int i = 1; i < HeaderColumnPlan.Count; i++)
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = HeaderColumnPlan[i][1];
                column.AutoIncrement = false;
                column.Caption = HeaderColumnPlan[i][0];
                column.ReadOnly = false;
                column.Unique = false;
                table.Columns.Add(column);
            }

            DataColumn[] PrimaryKeyColumns = new DataColumn[1];

            PrimaryKeyColumns[0] = table.Columns["id"];
            table.PrimaryKey = PrimaryKeyColumns;
            foreach (Abonent item in abonents)
            {
                row = table.NewRow();

                row["NumberLS"] = item.NumberLS;
                row["DateWork"] = item.DateWork.ToString("d");
                row["FIO"] = item.FIO;
                row["Adress"] = item.Adress;
                row["PuType"] = item.PuOldType;
                row["PuNumber"] = item.PuOldNumber;
                row["Podkluchenie"] = item.Podkl;  
                row["Pokazanie"] = "";
                string plobms="";
                foreach (Plomba plomb in item.OldPlombs)
                {
                    if (plomb.Status=="I")
                    {
                        plobms += plomb.Number + " " + plomb.Place + ";\n";
                    }
                   
                }
                row["Plombs"] = plobms;
                row["Raschet"] = item.Normativ;
                 row["Vneplan"] = item.Vneplan;
                table.Rows.Add(row);
            }
            return table;
        }

        public static void DataTableToExcel(List<AktTehProverki> akti, string mailPath, Dictionary<string, string> options = null)
        {
            if (options == null)
                options = new Dictionary<string, string>();
            SetDefaultOptions(options);
            //Создаем фаил


            using (var file = new FileStream(mailPath + "\\Реестр.xlsx", FileMode.Create))
            {
                int rowIndex = 1;
                ExcelPackage package = new ExcelPackage(file);
                // Добавляем новый Лист
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(options["Worksheet.Name"]);
                // Устанавливаем форматирование всего листа
                SetWorkSheetStyles(worksheet, options);
                //Записываем заголовок
                DataSet data_set = MakeDataSet(akti);

                foreach (DataTable dt in data_set.Tables)
                {

                    worksheet.Cells[rowIndex, 1].Value = dt.TableName;
                    SetHeaderStyle(worksheet.Cells[rowIndex, 1], options);
                    rowIndex++;
                    //Пишем заголовки таблицы
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        worksheet.Cells[rowIndex, i + 1].Value = dt.Columns[i].Caption;
                    }
                    // Форматируем заголовки таблицы
                    using (var range = worksheet.Cells[rowIndex, 1, rowIndex, dt.Columns.Count])
                    {
                        SetThStyle(range, options);
                    }
                    rowIndex++;
                    //Заполняем Таблицу
                    int row_start_pos = rowIndex;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        for (int j = 0; j < dt.Columns.Count; j++)
                        {

                            worksheet.Cells[i + row_start_pos, j + 1].Value = dt.Rows[i][j];
                            if (j == 0) worksheet.Cells[i + row_start_pos, j + 1].Value = (int)dt.Rows[i][j] + 1;
                            if (j == dt.Columns.Count - 1)
                            {
                                using (var range = worksheet.Cells[i + row_start_pos, 1, i + row_start_pos, dt.Columns.Count])
                                {
                                    SetTdStyle(range, options);
                                }
                            }
                        }
                        rowIndex++;
                    }

                }
                // Растягиваем столбцы
                List<double> width = new List<double>();
                width.Add(4);
                width.Add(7);
                width.Add(13);
                width.Add(30);
                width.Add(13);
                width.Add(47);
                width.Add(30);
                width.Add(14);
                width.Add(12);
                worksheet.Column(1).Width = width[0];
                for (int i = 1; i < width.Count; i++)
                {
                    worksheet.Column(i + 1).AutoFit(width[i]);
                }
                //Сохраняем фаил
                package.Save();
            }

            //Открываем экселем сохраняем в пдф
            /*  ExcelCOM.Application excelapp;
              ExcelCOM.Workbook wb;
              ExcelCOM.Workbooks wbs;
              ExcelCOM.Worksheet wsh;
              excelapp = new ExcelCOM.Application();
              excelapp.DisplayAlerts = false;
              excelapp.Visible = false;
              wbs = excelapp.Workbooks;
                          string save = path;
              wbs.Open(save);
              wb = excelapp.ActiveWorkbook;
              wsh = wb.Sheets[1];
              wsh.PageSetup.Zoom = false;
              wsh.PageSetup.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape;
              wsh.PageSetup.FitToPagesWide = 1;
              wsh.PageSetup.FitToPagesTall = 10;
              save =  path.Replace(".xlsx", ".pdf");
              wb.ExportAsFixedFormat(ExcelCOM.XlFixedFormatType.xlTypePDF, save, Type.Missing, true, Type.Missing, Type.Missing, Type.Missing, false, Type.Missing);
              wbs.Close();
              excelapp.Quit();*/


        }


        public static DataSet makeDataSetForSAPFL(FileStream excelFilePath)
        {
            using (IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(excelFilePath))
            {
                try
                {
                    DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    reader.Close();
                    return result;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
            }

        }


        internal static void CreatePdfReestr(DataTable tableL)
        {
            string currentMailDirectory = MainAtpModel.MailDirektory;

          
            //Объект документа пдф
            iTextSharp.text.Document doc = new iTextSharp.text.Document();
            doc.SetPageSize(PageSize.A4.Rotate());


            //Создаем объект записи пдф-документа в файл
            PdfWriter.GetInstance(doc, new FileStream("pdfTables.pdf", FileMode.Create));
            //Открываем документ
            doc.Open();
            //Определение шрифта необходимо для сохранения кириллического текста
            //Иначе мы не увидим кириллический текст
            //Если мы работаем только с англоязычными текстами, то шрифт можно не указывать
            BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.NORMAL);

            BaseFont smalFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font smallFont = new iTextSharp.text.Font(baseFont, 6, iTextSharp.text.Font.NORMAL);

            //Обход по всем таблицам датасета (хотя в данном случае мы можем опустить
            //Так как в нашей бд только одна таблица)



            //Создаем объект таблицы и передаем в нее число столбцов таблицы из нашего датасета
            PdfPTable table = new PdfPTable(tableL.Columns.Count);
                table.TotalWidth = 800f;
                table.LockedWidth = true;

                var colWidthPercentages = new[] { 2f, 8f, 6f, 11f, 15f, 9f, 7f, 2f, 15f , 21f , 3f, 1f};
                table.SetWidths(colWidthPercentages);

                //Добавим в таблицу общий заголовок
                PdfPCell cell = new PdfPCell(new Phrase("БД " + tableL.TableName + ", таблица №" , font));

                cell.Colspan = tableL.Columns.Count;
                cell.HorizontalAlignment = 1;
                //Убираем границу первой ячейки, чтобы балы как заголовок
                cell.Border = 0;
                table.AddCell(cell);

                //Сначала добавляем заголовки таблицы
                for (int j = 0; j < tableL.Columns.Count; j++)
                {
                    cell = new PdfPCell(new Phrase(new Phrase(tableL.Columns[j].Caption, font)));
                    //Фоновый цвет (необязательно, просто сделаем по красивее)
                    cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                }

                //Добавляем все остальные ячейки
                for (int j = 0; j < tableL.Rows.Count; j++)
                {
                    for (int k = 0; k < tableL.Columns.Count; k++)
                    {
                        var value = tableL.Rows[j][k].ToString();
                        if (k == 0) value = (Int32.Parse(value) + 1).ToString();


                        table.AddCell(new Phrase(value, (k==8)||(k==9)? smallFont :font));
                    }
                }
                //Добавляем таблицу в документ
                doc.Add(table);
            
            //Закрываем документ
            doc.Close();

            MessageBox.Show("Pdf-документ сохранен");

        }


        internal static void CreatePdfReestr()
        {
            string currentMailDirectory = MainAtpModel.MailDirektory;
            DataSet data_set = MakeDataSet(MainAtpModel.AllAkt);
            //Объект документа пдф
            iTextSharp.text.Document doc = new iTextSharp.text.Document();
            doc.SetPageSize(PageSize.A4.Rotate());


            //Создаем объект записи пдф-документа в файл
            PdfWriter.GetInstance(doc, new FileStream("pdfTables.pdf", FileMode.Create));
             //Открываем документ
            doc.Open();
            //Определение шрифта необходимо для сохранения кириллического текста
            //Иначе мы не увидим кириллический текст
            //Если мы работаем только с англоязычными текстами, то шрифт можно не указывать
            BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.NORMAL);

            //Обход по всем таблицам датасета (хотя в данном случае мы можем опустить
            //Так как в нашей бд только одна таблица)


            for (int i = 0; i < data_set.Tables.Count; i++)
            {
                //Создаем объект таблицы и передаем в нее число столбцов таблицы из нашего датасета
                PdfPTable table = new PdfPTable(data_set.Tables[i].Columns.Count);
                table.TotalWidth = 800f;
                table.LockedWidth = true;

                var colWidthPercentages = new[] { 2f, 4f, 8f, 15f, 10f, 28f, 18f, 8f, 7f };
                table.SetWidths(colWidthPercentages);
               
                //Добавим в таблицу общий заголовок
                PdfPCell cell = new PdfPCell(new Phrase("БД " + data_set.Tables[i].TableName+ ", таблица №" + (i + 1), font));

                cell.Colspan = data_set.Tables[i].Columns.Count;
                cell.HorizontalAlignment = 1;
                //Убираем границу первой ячейки, чтобы балы как заголовок
                cell.Border = 0;
                table.AddCell(cell);

                //Сначала добавляем заголовки таблицы
                for (int j = 0; j < data_set.Tables[i].Columns.Count; j++)
                {
                    cell = new PdfPCell(new Phrase(new Phrase(data_set.Tables[i].Columns[j].Caption, font)));
                    //Фоновый цвет (необязательно, просто сделаем по красивее)
                    cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                }

                //Добавляем все остальные ячейки
                for (int j = 0; j < data_set.Tables[i].Rows.Count; j++)
                {
                    for (int k = 0; k < data_set.Tables[i].Columns.Count; k++)
                    {
                        var value = data_set.Tables[i].Rows[j][k].ToString();
                        if (k == 0) value = (Int32.Parse(value)+1).ToString();

                        table.AddCell(new Phrase(value, font));
                    }
                }
                //Добавляем таблицу в документ
                doc.Add(table);
            }
            //Закрываем документ
            doc.Close();

            MessageBox.Show("Pdf-документ сохранен");

        }



    }
}

