using ATPWork.MyApp.ViewModel.AtpEditor;
using MyApp.Model;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ATPWork.MyApp.View
{
    /// <summary>
    /// Логика взаимодействия для AtpEditor.xaml
    /// </summary>
    public partial class AtpEditor : UserControl
    {
        private readonly AtpEditorVM AtpEditVM;

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
