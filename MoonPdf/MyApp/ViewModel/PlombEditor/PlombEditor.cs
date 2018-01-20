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
        private ObservableCollection<Plomba> _pl;
        public ObservableCollection<Plomba> PlombList
        {
            get { return  this._pl; }
            set { this._pl = value; }
        }
        public PlombEditorVM()
        {

            this.Commands = new Commands(this);
            GetPlacePLFromDb();
            GetTypePLFromDb();

        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
       public void AddPlomb()
        {
            PlombList.Add(new Plomba("", "", "", false));
        }
        public void DeletePlobm()
        {
            PlombList.Remove(SelectedPlomb);
        }
        public void ClonePlomb(bool inc=true)
        {
            int incr;
            incr= inc? (1): (-1);
            Plomba f = SelectedPlomb;
            long res;
            PlombList.Add(new Plomba(f.Type, long.TryParse(f.Number, out res)?(res + incr).ToString():f.Number, f.Place, f.Remove));
        }
        public bool checkSourse()
        {
            return PlombList != null;
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
