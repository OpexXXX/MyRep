using System.Windows;

namespace ATPWork.MyApp.View
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private ATPWork.Properties.Settings _sett = Properties.Settings.Default;
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

            }
        }

        private void SelectDirAktTehMail(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) Sett.DirAktTehMail = dialog.SelectedPath;

            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Sett.Save();

            this.Close();
        }
    }
}
