using System;
using System.Windows.Input;

namespace DefectListWpfControl.ViewModelImplement
{
    public abstract class CommandBase : ICommand, IDisposable
    {
        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter = null)
        {
            return true;
        }

        public abstract void Execute(object parameter = null);

        public virtual void OnCanExecuteChanged(object parameter = null)
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public void Dispose()
        {
            ExecuteDispose();
        }

        protected virtual void ExecuteDispose() { }
    }
}
