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
        public DelegateCommand DecrementAddPlombComand { get; private set; }

        public Commands(PlombEditorVM plmbData)
       {
            Predicate<object> isSelection = f => plmbData.checkSecection(); // used for the CanExecute callback
            Predicate<object> isSourse = f => plmbData.checkSourse();
            this.DeletePlombCommand = new DelegateCommand("Удалить пломбу", f => plmbData.DeletePlobm(), isSelection, new KeyGesture(Key.Left));
            this.AddPlombCommand = new DelegateCommand("Добавить пустую пломбу", f => plmbData.AddPlomb(), isSourse, new KeyGesture(Key.Right));
            this.IncrementAddPlombComand = new DelegateCommand("Клонировать пломбу", f => plmbData.ClonePlomb(), isSelection, new KeyGesture(Key.Home));
            this.DecrementAddPlombComand = new DelegateCommand("Клонировать пломбу", f => plmbData.ClonePlomb(false), isSelection, new KeyGesture(Key.Up));
        }
        
    }
}

