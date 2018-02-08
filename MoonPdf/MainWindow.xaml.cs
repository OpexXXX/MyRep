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
using MyApp.Model;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using ATPWork.MyApp.ViewModel.AtpEditor;
using ATPWork.MyApp.ViewModel.PlombEditorVm;
using System.Windows.Markup;
using System.Globalization;
using ATPWork.MyApp.ViewModel;
using ATPWork.MyApp.Model.Plan;
using ATPWork.MyApp.Model.VnePlan;

namespace MyApp
{
   

    public partial class MainWindow : Window
    {

        MainAtpVM MainAtpViemModel;


        internal MoonPdfPanel MoonPdfPanel { get { return this.moonPdfPanel; } }
        public MainWindow()
        {
            DataBaseWorker.Initial();
            MainAtpModel.InitMainAtpModel();
            ExcelWorker.InitHeader();
            InitializeComponent();
            MainAtpViemModel = (MainAtpVM)this.Resources["MainAtpVM"];
            MainAtpViemModel.PdfViewer = moonPdfPanel;
            MainAtpViemModel.ListBoxAktInWork = DatagridInWork;
            VnePlanModel.refreshZayavki();
            this.Loaded += MainWindow_Loaded;
            
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MainAtpModel.SaveBeforeCloseApp();
            VnePlanModel.SaveBeforeCloseApp();
            DataBaseWorker.ClosedApp();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double temp;
            if(double.TryParse(BuValue.Text, out temp)) TextBU.Text = PlanWorkModel.CalculationPremiumActBu(temp).ToString();
           
                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter("Test.txt");
                for (int i = 1; i < 50000; i+=500)
                {
                sw.WriteLine(i +"\t"+PlanWorkModel.CalculationPremiumActBu(i) );
            }
                //Write a line of text
              
                //Close the file
                sw.Close();
           
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           
        }
    }
}




