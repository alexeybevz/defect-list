using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DefectListDomain.Models;

namespace DefectListWpfControl.DefectList.Views
{
    /// <summary>
    /// Interaction logic for MeasurementMapReadOnlyWindow.xaml
    /// </summary>
    public partial class MeasurementMapReadOnlyWindow : Window
    {
        public MeasurementMapReadOnlyWindow()
        {
            InitializeComponent();
        }

        private void DataGridMeasurementMapItems_OnCopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            var currentColumn = DataGridMeasurementMapItems.CurrentCell.Column;
            var itemViewModel = e.Item as MeasurementMapItemDisplayModel;

            string resolvedText = null;

            if (itemViewModel != null)
            {
                if (ReferenceEquals(currentColumn, dataGridColumnPossibleDefectName))
                    resolvedText = itemViewModel.PossibleDefectName;
                else if (ReferenceEquals(currentColumn, dataGridColumnNominalValueName))
                    resolvedText = itemViewModel.NominalValueName;
                else if (ReferenceEquals(currentColumn, dataGridColumnActualValue))
                    resolvedText = itemViewModel.ActualValue;
                else if (ReferenceEquals(currentColumn, dataGridColumnActualRecommendedRepairMethod))
                    resolvedText = itemViewModel.ActualRecommendedRepairMethodName;
                else if (ReferenceEquals(currentColumn, dataGridColumnRequirementPostRepairName))
                    resolvedText = itemViewModel.RequirementPostRepairName;
                else if (ReferenceEquals(currentColumn, dataGridColumnMarkOfWorkCompletion))
                    resolvedText = itemViewModel.MarkOfWorkCompletion;
                else
                {
                    // Обычные текстовые колонки
                    // Для них запись в e.ClipboardRowContent есть, ищем по ссылке на колонку,
                    // а не по индексу, т.к. индекс совпадает с DisplayIndex только
                    // если все колонки бы попадали в список, а это не так
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
