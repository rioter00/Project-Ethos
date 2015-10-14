using System;
using System.Windows.Input;

namespace Ethos.Launcher.Infrastructure
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public RelayCommand(Action execute, Func<bool> canExecute) : this(execute)
        {
            _canExecute = canExecute;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute();

            return _execute != null;
        }

        void ICommand.Execute(object parameter)
        {
            _execute?.Invoke();
        }
    }
}