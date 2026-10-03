using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Views
{
    /// <summary>
    /// Interaction logic for LookupDictionaryWindow.xaml
    /// </summary>
    public partial class LookupDictionaryWindow : Window
    {
        public LookupDictionaryWindow()
        {
            InitializeComponent();
        }

        private void DataGridItems_OnCopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            var currentColumn = DataGridItems.CurrentCell.Column;
            var itemViewModel = e.Item as LookupItemViewModel;

            string resolvedText = null;

            if (itemViewModel != null)
            {
                if (ReferenceEquals(currentColumn, dataGridColumnId))
                    resolvedText = itemViewModel.Id.ToString();
                else if (ReferenceEquals(currentColumn, dataGridColumnName))
                    resolvedText = itemViewModel.Name;
                else if (ReferenceEquals(currentColumn, dataGridColumnIsUsed))
                    resolvedText = itemViewModel.IsUsed ? "Да" : "Нет";
                else if (ReferenceEquals(currentColumn, dataGridColumnIsActive))
                    resolvedText = itemViewModel.IsActive ? "Да" : "Нет";
                else
                {
                    var existing = e.ClipboardRowContent
                        .FirstOrDefault(c => ReferenceEquals(c.Column, currentColumn));
                    resolvedText = existing.Content as string;
                }
            }

            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(new DataGridClipboardCellContent(
                e.Item,
                currentColumn,
                resolvedText));
        }
    }
}
