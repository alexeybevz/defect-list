using System.Threading.Tasks;
using DefectListDomain.Queries;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using Ninject;

namespace DefectListWpfControl.DefectList.Factories
{
    // Единственная точка создания MeasurementMapDictionaryListWindow.
    // Инжектируется туда, откуда нужно открыть окно (например, в ViewModel
    // главного меню или в обработчик MenuItem.Click).
    //
    // Использование:
    //   await _factory.OpenAsync();
    public class MeasurementMapDictionaryListWindowFactory
    {
        private readonly MeasurementMapDictionaryStore _dictionaryStore;
        private readonly IGetMeasurementsMapTypeFormsQuery _typeFormsQuery;

        [Named("PossibleDefect")]
        private readonly LookupStore _possibleDefectStore;

        [Named("NominalValue")]
        private readonly LookupStore _nominalValueStore;

        [Named("RecommendedRepairMethod")]
        private readonly LookupStore _repairMethodStore;

        [Named("RequirementPostRepair")]
        private readonly LookupStore _requirementStore;

        private readonly ProductsStore _productsStore;

        public MeasurementMapDictionaryListWindowFactory(
            MeasurementMapDictionaryStore dictionaryStore,
            IGetMeasurementsMapTypeFormsQuery typeFormsQuery,
            [Named("PossibleDefect")]          LookupStore possibleDefectStore,
            [Named("NominalValue")]            LookupStore nominalValueStore,
            [Named("RecommendedRepairMethod")] LookupStore repairMethodStore,
            [Named("RequirementPostRepair")]   LookupStore requirementStore,
            ProductsStore productsStore)
        {
            _dictionaryStore = dictionaryStore;
            _typeFormsQuery = typeFormsQuery;
            _possibleDefectStore = possibleDefectStore;
            _nominalValueStore = nominalValueStore;
            _repairMethodStore = repairMethodStore;
            _requirementStore = requirementStore;
            _productsStore = productsStore;
        }

        public async Task OpenAsync()
        {
            var typeForms = await _typeFormsQuery.ExecuteAsync();

            var vm = new MeasurementMapDictionaryListViewModel(
                _dictionaryStore,
                _possibleDefectStore,
                _nominalValueStore,
                _repairMethodStore,
                _requirementStore,
                _productsStore,
                typeForms);

            var window = new MeasurementMapDictionaryListWindow { DataContext = vm };
            window.ShowDialog();
        }
    }
}