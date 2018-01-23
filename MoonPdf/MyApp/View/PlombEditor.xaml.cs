using ATPWork.MyApp.ViewModel.PlombEditorVm;
using MyApp.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    /// Логика взаимодействия для PlombEditor.xaml
    /// </summary>
    /// 
    public partial class PlombEditor : UserControl
    {
        public IEnumerable NewPlombItemsSource
        {
            get { return (IEnumerable)GetValue(NewPlombItemsSourceProperty); }
            set { SetValue(NewPlombItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty NewPlombItemsSourceProperty =
            DependencyProperty.Register("NewPlombItemsSource", typeof(IEnumerable), typeof(PlombEditor), new PropertyMetadata(new PropertyChangedCallback(OnNewPlombItemsSourcePropertyChanged)));
        private static void OnNewPlombItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) // при изменении источника
        {
            var control = sender as PlombEditor;
            if (control != null)
                control.OnNewPlombItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }
        private void OnNewPlombItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) //Отписка подписка на события изменения самой коллекции
        {

            // Remove handler for oldValue.CollectionChanged
            var oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;
                        if (null != oldValueINotifyCollectionChanged)
            {
                oldValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(newValueINotifyCollectionChanged_CollectionChanged);
            }
            // Add handler for newValue.CollectionChanged (if possible)
            var newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(newValueINotifyCollectionChanged_CollectionChanged);
            }

            editVM.NewPlombList = NewPlombItemsSource as ObservableCollection<Plomba>;
        }
        void newValueINotifyCollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) // при изменениях внутри коллекции
        {
            //Do your stuff here.
        }

        public IEnumerable OldPlombItemsSource
        {
            get { return (IEnumerable)GetValue(OldPlombItemsSourceProperty); }
            set { SetValue(OldPlombItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty OldPlombItemsSourceProperty =
            DependencyProperty.Register("OldPlombItemsSource", typeof(IEnumerable), typeof(PlombEditor), new PropertyMetadata(new PropertyChangedCallback(OnOldPlombItemsSourcePropertyChanged)));
        private static void OnOldPlombItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) // при изменении источника
        {
            var control = sender as PlombEditor;
            if (control != null)
                control.OnOldPlombItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.OldValue);
        }
        private void OnOldPlombItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) //Отписка подписка на события изменения самой коллекции
        {

            // Remove handler for oldValue.CollectionChanged
            var oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;
            if (null != oldValueINotifyCollectionChanged)
            {
                oldValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(oldValueINotifyCollectionChanged_CollectionChanged);
            }
            // Add handler for newValue.CollectionChanged (if possible)
            var newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(oldValueINotifyCollectionChanged_CollectionChanged);
            }

            editVM.OldPlombList = OldPlombItemsSource as ObservableCollection<Plomba>;
        }
        void oldValueINotifyCollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) // при изменениях внутри коллекции
        {
            //Do your stuff here.
        }


        private PlombEditorVM editVM;
        public PlombEditor()
        {
            InitializeComponent();
            editVM = (PlombEditorVM)this.Resources["PlombEditorViewModel"];
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OldPlombListBox.SelectedItem = null;
        }
    }
}
