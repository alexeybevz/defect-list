using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Views
{
    public partial class MeasurementMapWindow : Window
    {
        public MeasurementMapWindow()
        {
            InitializeComponent();
        }

        private void DataGridMeasurementMapItems_OnCopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            var currentColumn = DataGridMeasurementMapItems.CurrentCell.Column;
            var itemViewModel = e.Item as MeasurementMapItemViewModel;

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

        private void DataGridMeasurementMapItemLogs_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (!ReferenceEquals(e.Column, dataGridColumnActualRecommendedRepairMethod))
                return;

            var itemViewModel = e.Row.Item as MeasurementMapItemViewModel;
            if (itemViewModel != null && !itemViewModel.IsRepairMethodEditable)
                e.Cancel = true;
        }

        private void DataGridCell_GotFocus(object sender, RoutedEventArgs e)
        {
            var cell = sender as DataGridCell;
            if (cell == null || cell.IsEditing || cell.IsReadOnly)
                return;

            var comboBox = FindVisualChild<ComboBox>(DataGridMeasurementMapItems, cb => cb.IsDropDownOpen);
            if (comboBox != null)
                comboBox.IsDropDownOpen = false;

            DataGridMeasurementMapItems.BeginEdit();
        }

        private void DataGridMeasurementMapItems_OnPreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            var editingElement = e.EditingElement;

            editingElement.Dispatcher.BeginInvoke(new Action(() =>
            {
                var comboBox = editingElement as ComboBox ?? FindVisualChild<ComboBox>(editingElement, cb => cb.Visibility == Visibility.Visible);

                if (comboBox != null)
                {
                    comboBox.IsDropDownOpen = true;
                    return;
                }

                var textBox = editingElement as TextBox ?? FindVisualChild<TextBox>(editingElement, tb => tb.Visibility == Visibility.Visible);
                if (textBox != null)
                {
                    textBox.Focus();
                    Keyboard.Focus(textBox);
                    textBox.CaretIndex = textBox.Text.Length;
                }

            }), DispatcherPriority.Loaded);
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var typed = child as T;
                if (typed != null)
                    return typed;
                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private static T FindVisualChild<T>(DependencyObject parent, Func<T, bool> predicate) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var typed = child as T;
                if (typed != null && predicate(typed))
                    return typed;
                var descendant = FindVisualChild<T>(child, predicate);
                if (descendant != null)
                    return descendant;
            }
            return null;
        }

        private void DataGridMeasurementMapItems_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //var cell = sender as DataGridCell;

            //if (cell == null || cell.IsEditing || cell.IsReadOnly)
            //    return;

            //var openCombobox = FindVisualChild<ComboBox>(DataGridMeasurementMapItems, cb => cb.IsDropDownOpen);
            //if (openCombobox != null)
            //{
            //    openCombobox.IsDropDownOpen = false;
            //    DataGridMeasurementMapItems.CommitEdit(DataGridEditingUnit.Row, false);
            //}
        }

        private void ComboBox_OnDropDownOpened(object sender, EventArgs e)
        {
            var comboBox = (ComboBox) sender;
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(comboBox, ComboBox_PreviewMouseDownOutsideCapturedElement);
        }

        private void ComboBox_OnDropDownClosed(object sender, EventArgs e)
        {
            var comboBox = (ComboBox)sender;
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(comboBox, ComboBox_PreviewMouseDownOutsideCapturedElement);
        }

        private void ComboBox_PreviewMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
        {
            ((ComboBox) sender).IsDropDownOpen = false;
        }
    }
}
