using MyApp;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ATPWork.MyApp.View
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private ATPWork.Properties.Settings _sett = App.GlobalSett;
        public ATPWork.Properties.Settings Sett
        {
            get { return _sett; }
            set { _sett = value; }
        }

        public Settings()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Sett.Save();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SelectDirAktPDF(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) Sett.DirAktTehPDF = dialog.SelectedPath;
               BindingExpression be = DirAktTehPDF.GetBindingExpression(TextBox.TextProperty);
                be.UpdateTarget();
            }
        }

        private void SelectDirAktTehMail(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) Sett.DirAktTehMail = dialog.SelectedPath;
                BindingExpression be = DirAktTehPDF.GetBindingExpression(TextBox.TextProperty);
                be.UpdateSource();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Sett.Save();
            
            this.Close();
        }
    }
}
