using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kataana.Models
{
    public class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue) == false)
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                return (true);
            }
            return (false);
        }

        public class DelegateCommand : ICommand
        {
            private readonly Action<object> _executeAction;
            private readonly Func<object, bool> _canExecuteAction;

            public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteAction)
            {
                _executeAction = executeAction;
                _canExecuteAction = canExecuteAction;
            }

            public DelegateCommand(Action<object> executeAction)
            {
                _executeAction = executeAction;
            }

            public void Execute(object parameter) => _executeAction(parameter);

            public bool CanExecute(object parameter) => _canExecuteAction?.Invoke(parameter) ?? true;

            public event EventHandler CanExecuteChanged;

            public void InvokeCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
