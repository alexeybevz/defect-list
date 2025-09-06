using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Views
{
    public partial class SelectItemWindow : Window
    {
        public SelectItemWindow(ProductsStore productsStore, bool isVisibleQty = true)
        {
            var vm = new SelectItemViewModel(productsStore, isVisibleQty);
            vm.NewBomItemSelected += (item, product) => Close();
            DataContext = vm;
            InitializeComponent();
        }
    }
}
