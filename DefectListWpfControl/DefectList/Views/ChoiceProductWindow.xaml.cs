using System.Windows;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Views
{
    /// <summary>
    /// Interaction logic for ChoiceProductWindow.xaml
    /// </summary>
    public partial class ChoiceProductWindow : Window
    {
        public ChoiceProductWindow(ChoiceProductViewModel choiceProductViewModel)
        {
            InitializeComponent();
            DataContext = choiceProductViewModel;

            choiceProductViewModel.FilterTextUpdated += () =>
            {
                if (ProductListBox.Items.Count > 0)
                    ProductListBox.ScrollIntoView(ProductListBox.Items[0]);
            };
        }
    }
}
