using System.Windows;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Views
{
    /// <summary>
    /// Interaction logic for RepairMethodAlternativesWindow.xaml
    /// </summary>
    public partial class RepairMethodAlternativesWindow : Window
    {
        public RepairMethodAlternativesWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as RepairMethodAlternativesViewModel;
            if (vm == null) return;

            // После применения — закрываем окно
            vm.AppliedCallback = () => Close();
        }
    }
}
