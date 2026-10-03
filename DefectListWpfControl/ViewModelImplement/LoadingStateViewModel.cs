using System;
using System.Threading.Tasks;

namespace DefectListWpfControl.ViewModelImplement
{
    // Переиспользуемое состояние "идет асинхронная загрузка данных + ошибка" .
    // Выделено отдельно, чтобы разные ViewModel (в т.ч. открывающие
    // одни и те же данные в разных окнах – редактирование / только чтение)
    // могли шарить одну и ту же логику индикации загрузки без дублирования
    // и без завязки Command на конкретный класс ViewModel
    public class LoadingStateViewModel : ObservableObject
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged(nameof(IsLoading));
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged(nameof(ErrorMessage));
                NotifyPropertyChanged(nameof(HasErrorMessage));
            }
        }

        public bool HasErrorMessage => !string.IsNullOrEmpty(_errorMessage);

        // Обёртка: установить IsLoading, поймать ошибку в ErrorMessage
        public async Task ExecuteWithLoadingAsync(Func<Task> action)
        {
            IsLoading = true;
            ErrorMessage = null;
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}