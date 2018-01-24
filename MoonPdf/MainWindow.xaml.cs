using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MoonPdfLib;
using MoonPdfLib.Helper;
using MoonPdfLib.MuPdf;
using Microsoft.Win32;
using System.Data.SQLite;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SAPFEWSELib;
using System.Threading;

namespace MoonPdf
{

    public partial class MainWindow : Window
    {


        public ATPWorker ATPWork;

        internal MoonPdfPanel MoonPdfPanel { get { return this.moonPdfPanel; } }
        public MainWindow()
        {
            InitializeComponent();


            this.Loaded += MainWindow_Loaded;
        }
        void EnableAtpTab(bool value)
        {
            groupBox.IsEnabled = value;
            groupBox1.IsEnabled = value;
            groupBox2.IsEnabled = value;
            groupBox3.IsEnabled = value;
            groupBox4.IsEnabled = value;
            groupBox5.IsEnabled = value;
            button2.IsEnabled = value;
            button3.IsEnabled = value;
            button4.IsEnabled = value;
            button5.IsEnabled = value;

            //comboBox3.IsEnabled = value;
        }
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            // Will not overwrite if the destination file already exists.

            try
            {
                string path = String.Format("BASE\\{0}_{1}_{2}_{3}_{4}_{5}_{6}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, "filename.db");
                File.Copy("filename.db", path);
            }
            catch (Exception)
            {

                MessageBox.Show("");
            }

            EnableAtpTab(false);
            ATPWork = new ATPWorker(moonPdfPanel);
            ATPWork.DbWork.AgentListInit(ATPWork.agents);
            ATPWork.DbWork.PUListInit(ATPWork.SpisokPU);
            ATPWork.DbWork.LoadCompleteATP(ATPWork);

            comboBox.ItemsSource = ATPWork.agents;
            comboBox1.ItemsSource = ATPWork.agents;
            DataContext = ATPWork.AktATPInWork;
            dataGrid1.ItemsSource = ATPWork.AllAtpInWorkList;

            ListCollectionView collection1 = new ListCollectionView(ATPWork.AllAtpInWorkList);
            collection1.GroupDescriptions.Add(new PropertyGroupDescription("Complete"));
            dataGrid1.ItemsSource = collection1;
            var yourCostumFilter1 = new Predicate<object>(item => ((aktATP)item).NumberMail == 0);
            ListCollectionView collection2 = new ListCollectionView(ATPWork.CompleteAtpWorkList);
            collection2.GroupDescriptions.Add(new PropertyGroupDescription("TypeOfWork"));
            collection2.Filter = yourCostumFilter1;
            dataGridComplete.ItemsSource = collection2;
            // your Filter
            var yourCostumFilter = new Predicate<object>(item => ((aktATP)item).NumberMail != 0);
            ListCollectionView collection = new ListCollectionView(ATPWork.CompleteAtpWorkList);
            collection.GroupDescriptions.Add(new PropertyGroupDescription("GroupOfMail"));
            collection.GroupDescriptions.Add(new PropertyGroupDescription("TypeOfWork"));
            collection.Filter = yourCostumFilter;
            DataGridMailedAkt.ItemsSource = collection;
            List<string> ll = new List<string>();
            ll.Add("2400_4");
            ll.Add("2400_5");
            ll.Add("2400_6");
            comboBox3.DataContext = ATPWork;
            comboBox2.ItemsSource = ATPWork.SpisokPU;
            comboBox3.ItemsSource = ATPWork.AllAtpInWorkList;



            /**
            ICollectionView cvTasks = CollectionViewSource.GetDefaultView(dataGridComplete.ItemsSource);
            if (cvTasks != null && cvTasks.CanGroup == true)
            {
                cvTasks.GroupDescriptions.Clear();
                cvTasks.GroupDescriptions.Add(new PropertyGroupDescription("NumberMail"));
            
            }*/

            dataGridComboType.ItemsSource = ll;
            // OpenFileDialog op = new OpenFileDialog();
            //  op.ShowDialog();
            //  this.moonPdfPanel.OpenFile(op.FileName);
            //  this.moonPdfPanel.ZoomToWidth();
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
            op.RestoreDirectory = true;
            op.Title = "Откройте PDF фаил";

            if (op.ShowDialog() == true)
            {
                // if (ATPWork != null) ;
                ;
                statusStripText.Text = "Добавлено " + ATPWork.addAtpToWork(op.FileName).ToString() + " актов для заполнения. Фаил " + op.FileName + " открыт.";
                // 
                EnableAtpTab(true);
                moonPdfPanel.ZoomToWidth();
                //refreshATPTab();
            }
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox3.SelectedIndex > 0)
            {
                comboBox3.SelectedIndex = comboBox3.SelectedIndex - 1;
                refreshATPTab();
            }
            moonPdfPanel.ZoomToWidth();
        }
        private void button5_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox3.SelectedIndex != -1) refreshATPTab();
            comboBox3.SelectedIndex = comboBox3.SelectedIndex + 1;
            moonPdfPanel.ZoomToWidth();
        }
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.GotoPage(ATPWork.AktATPInWork.NumberOfPagesInSoursePdf[0] + 1);
        }
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.GotoPage(ATPWork.AktATPInWork.NumberOfPagesInSoursePdf[1] + 1);
        }
        private void moonPdfPanel_PageRowDisplayChanged(object sender, EventArgs e)
        {

        }
        public void refreshATPTab()
        {
            if (ATPWork.AktATPInWork != null)
            {
                tabControl.DataContext = ATPWork.AktATPInWork;
                dataGrid.ItemsSource = ATPWork.AktATPInWork.plomb;
            }
        }
        private void comboBox3_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            if (comboBox3.SelectedIndex != -1)
            {
                refreshATPTab();
                EnableAtpTab(true);

            }
            else
            {

                // moonPdfPanel.Unload();
                EnableAtpTab(false);
            }
        }
        public void SetDataFromDb(string searchText, string from = "LS")
        {
            List<Dictionary<string, string>> tempList;
            if (from == "LS")
            {
                tempList = ATPWork.DbWork.GetAbonentFromLS(textBox1.Text);
            }
            else
            {
                tempList = ATPWork.DbWork.GetAbonentFromDbByPU(textBox6.Text);
            }

            if (tempList.Count == 1)
            {
                ATPWork.AktATPInWork.setDataByDb(tempList[0]);
                GridPU.Height = new GridLength(94);
                RowPU.Height = new GridLength(21);
                PotrGridColumn.Height = new GridLength(96);

            }
            else if (tempList.Count > 1)
            {
                ResultSearchWindow resultWindow = new ResultSearchWindow();
                List<ListBoxView> ListForDataGrid = new List<ListBoxView>();
                foreach (var item in tempList)
                {
                    ListForDataGrid.Add(new ListBoxView(item));
                }
                resultWindow.dataGrid.ItemsSource = ListForDataGrid;
                resultWindow.Owner = this;
                resultWindow.ShowDialog();
                if ((bool)resultWindow.DialogResult)
                {
                    ATPWork.AktATPInWork.setDataByDb(resultWindow.SelectVal);
                    GridPU.Height = new GridLength(94);
                    RowPU.Height = new GridLength(21);
                    PotrGridColumn.Height = new GridLength(96);
                }

                else
                {
                    PotrGridColumn.Height = new GridLength(53);
                    GridPU.Height = new GridLength(73);
                    RowPU.Height = new GridLength(0);
                    statusStripText.Text = "Не найденно в базе";
                }
            }
        }

        private void textBox6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (textBox6.Text.Length > 4)
                {
                    SetDataFromDb(textBox6.Text, "PU");
                }
                else
                {
                    statusStripText.Text = "Введите более 4х символов для поиска по номеру ПУ";
                }
            }
        }


        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (textBox1.Text.Length > 9)
                {
                    SetDataFromDb(textBox1.Text);

                }
                else
                {
                    statusStripText.Text = "Введите более 9ти символов для поиска по лицевому счету";
                }
            }
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ATPWork.AktATPInWork.plomb.Add(new plomba("", "", "", false));
        }
        private void dataGrid_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void Dublirovat(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                plomba Plomb = (plomba)dataGrid.SelectedItem;
                ATPWork.AktATPInWork.plomb.Add(new plomba(Plomb.Type, Plomb.Number, Plomb.Place, false));
            }
        }
        private void RemovPlobmFroDataGrid(object sender, RoutedEventArgs e)
        {
            ATPWork.AktATPInWork.plomb.Remove((plomba)dataGrid.SelectedItem);
        }
        private void AddPlombIncrement(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                plomba Plomb = (plomba)dataGrid.SelectedItem;
                Double numParse;
                if (Double.TryParse(Plomb.Number, out numParse))
                {
                    numParse++;
                    ATPWork.AktATPInWork.plomb.Add(new plomba(Plomb.Type, numParse.ToString(), Plomb.Place, false));
                }
                else
                {
                    ATPWork.AktATPInWork.plomb.Add(new plomba(Plomb.Type, Plomb.Number, Plomb.Place, false));
                }
            }
        }
        private void MenuItemSaveAtpFile(object sender, RoutedEventArgs e)
        {
            SaveAsAtpFile();
        }
        private void SaveAsAtpFile()
        {
            if (!(ATPWork.AllAtpInWorkList.Count == 0))
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "ATP Files (*.atp)|*.atp|All Files (*.*)|*.*";
                save.RestoreDirectory = true;
                save.Title = "Сохранить";
                if (save.ShowDialog() == true)
                {
                    ATPWork.DbWork.SaveATP(ATPWork, save.FileName);
                    ATPWork.CurrentSaveFileAtp = save.FileName;
                    this.Title = "ATPWorker" + ":" + save.FileName;
                }
            }
            else MessageBox.Show("Нехер сохранять");
        }
        private void MenuItemLoadAtpFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "ATP Files (*.atp)|*.atp|All Files (*.*)|*.*";
            op.RestoreDirectory = true;
            op.Title = "Откройте ATP фаил";
            if (op.ShowDialog() == true)
            {
                ATPWork.DbWork.LoadATP(op.FileName, ATPWork);
                ATPWork.CurrentSaveFileAtp = op.FileName;
                int compl_akt = ATPWork.MoveComleteAtp();
                if (compl_akt > 0) statusStripText.Text = compl_akt.ToString() + "шт. актов перемещено в заполненные, в работе осталось " + ATPWork.AllAtpInWorkList.Count.ToString();
                refreshATPTab();
            }
        }
        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var filePath = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (MessageBox.Show("Добавить акты в работу?", "Добавление актов", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly) == MessageBoxResult.Yes)
                {
                    foreach (var item in filePath)
                    {
                        bool exist = File.Exists(item);

                        bool pdf = item.Contains(".pdf");
                        if (exist && pdf)
                        {
                            statusStripText.Text = "Добавлено " + ATPWork.addAtpToWork(item).ToString();
                            EnableAtpTab(true);
                        }
                    }


                }
            }
        }
        private void moonPdfPanel_PdfLoaded(object sender, EventArgs e)
        {

        }
        private void MenuItemMoveAktToComplete(object sender, RoutedEventArgs e)
        {
            statusStripText.Text = ATPWork.MoveComleteAtp().ToString() + "шт. актов перемещено в заполненные, в работе осталось " + ATPWork.AllAtpInWorkList.Count.ToString();
            //dataGridComplete.ItemsSource = ATPWork.CompleteAtpWorkList;
        }
        private int chekForUnMail()
        {
            int i = 0;
            foreach (var item in ATPWork.CompleteAtpWorkList)
            {
                if (item.NumberMail == 0) i++;
            }
            return i;
        }
        private void MenuItemCreateExcel(object sender, RoutedEventArgs e)
        {
            if (chekForUnMail() == 0)
            {
                statusStripText.Text = "Нечего отправлять";
                return;
            }
            string nameMail;
            string pathOfMail;
            nameMail = @"исх.№91-" + numberMailTextBlock.Text + @" от " + dateMail.Text + @"г. Акты ПР ФЛ";
            if (ATPWork.PathOfMailFolder == null)
            {
                if (!ATPWork.selectMailFolder()) return;
            }
            pathOfMail = ATPWork.PathOfMailFolder + nameMail;
            ATPWork.createMailPath(pathOfMail);
            ExcelWorker excel = new ExcelWorker();
            // excel.ToExcel(excel.MakeDataTable(ATPWork.AllAtpInWorkList));
            AllATPObserv listATPForMail = new AllATPObserv();
            foreach (var item in ATPWork.CompleteAtpWorkList)
            {
                if (item.NumberMail == 0)
                {
                    listATPForMail.Add(item);
                    item.NumberMail = Int32.Parse(numberMailTextBlock.Text);
                    item.DateMail = dateMail.Text;
                }
            }
            excel.DataTableToExcel(listATPForMail, pathOfMail + "\\" + @"Реестр.xlsx");
            ATPWork.blindPdf(listATPForMail, pathOfMail);
        }
        private void dateTimePicker1_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            int temp;
            if (Int32.TryParse(textBox.Text, out temp))
            {
                if (e.Key == Key.Up)
                {
                    temp++;
                    textBox.Text = temp.ToString();
                }

                if (e.Key == Key.Down)
                {
                    temp--;
                    textBox.Text = temp.ToString();
                }

            }
        }
        private void dateTimePicker1_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void dateTimePicker1_KeyUp(object sender, KeyEventArgs e)
        {

            if (dateTimePicker1.SelectedDate != null)
            {
                if (e.Key == Key.Up)
                {
                    dateTimePicker1.Text = dateTimePicker1.SelectedDate.Value.AddDays(1).ToString("d");


                }
                if (e.Key == Key.Down)
                {
                    dateTimePicker1.Text = dateTimePicker1.SelectedDate.Value.AddDays(-1).ToString("d");
                }
            }
        }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        private void textBox4_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {


        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ATPWork.DbWork.DromCompliteTable();
            ATPWork.DbWork.InsertCompleteAktAPT(ATPWork.CompleteAtpWorkList);
        }
        private void SaveAtpList(object sender, RoutedEventArgs e)
        {
            if (ATPWork.CurrentSaveFileAtp != null)
            {
                ATPWork.DbWork.SaveATP(ATPWork, ATPWork.CurrentSaveFileAtp);
            }
            else
            {
                SaveAsAtpFile();
            }

        }
        private void MenuItem_RefreshDb(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "xlsx Files (*.)|*.xlsx|All Files (*.*)|*.*";
            op.RestoreDirectory = true;
            op.Title = "Откройте XLSX фаил c результатами поиска SAP";
            if (op.ShowDialog() == true)
            {
                using (FileStream stream = File.Open(op.FileName, FileMode.Open, FileAccess.Read))
                {
                    ExcelWorker excel = new ExcelWorker();
                    ATPWork.DbWork.RefreshSAPFL(excel.makeDataSetForSAPFL(stream));
                }
            }

        }
        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)radioButton.IsChecked)
            {
                rowToHide.Height = new GridLength(101);
            }

        }
        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)radioButton1.IsChecked)
            {
                rowToHide.Height = new GridLength(0);
            }
        }
        private void dataGridComplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        public SAPActive sAP;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {

            if (sAP == null)
            {
                try
                {
                    sAP = new SAPActive("ER2");
                sAP.login();
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                    return;
                }
            }

            progressBar.Minimum = 0;
            progressBar.Maximum = ATPWork.CompleteAtpWorkList.Count;

            int i = 0;

            foreach (var item in ATPWork.CompleteAtpWorkList)
            {

                if ((item.SapNumberAkt == "") && (File.Exists(ATPWork.AktDirektory + item.PathOfPdfFile)))
                {
                    try
                    {
                        sAP.enterAktTehProverki(item, ATPWork.AktDirektory);
                        // item.PathOfPdfFile = item.Number.ToString() + ".pdf";
                        //MessageBox.Show(item.PathOfPdfFile);
                        ATPWork.DbWork.DromCompliteTable();
                        ATPWork.DbWork.InsertCompleteAktAPT(ATPWork.CompleteAtpWorkList);

                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                   

                }
                progressBar.Value = i;
                i++;
            }

            //MessageBox.Show(ex.Message);

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            /*foreach (aktATP item in ATPWork.CompleteAtpWorkList)
            {
                List<Dictionary<string, string>> tempList;
                tempList = ATPWork.DbWork.GetAbonentFromLS(item.NumberLS);
                item.setDataByDb(tempList[0]);
            }*/
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {


            /* foreach (aktATP item in ATPWork.CompleteAtpWorkList)
             {
                  int i = 0;
                  string fileName = "";
                  i=(item.PathOfPdfFile.Split('\\')).Length;
                  fileName = item.PathOfPdfFile.Split('\\')[i - 1];
                  item.PathOfPdfFile = fileName;
                 string p = ATPWork.AktDirektory + item.PathOfPdfFile;
                 if (!File.Exists(p))
                 {
                     MessageBox.Show(item.Title + " " + item.PathOfPdfFile);
                 }
             }*/


        }

        private void DataGridMailedAkt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                aktATP akt = (aktATP)DataGridMailedAkt.SelectedItem;
                moonPdfPanel.OpenFile(akt.PathOfPdfFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button_Click_3(object sender, RoutedEventArgs e)
        {
            /* string temp_pu;
             temp_pu = comboBox4.SelectedItem.ToString();
             foreach (PriborUcheta item in ATPWork.SpisokPU)
             {
                 if (item.Nazvanie == temp_pu) ((aktATP)DataGridMailedAkt.SelectedItem).PuNewType = new PriborUcheta(item.SapNumberPU, item.Nazvanie, item.Poverka, item.Znachnost);
             }

          */
        }

    }
}




