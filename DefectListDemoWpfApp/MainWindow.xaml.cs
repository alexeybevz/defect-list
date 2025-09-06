using System.Windows;
using DefectListWpfControl;
using DefectListWpfControl.DefectList.Views;

namespace DefectListDemoWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Hide();
            var window = DefectListIocKernel.Get<DefectListHeaderWindow>();
            window.Show();
        }
    }
}
