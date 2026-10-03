using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Views
{
    /// <summary>
    /// Interaction logic for MeasurementMapDictionaryItemsWindow.xaml
    /// </summary>
    public partial class MeasurementMapDictionaryItemsWindow : Window
    {
        public MeasurementMapDictionaryItemsWindow()
        {
            InitializeComponent();
        }

        private void DataGridItems_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MeasurementMapDictionaryItemsViewModel;
            if (vm?.SelectedItem != null)
                vm.EditItemCommand?.Execute();
        }

        private void DataGridItems_OnCopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            var currentColumn = DataGridItems.CurrentCell.Column;
            var itemViewModel = e.Item as MeasurementMapDictionaryItemViewModel;

            string resolvedText = null;

            if (itemViewModel != null)
            {
                if (ReferenceEquals(currentColumn, dataGridColumnSortOrder))
                    resolvedText = itemViewModel.SortOrder.ToString();
                else if (ReferenceEquals(currentColumn, dataGridColumnPossibleDefectName))
                    resolvedText = itemViewModel.PossibleDefectName;
                else if (ReferenceEquals(currentColumn, dataGridColumnNominalValueName))
                    resolvedText = itemViewModel.NominalValueName;
                else if (ReferenceEquals(currentColumn, dataGridColumnAlternateNominalValueName))
                    resolvedText = itemViewModel.AlternateNominalValueName;
                else if (ReferenceEquals(currentColumn, dataGridColumnRecommendedRepairMethod))
                    resolvedText = itemViewModel.RecommendedRepairMethodName;
                else if (ReferenceEquals(currentColumn, dataGridColumnAlternateRecommendedRepairMethod))
                    resolvedText = itemViewModel.RepairMethodAlternativesDisplay;
                else if (ReferenceEquals(currentColumn, dataGridColumnRequirementPostRepairName))
                    resolvedText = itemViewModel.RequirementPostRepairName;
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
