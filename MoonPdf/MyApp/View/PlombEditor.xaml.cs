using ATPWork.MyApp.ViewModel.PlombEditorVm;
using MyApp.Model;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace ATPWork.MyApp.View
{
    public class FileName : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.IO.Path.GetFileName(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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

        private void newValueINotifyCollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) // при изменениях внутри коллекции
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

        private void oldValueINotifyCollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) // при изменениях внутри коллекции
        {
            //Do your stuff here.
        }


        private readonly PlombEditorVM editVM;
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
