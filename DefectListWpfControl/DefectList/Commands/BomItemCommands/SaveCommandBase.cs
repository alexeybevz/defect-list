using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using DefectListDomain.Services;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public abstract class SaveCommandBase : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomItemsStore _bomItemsStore;
        private readonly ISaveBomItemCommand _saveBomItemCommand;
        private readonly IBomItemsValidator _bomItemsValidator;

        protected readonly List<BomItemViewModel> BomItemsToRequiredSubmit =
            new List<BomItemViewModel>();

        protected SaveCommandBase(
            DefectListItemViewModel bomItemViewModel,
            BomItemsStore bomItemsStore,
            ISaveBomItemCommand saveBomItemCommand,
            IBomItemsValidator bomItemValidator)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomItemsStore = bomItemsStore;
            _saveBomItemCommand = saveBomItemCommand;
            _bomItemsValidator = bomItemValidator;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            OnBeforeSave();
            await SaveAsync(parameter);
            OnAfterSave();
        }

        protected virtual void OnBeforeSave()
        {
            BomItemsToRequiredSubmit.Clear();
        }

        protected abstract Task SaveAsync(object parameter);

        protected virtual void OnAfterSave()
        {
            if (!BomItemsToRequiredSubmit.Any())
                return;

            var items = BomItemsToRequiredSubmit.Select(x => x.Detal).ToList();
            var str = string.Join(Environment.NewLine, items);
            MessageBox.Show("Следующие ДСЕ требуется предъявить ВП:\n\n" + str, 
                "Внимание",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) &&
                   PermissionsStore.IsWriteAccessUser &&
                   !_bomItemViewModel.BomHeader.StateInfo.IsClosed &&
                   (_bomItemViewModel.SelectedBomItemViewModel?.IsValid ?? false);
        }

        protected async Task UpdateDefectPropsAndMoveNext(bool isApplyOnAssembly, Func<IBomItemModel, bool> filterBomItems = null)
        {
            if (_bomItemViewModel.SelectedBomItemViewModel == null)
                return;

            var current = new BomItemViewModel(_bomItemViewModel.SelectedBomItemViewModel);

            var tempDefect = _bomItemViewModel.SelectedBomItemViewModel.Defect;
            var tempDecision = _bomItemViewModel.SelectedBomItemViewModel.Decision;
            var tempResearchAction = _bomItemViewModel.SelectedBomItemViewModel.ResearchAction;
            var tempResearchResult = _bomItemViewModel.SelectedBomItemViewModel.ResearchResult;

            if (isApplyOnAssembly && _bomItemViewModel.SelectedBomItemViewModel.UzelFlag == 1)
            {
                var dialogResult = MessageBox.Show($"Дефект и решение будут применены ко всему составу сборочной единицы {_bomItemViewModel.SelectedBomItemViewModel.Detal} - {_bomItemViewModel.SelectedBomItemViewModel.DetalIma}. Вы уверены?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                var dialogResult2 = dialogResult == MessageBoxResult.Yes ? MessageBox.Show("Вы действительно хотите применить дефект и решение ко всему составу сборочной единицы?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) : MessageBoxResult.No;

                if (dialogResult == MessageBoxResult.Yes && dialogResult2 == MessageBoxResult.Yes)
                    if (await ApplyTempDefectAndDecision(filterBomItems, tempDefect, tempDecision, tempResearchAction, tempResearchResult))
                        return;
            }

            var saveResult = await UpdateDefectProps(_bomItemViewModel.SelectedBomItemViewModel);

            if (_bomItemViewModel.SelectedBomItemViewModel != null && current.IsExpanded != _bomItemViewModel.SelectedBomItemViewModel.IsExpanded)
            {
                await _bomItemViewModel.LoadBomItemsCommand.ExecuteAsync();

                var newObject = _bomItemViewModel.BomItemsView.Cast<BomItemViewModel>().FirstOrDefault(x => x.Id == current.Id);
                if (newObject != null)
                    _bomItemViewModel.BomItemsView.MoveCurrentTo(newObject);
            }

            if (saveResult)
                _bomItemViewModel.BomItemsView.MoveCurrentToNext();
        }

        protected async Task<bool> ApplyTempDefectAndDecision(Func<IBomItemModel, bool> filterBomItems, string tempDefect, string tempDecision, string tempResearchAction, string tempResearchResult)
        {
            BomItemState bomItemState;
            try
            {
                bomItemState = GetBomItemStateFromQty(_bomItemViewModel.SelectedBomItemViewModel);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                await _bomItemViewModel.LoadBomItemCommand.ExecuteAsync();
                return true;
            }

            await UpdateDefectProps(_bomItemViewModel.SelectedBomItemViewModel);

            var bomItems = _bomItemViewModel.BomItemsView.OfType<BomItemViewModel>().Where(filterBomItems).Where(x => x.Id != _bomItemViewModel.SelectedBomItemViewModel.Id).ToList();

            foreach (var bomItemModel in bomItems)
            {
                string defectState;
                switch (bomItemState)
                {
                    case BomItemState.Restore:
                        defectState = "Ремонт";
                        break;
                    case BomItemState.Replace:
                        defectState = "Замена";
                        break;
                    case BomItemState.Good:
                        defectState = "Годная";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _bomItemViewModel.SelectedBomItemViewModel = (BomItemViewModel)bomItemModel;
                _bomItemViewModel.SelectedBomItemViewModel.QtyRestore = 0;
                _bomItemViewModel.SelectedBomItemViewModel.QtyReplace = 0;
                _bomItemViewModel.SelectedBomItemViewModel.Defect = string.Empty;
                _bomItemViewModel.SelectedBomItemViewModel.Decision = string.Empty;
                _bomItemViewModel.SelectedBomItemViewModel.FinalDecision = string.Empty;
                _bomItemViewModel.SelectedBomItemViewModel.TechnologicalProcessUsed = string.Empty;
                _bomItemViewModel.SelectedBomItemViewModel.ResearchAction = string.Empty;
                _bomItemViewModel.SelectedBomItemViewModel.ResearchResult = string.Empty;

                _bomItemViewModel.MapDefectToDecisionChanged(new DefectToDecisionMapCheckBoxViewModel(new MapDefectToDecision()
                {
                    Defect = tempDefect,
                    Decision = tempDecision,
                    StateDetals = new StateDetals() { StateDetalsName = defectState },
                }, _bomItemViewModel.MapDefectToDecisionChanged),
                    true);

                _bomItemViewModel.SelectedBomItemViewModel.ResearchAction = tempResearchAction;
                _bomItemViewModel.SelectedBomItemViewModel.ResearchResult = tempResearchResult;

                if (await UpdateDefectProps(_bomItemViewModel.SelectedBomItemViewModel))
                    _bomItemViewModel.BomItemsView.MoveCurrentToNext();
            }

            return false;
        }

        protected async Task<bool> UpdateDefectProps(BomItemViewModel defectListItem)
        {
            await _bomItemViewModel.RefreshDefectListHeader();

            string stage = "Начало";

            if (defectListItem == null)
                return false;

            var current = new BomItemViewModel(defectListItem);

            try
            {
                stage = "Извлечение актуальной информации";

                var bomItemDb = await _bomItemsStore.GetBomItemById(current.Id);
                if (bomItemDb == null)
                {
                    MessageBox.Show(
                        "Строка была удалена другим пользователем.\nДефектовочная ведомость будет обновлена целиком.");
                    await _bomItemViewModel.LoadBomItemsCommand.ExecuteAsync();
                    return false;
                }

                bool isExistsChangedProps =
                    current.QtyRestore != bomItemDb.QtyRestore ||
                    current.QtyReplace != bomItemDb.QtyReplace ||
                    current.Defect != bomItemDb.Defect ||
                    current.Decision != bomItemDb.Decision ||
                    current.FinalDecision != bomItemDb.FinalDecision ||
                    current.IsRequiredSubmit != bomItemDb.IsRequiredSubmit ||
                    current.IsSubmitted != bomItemDb.IsSubmitted ||
                    current.TechnologicalProcessUsed != bomItemDb.TechnologicalProcessUsed ||
                    current.CommentDef != bomItemDb.CommentDef ||
                    current.SerialNumber != bomItemDb.SerialNumber ||
                    current.ResearchAction != bomItemDb.ResearchAction ||
                    current.ResearchResult != bomItemDb.ResearchResult;

                if (!isExistsChangedProps)
                    return false;

                stage = "Проверка количества";

                var validationResult =
                    _bomItemsValidator.Execute(_bomItemViewModel.BomItemsView.OfType<BomItemViewModel>().Select(x => x.BomItem).ToList(), current);
                if (validationResult != null)
                {
                    _bomItemViewModel._errorMessage.Text = validationResult;
                    _bomItemViewModel.InfoMessage = _bomItemViewModel._errorMessage;
                    return false;
                }

                if (bomItemDb.RecordDate != current.RecordDate)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Ваши текущие значения:");
                    sb.AppendLine($"Кол-во(ремонт) : {current.QtyRestore}");
                    sb.AppendLine($"Кол-во(замена) : {current.QtyReplace}");
                    sb.AppendLine($"Дефект: {current.Defect}");
                    sb.AppendLine($"Решение: {current.Decision}");
                    sb.AppendLine($"Окончательное решение: {current.FinalDecision}");
                    sb.AppendLine($"Требуется предъявить ВП: {current.IsRequiredSubmit}");
                    sb.AppendLine($"Предъявлено ВП: {current.IsSubmitted}");
                    sb.AppendLine($"Применяемый тех. процесс: {current.TechnologicalProcessUsed}");
                    sb.AppendLine($"Примечание: {current.CommentDef}");
                    sb.AppendLine($"Заводской номер: {current.SerialNumber}");
                    sb.AppendLine("\nЗначения, сохранненые другим пользователем:");
                    sb.AppendLine($"Кол-во(ремонт) : {bomItemDb.QtyRestore}");
                    sb.AppendLine($"Кол-во(замена) : {bomItemDb.QtyReplace}");
                    sb.AppendLine($"Дефект: {bomItemDb.Defect}");
                    sb.AppendLine($"Решение: {bomItemDb.Decision}");
                    sb.AppendLine($"Окончательное решение: {bomItemDb.FinalDecision}");
                    sb.AppendLine($"Требуется предъявить ВП: {bomItemDb.IsRequiredSubmit}");
                    sb.AppendLine($"Предъявлено ВП: {bomItemDb.IsSubmitted}");
                    sb.AppendLine($"Применяемый тех. процесс: {bomItemDb.TechnologicalProcessUsed}");
                    sb.AppendLine($"Примечание: {bomItemDb.CommentDef}");
                    sb.AppendLine($"Заводской номер: {bomItemDb.SerialNumber}");
                    sb.AppendLine($"Мероприятия по изучению: {bomItemDb.ResearchAction}");
                    sb.AppendLine($"Результаты изучения: {bomItemDb.ResearchResult}");

                    var dialogResult = MessageBox.Show(
                        $"Во время работы над дефектовочной ведомостью строка была обновлена пользователем '{bomItemDb.UpdatedBy}'.\n\n{sb}\n" +
                        "Применить ваши значения?", "Внимание", MessageBoxButton.OKCancel);
                    if (dialogResult == MessageBoxResult.Cancel)
                        return false;
                }

                stage = "Сохранение информации";

                _saveBomItemCommand.Execute(current, _bomItemViewModel.UserIdentity.Name);

                stage = "Обновление формы";

                if (current.IsRequiredSubmit == IsBomItemRequiredSubmit.Yes)
                    BomItemsToRequiredSubmit.Add(current);

                await _bomItemViewModel.LoadBomItemCommand.ExecuteAsync();

                _bomItemViewModel.InfoMessage = _bomItemViewModel._successMessage;

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine +
                                $"Этап: {stage}" + Environment.NewLine +
                                $"Позиция: {current.StructureNumber}" + Environment.NewLine +
                                $"ДСЕ: {current.Detal}");
                return false;
            }
        }

        private BomItemState GetBomItemStateFromQty(IBomItem bi)
        {
            if (bi.QtyRestore > 0 && bi.QtyReplace == 0)
                return BomItemState.Restore;
            if (bi.QtyRestore == 0 && bi.QtyReplace > 0)
                return BomItemState.Replace;
            if (bi.QtyRestore == 0 && bi.QtyReplace == 0)
                return BomItemState.Good;

            throw new InvalidEnumArgumentException("Не удалось определить состояние ДСЕ (Ремонт/Замена/Годная). Операция отменена");
        }

        private enum BomItemState
        {
            Restore,
            Replace,
            Good
        }
    }
}