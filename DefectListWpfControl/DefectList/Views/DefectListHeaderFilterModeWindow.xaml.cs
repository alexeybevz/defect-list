using System.ComponentModel;
using System.Windows;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Views
{
    /// <summary>
    /// Interaction logic for DefectListHeaderFilterModeWindow.xaml
    /// </summary>
    public partial class DefectListHeaderFilterModeWindow : Window
    {
        private bool _isClose;

        public DefectListHeaderFilterModeWindow(DefectListHeaderFilterModeViewModel defectListHeaderFilterModeViewModel)
        {
            InitializeComponent();

            DataContext = defectListHeaderFilterModeViewModel;
            _isClose = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_OnClosing(object sender, CancelEventArgs e)
        {
            if (_isClose)
                return;

            e.Cancel = true; // Отменяем закрытие формы
            Hide();          // Скрываем окно
        }

        public void ForceClose()
        {
            _isClose = true;
            Close();
        }
    }
}
