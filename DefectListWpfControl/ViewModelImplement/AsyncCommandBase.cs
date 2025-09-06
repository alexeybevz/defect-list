using System.Threading.Tasks;

namespace DefectListWpfControl.ViewModelImplement
{
    public abstract class AsyncCommandBase : CommandBase
    {
        private bool _isExecuting;

        public bool IsExecuting
        {
            get { return _isExecuting; }
            set
            {
                _isExecuting = value;
                OnCanExecuteChanged();
            }
        }

        public override bool CanExecute(object parameter = null)
        {
            return !IsExecuting && base.CanExecute(parameter);
        }

        public override async void Execute(object parameter = null)
        {
            IsExecuting = true;

            try
            {
                await ExecuteAsync(parameter);
            }
            finally
            {
                IsExecuting = false;
            }
        }

        public abstract Task ExecuteAsync(object parameter = null);
    }
}
