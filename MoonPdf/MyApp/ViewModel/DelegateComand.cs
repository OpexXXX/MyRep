using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ATPWork.MyApp.ViewModel
{
    public class DelegateCommand : BaseCommand
    {
        private Action<object> executeAction;
        private Predicate<object> canExecuteFunc;

        public DelegateCommand(string name, Action<object> executeAction, Predicate<object> canExecuteFunc, InputGesture inputGesture)
            : base(name, inputGesture)
        {
            this.executeAction = executeAction;
            this.canExecuteFunc = canExecuteFunc;
        }

        public override bool CanExecute(object parameter)
        {
            return this.canExecuteFunc != null ? this.canExecuteFunc(parameter) : true;
        }

        public override void Execute(object parameter)
        {
            this.executeAction(parameter);
        }
    }
}
