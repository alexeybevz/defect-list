using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapCommands
{
    public class OpenFormSketchViewCommand : AsyncCommandBase
    {
        private readonly MeasurementMapStore _measurementMapStore;
        private readonly SelectedBomItemStore _selectedBomItemStore;

        public OpenFormSketchViewCommand(
            MeasurementMapStore measurementMapStore,
            SelectedBomItemStore selectedBomItemStore)
        {
            _measurementMapStore = measurementMapStore;
            _selectedBomItemStore = selectedBomItemStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var sketchFilePaths = _measurementMapStore.MeasurementMap?.SketchFilePaths;
            var title = $"Эскиз для {_selectedBomItemStore.SelectedBomItem.Detal}";

            if (!sketchFilePaths.Any())
            {
                MessageBox.Show("Эскиз не привязан к карте измерения.");
                return;
            }

            foreach (var sketchFilePath in sketchFilePaths)
            {
                var w = new SketchViewerWindow(sketchFilePath, title);
                w.Show();
            }
        }
    }
}