using System;
using System.Threading;
using System.Windows.Input;

namespace DefectListWpfControl.ViewModelImplement
{
    public sealed class CancelAsyncCommand : ICommand, IDisposable
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _commandExecuting;

        public CancellationToken Token => _cts.Token;

        public void NotifyCommandStarting()
        {
            _commandExecuting = true;
            RaiseCanExecuteChanged();

            if (!_cts.IsCancellationRequested)
                return;

            _cts = new CancellationTokenSource();

            RaiseCanExecuteChanged();
        }

        public void NotifyCommandFinished()
        {
            _commandExecuting = false;
            RaiseCanExecuteChanged();
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _commandExecuting && !_cts.IsCancellationRequested;
        }

        public void Execute(object parameter)
        {
            _cts.Cancel();
            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            _cts.Dispose();
        }
    }
}