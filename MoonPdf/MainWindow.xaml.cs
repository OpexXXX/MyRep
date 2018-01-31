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
                

            this.Loaded += MainWindow_Loaded;
            
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MainAtpModel.SaveBeforeCloseApp();
            DataBaseWorker.ClosedApp();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*  ExcelWorker.CreatePdfReestr();*/
            PlanWorkModel plan = new PlanWorkModel(new DateTime(2018, 2, 2));
            var gg = ExcelWorker.MakeDataTableForPlan(plan.AbonentList);
            ExcelWorker.CreatePdfReestr(gg);
            int rf = 6; 
        }
    }
}




