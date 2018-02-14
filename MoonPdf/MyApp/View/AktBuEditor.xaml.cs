using ATPWork.MyApp.Model.AktBuWork;
using ATPWork.MyApp.ViewModel.MainAktBu.BuEditor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ATPWork.MyApp.View
{
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
                    BuEditVM.AktInWork.PhotoFile.Add(item);
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

        
    }
}
