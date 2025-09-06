using DefectListWpfControl.DefectList.ViewModels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DefectListWpfControl.DefectList.Components
{
    /// <summary>
    /// Interaction logic for BomItemReadWriteFieldsViewModel.xaml
    /// </summary>
    public partial class BomItemReadWriteFieldsViewModel : UserControl
    {
        public BomItemReadWriteFieldsViewModel()
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

        private void QtyRestoreTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }

        private void QtyReplaceTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }

        private void QtyRestoreTextBox_OnPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste ||
                e.Command == ApplicationCommands.Cut)
                e.Handled = true;
        }


        private void QtyReplaceTextBox_OnPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste ||
                e.Command == ApplicationCommands.Cut)
                e.Handled = true;
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (CheckBoxClosePosibleDefectsBehaviour.IsChecked ?? false)
                ComboBoxPosibleDefect.IsDropDownOpen = false;
        }

        private void PosibleDefect_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = ((TextBlock)sender).DataContext as DefectToDecisionMapCheckBoxViewModel;
            if (selectedItem == null)
                return;
            selectedItem.IsSelected = !selectedItem.IsSelected;
        }
    }
}
