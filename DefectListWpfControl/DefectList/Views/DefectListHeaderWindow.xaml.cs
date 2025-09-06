using System.Linq;
using System.Windows;

namespace DefectListWpfControl.DefectList.Views
{
    public partial class DefectListHeaderWindow : Window
    {
        public DefectListHeaderWindow()
        {
            InitializeComponent();
        }

        private void DefectListHeadersDataGrid_CopyingRowClipboardContent(object sender, System.Windows.Controls.DataGridRowClipboardEventArgs e)
        {
            var visibleColumns = DefectListHeadersDataGrid.Columns.Where(x => x.Visibility == Visibility.Visible).ToList();
            var index = visibleColumns.IndexOf(DefectListHeadersDataGrid.CurrentCell.Column);

            var currentCell = e.ClipboardRowContent[index];
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(currentCell);
        }
    }
}
