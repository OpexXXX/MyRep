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
    public class PlombEditorVM :ViewModelBase
    {
        #region Команды
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
        public bool checkSourse()
        {
            return PlombList != null;
        }
        public bool checkSecection()
        {
            return SelectedPlomb != null;
        }
        #endregion
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
        private static ObservableCollection<string> _typePL = new ObservableCollection<string>();
        public static ObservableCollection<string> typePL
        {
            get { return _typePL; }
            set { _typePL = value; }
        }
        private static ObservableCollection<string> _placePL = new ObservableCollection<string>();
        public static ObservableCollection<string> placePL
        {
            get { return _placePL; }
            set { _placePL = value; }
        }
        private ObservableCollection<Plomba> _pl = new ObservableCollection<Plomba>();
        public ObservableCollection<Plomba> PlombList
        {
            get { return this._pl; }
            set { this._pl = value; }
        }
        public PlombEditorVM()
        {
            this.Commands = new Commands(this);
            GetListForComboBoxFromDb();
            MainAtpModel.ComboRefresh += GetListForComboBoxFromDb;
        }
        public void AddPlomb()
        {
            PlombList.Add(new Plomba("", "", "", false));
        }
        public void DeletePlobm()
        {
            PlombList.Remove(SelectedPlomb);
        }
        public void ClonePlomb(bool inc = true)
        {
            int incr;
            incr = inc ? (1) : (-1);
            Plomba f = SelectedPlomb;
            long res;
            PlombList.Add(new Plomba(f.Type, long.TryParse(f.Number, out res) ? (res + incr).ToString() : f.Number, f.Place, f.Remove));
        }
        public static void GetListForComboBoxFromDb()
        {
            placePL = new ObservableCollection<string>(MainAtpModel.PlacePlomb);
            typePL = new ObservableCollection<string>(MainAtpModel.PlacePlomb);
        }
    }
}
