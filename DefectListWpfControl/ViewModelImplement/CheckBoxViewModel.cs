using System;
using System.Windows.Input;

namespace DefectListWpfControl.ViewModelImplement
{
    public class CheckBoxViewModel : ObservableObject
    {
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                NotifyPropertyChanged(nameof(IsChecked));
                IsCheckedChanged?.Invoke();
            }
        }

        public string Text { get; }

        public ICommand ClickCommand { get; }

        public event Action IsCheckedChanged;

        public CheckBoxViewModel(string text, bool isChecked = false)
        {
            IsChecked = isChecked;
            Text = text;
            ClickCommand = new DelegateCommand(p => IsChecked = !IsChecked);
        }
    }
}