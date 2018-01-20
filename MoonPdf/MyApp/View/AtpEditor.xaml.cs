using ATPWork.MyApp.ViewModel.AtpEditor;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// Логика взаимодействия для AtpEditor.xaml
    /// </summary>
    public partial class AtpEditor : UserControl
    {
       private AtpEditorVM AtpEditVM;

        #region AtpInWork Depency
        public static readonly DependencyProperty AtpInWorkProperty =
            DependencyProperty.Register("AtpInWorkSourse",
                typeof(AktTehProverki),
                typeof(AtpEditor),
                 new PropertyMetadata(new PropertyChangedCallback(OnAtpInWorkSoursePropertyChanged)));

        [Bindable(true)]
        public AktTehProverki AtpInWorkSourse
        {
            get { return (AktTehProverki)this.GetValue(AtpInWorkProperty); }
            set { this.SetValue(AtpInWorkProperty, value); }
        }
        private static void OnAtpInWorkSoursePropertyChanged
   (
       DependencyObject d, // i.e. owner of the dependency property.
       DependencyPropertyChangedEventArgs e // Event arguments.
   )
        {
           
            var control = d as AtpEditor;
            if (control != null)
            {
                control.OnAtpInWorkSourseChanged((AktTehProverki)e.OldValue, (AktTehProverki)e.NewValue);
            }
        }
        private void OnAtpInWorkSourseChanged(AktTehProverki oldValue, AktTehProverki newValue)
        {
            AtpEditVM.AktInWork = newValue;
        }
        #endregion
        public AtpEditor()
        {
            InitializeComponent();
            AtpEditVM = (AtpEditorVM)this.Resources["AtpEditorViewModel"];
        }

    }
}
