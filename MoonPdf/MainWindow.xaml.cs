using System.IO;
using System.Windows;
using MoonPdfLib;
using System.ComponentModel;
using MyApp.Model;
using ATPWork.MyApp.ViewModel;
using ATPWork.MyApp.Model.Plan;
using ATPWork.MyApp.Model.VnePlan;
using ATPWork.MyApp.View;

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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var g = new Settings();
            g.Owner = this;
            g.ShowDialog();
        }

       
    }
}




