using System;
using System.Windows;

namespace DefectListWpfControl.HelpersWindow
{
    /// <summary>
    /// Логика взаимодействия для ChoiceDateWindow.xaml
    /// </summary>
    public partial class ChoiceDateWindow : Window
    {
        public ChoiceDateWindow()
        {
            InitializeComponent();

            DatePicker_Start.SelectedDate = DateTime.Now.Date;
            DatePicker_End.SelectedDate = DateTime.Now.Date;
        }

        public ChoiceDateWindow(DateTime startDate, DateTime endDate)
        {
            InitializeComponent();

            DatePicker_Start.SelectedDate = startDate;
            DatePicker_End.SelectedDate = endDate;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            StartDate = DatePicker_Start.SelectedDate.Value.Date;
            EndDate = DatePicker_End.SelectedDate.Value.Date;

            DialogResult = true;
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}