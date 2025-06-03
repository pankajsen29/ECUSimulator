using System.Windows.Input;

namespace WPFComSetupViewLib.Command
{
    /// <summary>
    /// This is a command whose sole purpose is to relay its functionality to other objects by invoking delegates.
    /// 
    /// The RelayCommand allows to inject the command's logic via delegates passed into its constructor. 
    /// This approach allows to write a concise command implementation in ViewModel classes. 
    /// RelayCommand is a simplified variation of the DelegateCommand.
    /// 
    /// Action: is a delegate (pointer) to a method, that takes zero, one or more input parameters, but does not return anything.
    /// Predicate: is a delegate (pointer) to a method, takes exactly one generic input parameter and returns bool).
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute) : this(execute, null)
        {
        }
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute)
        {
            if (null == execute)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// The default return value for the CanExecute method is 'true'.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
