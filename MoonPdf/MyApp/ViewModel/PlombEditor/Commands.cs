using ATPWork.MyApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ATPWork.MyApp.ViewModel.PlombEditorVm
{

    public class Commands
    {
        public DelegateCommand DeletePlombCommand { get; private set; }
        public DelegateCommand AddPlombCommand { get; private set; }
        public DelegateCommand IncrementAddPlombComand { get; private set; }

        public Commands(plombDataContext plmbData, PlombEditor wnd)
       {
            Predicate<object> isSelection = f => wnd.checkSecection(); // used for the CanExecute callback
            this.DeletePlombCommand = new DelegateCommand("Удалить пломбу", f => plmbData.DeletePlobm(), isSelection, new KeyGesture(Key.Left));
            this.AddPlombCommand = new DelegateCommand("Добавить пустую пломбу", f => plmbData.AddPlomb(),null, new KeyGesture(Key.Right));
            this.IncrementAddPlombComand = new DelegateCommand("Клонировать пломбу", f => plmbData.ClonePlomb(), isSelection, new KeyGesture(Key.Home));
            RegisterInputBindings(wnd);
        }
        private void RegisterInputBindings(PlombEditor wnd)
        {
            // register inputbindings for all commands (properties)
            foreach (var pi in typeof(Commands).GetProperties())
            {
                var cmd = (BaseCommand)pi.GetValue(this, null);

                if (cmd.InputBinding != null)
                    wnd.InputBindings.Add(cmd.InputBinding);
            }
        }
    }
}

