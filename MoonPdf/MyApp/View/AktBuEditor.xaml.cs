using ATPWork.MyApp.Model.AktBuWork;
using ATPWork.MyApp.ViewModel.MainAktBu.BuEditor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ATPWork.MyApp.View
{
    public class VidNarusheniyaConv : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return (VidNarusheniya)value;
        }
    }
    /// <summary>
    /// Логика взаимодействия для AktBuEditor.xaml
    /// </summary>
    public partial class AktBuEditor : UserControl
    {
        #region AktInWork Depency
        public static readonly DependencyProperty BuInWorkProperty =
            DependencyProperty.Register("BuInWorkSourse",
                typeof(AktBu),
                typeof(AktBuEditor),
                 new PropertyMetadata(new PropertyChangedCallback(OnBuInWorkPropertyPropertyChanged)));

        [Bindable(true)]
        public AktBu BuInWorkSourse
        {
            get { return (AktBu)this.GetValue(BuInWorkProperty); }
            set { this.SetValue(BuInWorkProperty, value); }
        }
        private static void OnBuInWorkPropertyPropertyChanged
   (
       DependencyObject d, // i.e. owner of the dependency property.
       DependencyPropertyChangedEventArgs e // Event arguments.
   )
        {

            var control = d as AktBuEditor;
            if (control != null)
            {
                control.OnBuInWorkPropertyChanged((AktBu)e.OldValue, (AktBu)e.NewValue);
            }
        }
        private void OnBuInWorkPropertyChanged(AktBu oldValue, AktBu newValue)
        {
            BuEditVM.AktInWork = newValue;
        }
        #endregion
        private BuEditorViewModel BuEditVM;
        public AktBuEditor()
        {
            InitializeComponent();
            BuEditVM = (BuEditorViewModel)this.Resources["BuEditorViewModel"];
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var item in files)
                {

                    string extension;
                    extension = System.IO.Path.GetExtension(item);
                    if (extension == ".JPG" || extension == ".JPEG"|| extension == ".jpg" || extension == ".jpeg") BuEditVM.AktInWork.PhotoFile.Add(item);
                }

            }

        }

        private void ListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {

               if(BuEditVM.SelectedPhoto!=null) BuEditVM.AktInWork.PhotoFile.Remove(BuEditVM.SelectedPhoto);
            }
        }

        private void GroupBoxAktBU_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string extension;
                extension = System.IO.Path.GetExtension(files[0]);
                if (extension == ".PDF" || extension == ".pdf") BuEditVM.AktInWork.AktBuPdf = files[0];
            }
        }

        private void GroupBoxAktProverki_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string extension;
                extension =System.IO.Path.GetExtension(files[0]);
                if(extension == ".PDF"|| extension==".pdf") BuEditVM.AktInWork.AktPredProverkiPdf = files[0];
            }
        }

        private void GroupBoxIzvechenie_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string extension;
                extension = System.IO.Path.GetExtension(files[0]);
                if (extension == ".PDF" || extension == ".pdf") BuEditVM.AktInWork.IzvesheniePDF = files[0];
            }
        }
    }
}
