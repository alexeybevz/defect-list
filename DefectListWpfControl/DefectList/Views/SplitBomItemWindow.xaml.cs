using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DefectListWpfControl.DefectList.Views
{
    /// <summary>
    /// Interaction logic for SplitBomItemWindow.xaml
    /// </summary>
    public partial class SplitBomItemWindow : Window
    {
        public SplitBomItemWindow()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            string newText = ((TextBox)sender).Text;
            newText = newText.Insert(((TextBox)sender).CaretIndex, e.Text);
            newText = newText.Replace(" ", "s");

            var regex = new Regex(@"^([0-9]{1,4}|[0-9]{1,4}[\.]?|[0-9]{1,4}[\.][0-9]{1,3})$");
            e.Handled = !regex.IsMatch(newText);
        }
    }
}
