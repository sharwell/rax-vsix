namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System;
    using System.Windows.Input;

    internal class RelayCommand : ICommand
    {
        private static readonly Func<bool> TruePredicate = () => true;

        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute)
            : this(execute, TruePredicate)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            if (canExecute == null)
                throw new ArgumentNullException("canExecute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            var t = CanExecuteChanged;
            if (t != null)
                t(this, e);
        }
    }
}