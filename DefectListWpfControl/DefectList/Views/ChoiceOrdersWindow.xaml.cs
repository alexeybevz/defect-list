using System.Collections.Generic;
using System.Windows;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Views
{
    public partial class ChoiceOrdersWindow : Window
    {
        public IEnumerable<BomHeader> SelectBomHeaders { get; private set; }

        public ChoiceOrdersWindow(IEnumerable<BomHeader> bomHeaders)
        {
            InitializeComponent();
            var viewModel = new ChoiceOrdersViewModel(bomHeaders);
            viewModel.OnOrdersSelected += ViewModel_OnOrdersSelected;
            DataContext = viewModel;
        }

        private void ViewModel_OnOrdersSelected(IEnumerable<BomHeader> selectedBomHeaders)
        {
            SelectBomHeaders = selectedBomHeaders;
            this.Close();
        }

        private void FilterStringTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            FilterStringTextBox.Focus();
        }
    }
}
