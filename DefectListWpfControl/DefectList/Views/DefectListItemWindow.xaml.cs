using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DefectListDomain.Models;

namespace DefectListWpfControl.DefectList.Views
{
    /// <summary>
    /// Interaction logic for DefectListItemWindow.xaml
    /// </summary>
    public partial class DefectListItemWindow : Window
    {
        public DefectListItemWindow()
        {
            InitializeComponent();
        }

        private void dataGridBomItems_CopyingRowClipboardContent(object sender, System.Windows.Controls.DataGridRowClipboardEventArgs e)
        {
            var visibleColumns = dataGridBomItems.Columns.Where(x => x.Visibility == Visibility.Visible).ToList();
            var index = visibleColumns.IndexOf(dataGridBomItems.CurrentCell.Column);

            var currentCell = e.ClipboardRowContent[index];
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(currentCell);
        }

        private void DataGridBomItemLog_OnCopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            var currentCell = e.ClipboardRowContent[dataGridBomItemLog.CurrentCell.Column.DisplayIndex];
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(currentCell);
        }

        private void DataGridMapBomItemToRouteCharts_OnCopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            var currentCell = e.ClipboardRowContent[dataGridMapBomItemToRouteCharts.CurrentCell.Column.DisplayIndex];
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(currentCell);
        }

        private void DataGridMeasurementMapItemLogs_OnCopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            // Для DataGridTemplateColumn (PossibleDefectName, NominalValueName, ActualValue, RequirementPostRepairName) DataGrid
            // не может сам определить, что скопировать в буфер — внутри шаблона
            // может быть что угодно (TextBlock с Inlines, ComboBox и т.д.),
            // поэтому ClipboardRowContent для этих колонок приходит пустым
            // или с визуальным мусором (например, остатком LineBreak из Inlines).
            // Подставляем нужный текст явно, опираясь на ViewModel строки,
            // а не на визуальное содержимое ячейки.

            var currentColumn = dataGridMeasurementMapItemLogs.CurrentCell.Column;
            var itemViewModel = e.Item as MeasurementMapItemLog;

            string resolvedText = null;

            if (itemViewModel != null)
            {
                if (ReferenceEquals(currentColumn, dataGridColumnPossibleDefectName))
                    resolvedText = itemViewModel.PossibleDefectName;
                else if (ReferenceEquals(currentColumn, dataGridColumnNominalValueName))
                    resolvedText = itemViewModel.NominalValueName;
                else if (ReferenceEquals(currentColumn, dataGridColumnActualValue))
                    resolvedText = itemViewModel.ActualValue;
                else if (ReferenceEquals(currentColumn, dataGridColumnActualRecommendedRepairMethodName))
                    resolvedText = itemViewModel.ActualRecommendedRepairMethodName;
                else if (ReferenceEquals(currentColumn, dataGridColumnRequirementPostRepairName))
                    resolvedText = itemViewModel.RequirementPostRepairName;
                else if (ReferenceEquals(currentColumn, dataGridColumnCreateDate))
                    resolvedText = itemViewModel.CreateDate.ToString("dd.MM.yyyy HH:ss:mm");
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
