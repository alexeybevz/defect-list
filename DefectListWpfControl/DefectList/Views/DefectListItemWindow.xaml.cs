using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
    }
}
