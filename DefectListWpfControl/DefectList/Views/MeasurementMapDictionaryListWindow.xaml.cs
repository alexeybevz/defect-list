using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using System.Linq;

namespace DefectListWpfControl.DefectList.Views
{
    public partial class MeasurementMapDictionaryListWindow : Window
    {
        public MeasurementMapDictionaryListWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MeasurementMapDictionaryListViewModel;
            if (vm == null) return;

            vm.OpenItemsRequested += OnOpenItemsRequested;
            vm.OpenLookupRequested += OnOpenLookupRequested;
        }

        // Открываем форму строк выбранного справочника
        private void OnOpenItemsRequested(int dictionaryId)
        {
            var vm = DataContext as MeasurementMapDictionaryListViewModel;
            if (vm == null) return;

            var itemsVm = vm.BuildItemsViewModel(dictionaryId);
            var itemsWindow = new MeasurementMapDictionaryItemsWindow
            {
                DataContext = itemsVm,
                Owner = this
            };

            // Пробрасываем запрос открытия формы редактирования строки
            itemsVm.OpenItemEditRequested += item => OnOpenItemEditRequested(item, itemsVm, itemsWindow);

            itemsWindow.ShowDialog();
        }

        // Открываем форму редактирования одной строки справочника
        private void OnOpenItemEditRequested(
            MeasurementMapDictionaryItem originalItem,
            MeasurementMapDictionaryItemsViewModel itemsVm,
            Window owner)
        {
            var editVm = itemsVm.BuildItemEditViewModel(originalItem);
            var editWindow = new MeasurementMapDictionaryItemEditWindow
            {
                DataContext = editVm,
                Owner = owner
            };

            editVm.OpenAlternativesRequested += () => OnOpenAlternativesRequested(editVm, editWindow);
            editVm.SavedCallback = () => editWindow.Close();

            editWindow.ShowDialog();
        }

        // Открываем окно выбора альтернатив метода ремонта
        private void OnOpenAlternativesRequested(
            MeasurementMapDictionaryItemEditViewModel editVm,
            Window owner)
        {
            var altVm = new RepairMethodAlternativesViewModel(
                editVm.RecommendedRepairMethods,
                editVm.SelectedAlternatives);

            var altWindow = new RepairMethodAlternativesWindow
            {
                DataContext = altVm,
                Owner = owner
            };

            altVm.AppliedCallback = () => altWindow.Close();

            altWindow.ShowDialog();
        }

        // Открываем универсальное окно нужного lookup-справочника
        private void OnOpenLookupRequested(LookupStore store)
        {
            var lookupVm = new LookupDictionaryViewModel(store);
            var lookupWindow = new LookupDictionaryWindow
            {
                DataContext = lookupVm,
                Owner = this
            };
            lookupWindow.ShowDialog();
            lookupVm.Dispose();
        }

        private void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MeasurementMapDictionaryListViewModel;
            if (vm?.SelectedDictionary != null)
                vm.OpenItemsCommand.Execute(null);
        }

        protected override void OnClosed(EventArgs e)
        {
            var vm = DataContext as MeasurementMapDictionaryListViewModel;
            if (vm != null)
            {
                vm.OpenItemsRequested -= OnOpenItemsRequested;
                vm.OpenLookupRequested -= OnOpenLookupRequested;
                vm.Dispose();
            }
            base.OnClosed(e);
        }

        private void DataGridDictionaries_OnCopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            var currentColumn = DataGridDictionaries.CurrentCell.Column;
            var itemViewModel = e.Item as MeasurementMapDictionaryViewModel;

            string resolvedText = null;

            if (itemViewModel != null)
            {
                if (ReferenceEquals(currentColumn, dataGridColumnId))
                    resolvedText = itemViewModel.Id.ToString();
                else if (ReferenceEquals(currentColumn, dataGridColumnCode_LSF82))
                    resolvedText = itemViewModel.Code_LSF82.ToString();
                else if (ReferenceEquals(currentColumn, dataGridColumnMeasurementsMapTypeFormName))
                    resolvedText = itemViewModel.MeasurementsMapTypeFormName;
                else if (ReferenceEquals(currentColumn, dataGridColumnVersion))
                    resolvedText = itemViewModel.Version.ToString();
                else if (ReferenceEquals(currentColumn, dataGridColumnIsActive))
                    resolvedText = itemViewModel.IsActive ? "Да" : "Нет";
                else if (ReferenceEquals(currentColumn, dataGridColumnComment))
                    resolvedText = itemViewModel.Comment;
                else if (ReferenceEquals(currentColumn, dataGridColumnCreatedBy))
                    resolvedText = itemViewModel.CreatedBy;
                else if (ReferenceEquals(currentColumn, dataGridColumnCreatedDate))
                    resolvedText = itemViewModel.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
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
