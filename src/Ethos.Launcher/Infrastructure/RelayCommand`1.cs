using System;
using System.Windows.Input;

namespace Ethos.Launcher.Infrastructure
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<T> execute)
        {
            _execute = execute;
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute) : this(execute)
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
                return _canExecute((T) parameter);

            return _execute != null;
        }

        void ICommand.Execute(object parameter)
        {
            _execute?.Invoke((T) parameter);
        }
    }
}