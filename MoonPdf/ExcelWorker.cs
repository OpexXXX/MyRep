using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data;
using System.Collections;
using OfficeOpenXml;
using System.Drawing;
using System.IO;
using OfficeOpenXml.Style;
using ExcelCOM = Microsoft.Office.Interop.Excel;
using System.ComponentModel.DataAnnotations;
using ExcelLibrary;
using ExcelDataReader;

namespace MoonPdf
{
    public class ExcelWorker
    {
        private List<string[]> HeaderColumn;
        public enum AktType
        {
            Proverka,
            Dopusk
        }
        public ExcelWorker()
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
        }
        private DataSet MakeDataSet(AllATPObserv akti)
        {
            DataSet Result = new DataSet("Reestr");
            AllATPObserv proverki = new AllATPObserv();
            AllATPObserv dopuski = new AllATPObserv();
            AllATPObserv demontag = new AllATPObserv();
            DataTable table;
            foreach (aktATP item in akti)
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
        private DataTable MakeDataTable(AllATPObserv Temp_akti)
        {
            DataTable table = new DataTable("Reestr");
            DataColumn column;
            DataRow row;
            List<aktATP> akti = new List<aktATP>();
            foreach (aktATP item in Temp_akti)
            {
                akti.Add(item);
            }
            akti.Sort();
            // parts.Sort(delegate (Part x, Part y);
            // Создаем новый стобец указываем тип 
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
            foreach (aktATP item in akti)
            {
                row = table.NewRow();
                row["Number"] = item.Number;
                row["DateWork"] = item.DateWork.ToString("d");
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
        private void SetDefaultOptions(Dictionary<string, string> options)
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
        private void SetWorkSheetStyles(ExcelWorksheet worksheet, Dictionary<string, string> options)
        {
            worksheet.TabColor = (Color)typeof(Color).GetProperty(options["Worksheet.TabColor"]).GetValue(null, null);
            worksheet.DefaultRowHeight = Int32.Parse(options["Worksheet.DefaultRowHeight"]);
            worksheet.DefaultColWidth = Int32.Parse(options["Worksheet.DefaultColWidth"]);
            worksheet.HeaderFooter.FirstFooter.LeftAlignedText = string.Format("Generated: {0}", DateTime.Now.ToShortDateString());
            worksheet.Row(2).Height = 45;
        }
        private void SetThStyle(ExcelRange range, Dictionary<string, string> options)
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
        private void SetHeaderStyle(ExcelRange range, Dictionary<string, string> options)
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor((Color)typeof(Color).GetProperty(options["Td.Style.Fill.BackgroundColor"]).GetValue(null, null));
            range.Style.Font.Color.SetColor((Color)typeof(Color).GetProperty(options["Td.Style.Fill.Color"]).GetValue(null, null));
            range.Style.Font.Size = 14;
            range.Style.ShrinkToFit = false;
            range.Style.WrapText = false;

        }
        private void SetTdStyle(ExcelRange range, Dictionary<string, string> options)
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
        public void DataTableToExcel(AllATPObserv akti, string path, Dictionary<string, string> options = null)
        {
            if (options == null)
                options = new Dictionary<string, string>();
            SetDefaultOptions(options);
            //Создаем фаил
            
            using (var file = new FileStream(path, FileMode.Create))
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
            

            ExcelCOM.Application excelapp;
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
            excelapp.Quit();


        }
        public DataSet makeDataSetForSAPFL(FileStream excelFilePath)
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
    }
}

