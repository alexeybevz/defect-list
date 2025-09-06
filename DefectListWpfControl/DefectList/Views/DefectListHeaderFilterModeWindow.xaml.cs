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
        public DefectListHeaderFilterModeWindow(DefectListHeaderFilterModeViewModel defectListHeaderFilterModeViewModel)
        {
            InitializeComponent();

            DataContext = defectListHeaderFilterModeViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true; // Отменяем закрытие формы
            Hide();          // Скрываем окно
        }
    }
}
