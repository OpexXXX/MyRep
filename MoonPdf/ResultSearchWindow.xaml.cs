using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MoonPdf
{
    
    public class ListBoxView :INotifyPropertyChanged, IEquatable<ListBoxView>
    {
        private Dictionary<String, String> val;
        public Dictionary<String, String> Val
        {
            get { return this.val; }
            set
            {
                this.val = value; this.OnPropertyChanged("Val");
            }
        }


        private string puOldNumber;
        public string PuOldNumber
        {
            get { return this.puOldNumber; }
            set
            {
                this.puOldNumber = value; this.OnPropertyChanged("PuOldNumber");
            }
        }

        private string puOldType;
        public string PuOldType
        {
            get { return this.puOldType; }
            set
            {
                this.puOldType = value; this.OnPropertyChanged("PuOldType");
            }
        }

        private string adress;
        public string Adress
        {
            get { return this.adress; }
            set
            {
                //TypeOfWorkBool = (value == "Допуск");
                this.adress = value; this.OnPropertyChanged("Adress");
            }
        }

        private string fIO;
        public string FIO
        {
            get { return this.fIO; }
            set { this.fIO = value; this.OnPropertyChanged("FIO"); }
        }

        private string numberLS;
        public string NumberLS
        {
            get { return this.numberLS; }
            set { this.numberLS = value; this.OnPropertyChanged("NumberLS"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public bool Equals(ListBoxView item) //Сравнение
        {
            return item.numberLS == this.numberLS;
        }
        protected void OnPropertyChanged(string info) // На изменение полей
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        public ListBoxView(Dictionary<String,String> Val)
        {
            string tmpadress = "";
            tmpadress = Val["City"] + ", " + Val["Street"] + ", д. " + Val["House"];
            if (Val.ContainsKey("Korpus")) tmpadress += Val["Korpus"];
            if (Val.ContainsKey("Kv")) tmpadress += ", кв." + Val["Kv"];
            Adress = tmpadress;
            this.Adress = tmpadress;
            this.FIO = Val["FIO"];
            this.NumberLS = Val["LsNumber"];
            this.PuOldNumber = Val["PuNumber"];
            this.PuOldType = Val["PuType"];
            this.Val = Val;
            

        }
    }
    /// <summary>
    /// Логика взаимодействия для ResultSearchWindow.xaml
    /// </summary>
    public partial class ResultSearchWindow : Window
    {
        private Dictionary<String, String> selectVal;
        public Dictionary<String, String> SelectVal
        {
            get { return this.selectVal; }
            set
            {
                this.selectVal = value; 
            }
        }
        public ResultSearchWindow()
        {
            InitializeComponent();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxView tamp = (ListBoxView)dataGrid.SelectedItem;
            SelectVal = tamp.Val;
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxView tamp = (ListBoxView)dataGrid.SelectedItem;
            SelectVal = tamp.Val;
            this.DialogResult = true;
            this.Close();
        }
    }
}
