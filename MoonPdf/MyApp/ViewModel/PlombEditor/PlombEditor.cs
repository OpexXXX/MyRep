using ATPWork.MyApp.View;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ATPWork.MyApp.ViewModel.PlombEditorVm
{
    public class PlombEditorVM : INotifyPropertyChanged
    {
        private Commands _commands;
        public Commands Commands
        {
            get { return _commands; }
            set
            {
                _commands = value;
                OnPropertyChanged("Commands");
            }
        }
        private Plomba _selectedPlomb;
        public Plomba SelectedPlomb
        {
            get { return _selectedPlomb; }
            set
            {
                _selectedPlomb = value;
                OnPropertyChanged("SelectedPlomb");
            }
        }

        private static List<string> _typePL;
        public static List<string> typePL
        {
            get { return _typePL; }
            set { _typePL = value; }
        }
        private static List<string> _placePL;
        public static List<string> placePL
        {
            get { return _placePL; }
            set { _placePL = value; }
        }
       


        public ObservableCollection<Plomba> _pl;
        public ObservableCollection<Plomba> PlombList
        {
            get { return  this._pl; }

        }
        public PlombEditorVM()
        {
            this.Commands = new Commands(this);
            GetPlacePLFromDb();
            GetTypePLFromDb();
            _pl = new ObservableCollection<Plomba>();

            PlombList.Add(new Plomba("2400_5", 240501801, "ВКА", false));
            PlombList.Add(new Plomba("2400_5", 24021, "Щит Учета", false));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

       public void AddPlomb()
        {
            PlombList.Add(new Plomba("2400_4", 0, "ВКА", false));
        }
        public void DeletePlobm()
        {
            PlombList.Remove(SelectedPlomb);
        }
        public void ClonePlomb()
        {
            Plomba f = SelectedPlomb;
            PlombList.Add(new Plomba(f.Type, f.Number, f.Place, f.Remove));
        }
        public bool checkSecection()
        {
            return SelectedPlomb != null;
        }
        public static void GetPlacePLFromDb()
        {
            placePL = new List<string>();
            placePL.Add("ВКА");
            placePL.Add("Щит Учета");
            placePL.Add("Кл. кр. ПУ");
            placePL.Add("Корпус ПУ");
        }
        public static void GetTypePLFromDb()
        {
            typePL = new List<string>();
            typePL.Add("2400_4");
            typePL.Add("2400_5");
            typePL.Add("2400_6");
        }
    }
}
