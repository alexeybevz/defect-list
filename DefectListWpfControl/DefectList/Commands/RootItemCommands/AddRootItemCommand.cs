using System;
using System.Threading.Tasks;
using DefectListBusinessLogic.Services;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.RootItemCommands
{
    public class AddRootItemCommand : AsyncCommandBase
    {
        private readonly AddRootItemViewModel _addRootItemViewModel;
        private readonly RootItemsStore _rootItemsStore;

        public AddRootItemCommand(AddRootItemViewModel addRootItemViewModel, RootItemsStore rootItemsStore)
        {
            _addRootItemViewModel = addRootItemViewModel;
            _rootItemsStore = rootItemsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            _addRootItemViewModel.ErrorMessage = null;
            _addRootItemViewModel.IsSubmitting = true;

            try
            {
                var rootItem = new RootItem()
                {
                    Izdels = SpecifKeyCreator.CreateKey(_addRootItemViewModel.Product.Name),
                    Izdel = _addRootItemViewModel.Product.Name,
                    IzdelIma = _addRootItemViewModel.Product.ExtName,
                    IzdelTyp = _addRootItemViewModel.Product.Type,
                    IzdelInitial = _addRootItemViewModel.ProductInitial.Name
                };

                await _rootItemsStore.Add(rootItem);
            }
            catch (Exception ex)
            {
                _addRootItemViewModel.ErrorMessage = "Ошибка: " + ex.Message;
            }
            finally
            {
                _addRootItemViewModel.IsSubmitting = false;
            }
        }
    }
}