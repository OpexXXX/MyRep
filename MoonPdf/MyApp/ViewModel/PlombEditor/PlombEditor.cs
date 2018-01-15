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
    public class plombDataContext : INotifyPropertyChanged
    {
        private PlombEditor plombEd;
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
        private List<string> _typePL;
        public List<string> typePL
        {
            get { return this._typePL; }
            set { this._typePL = value; }
        }
        private List<string> _placePL;
        public List<string> placePL
        {
            get { return this._placePL; }
            set { this._placePL = value; }
        }
        private ObservableCollection<Plomba> _pl;
        public ObservableCollection<Plomba> pl
        {
            get { return this._pl; }
            set { this._pl = value; }
        }
        public plombDataContext(PlombEditor plombEdit)
        {
            this.Commands = new Commands(this, plombEdit);
            this.plombEd = plombEdit;
           
            typePL = new List<string>();
            placePL = new List<string>();
            typePL.Add("2400_4");
            typePL.Add("2400_5");
            typePL.Add("2400_6");
            placePL.Add("ВКА");
            placePL.Add("Щит Учета");
            placePL.Add("Кл. кр. ПУ");
            placePL.Add("Корпус ПУ");
            pl = new ObservableCollection<Plomba>();

            pl.Add(new Plomba("2400_5", 240501801, "ВКА", false));
            pl.Add(new Plomba("2400_5", 24021, "Щит Учета", false));

        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

       public void AddPlomb()
        {
            pl.Add(new Plomba("2400_4", 0, "ВКА", false));
        }
        public void DeletePlobm()
        {
            pl.Remove(SelectedPlomb);
        }
        public void ClonePlomb()
        {
            Plomba f = SelectedPlomb;
            pl.Add(new Plomba(f.Type, f.Number, f.Place, f.Remove));
        }
    }
}
