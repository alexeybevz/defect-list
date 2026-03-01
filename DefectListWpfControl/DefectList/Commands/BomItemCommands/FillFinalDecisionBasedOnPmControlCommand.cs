using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using DefectListDomain.Services;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class FillFinalDecisionBasedOnPmControlCommand : SaveCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomHeadersStore _bomHeadersStore;

        public FillFinalDecisionBasedOnPmControlCommand(
            DefectListItemViewModel bomItemViewModel,
            BomItemsStore bomItemsStore,
            BomHeadersStore bomHeadersStore,
            ISaveBomItemCommand saveBomItemCommand,
            IBomItemsValidator bomItemsValidator) : base(bomItemViewModel, bomItemsStore, saveBomItemCommand, bomItemsValidator)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomHeadersStore = bomHeadersStore;
        }

        protected override async Task SaveAsync(object parameter)
        {
            try
            {
                var dialogResult = MessageBox.Show("Вы действительно хотите заполнить окончательное решение на основе данных из ПМ Контроль?", "Внимание",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (dialogResult == MessageBoxResult.No)
                    return;

                var routeCharts = await _bomHeadersStore.GetRouteChartsTrackInfos(_bomItemViewModel.BomHeader.Orders);

                var bomItemsDict = _bomItemViewModel
                    .BomItemsView
                    .OfType<BomItemViewModel>()
                    .Where(x => x.Decision == "ремонт" && string.IsNullOrEmpty(x.FinalDecision))
                    .ToDictionary(k => k.Id, v => v);

                var duplicates = bomItemsDict.Values
                    .Where(x => x.MapBomItemToRouteCharts != null)
                    .SelectMany(x => x.MapBomItemToRouteCharts)
                    .GroupBy(x => x.RouteChart_Number)
                    .Where(x => x.Count() > 1)
                    .ToDictionary(x => x.Key, v => v.ToList());

                var runsRouteMaps = bomItemsDict.Values
                    .Where(x => x.MapBomItemToRouteCharts != null)
                    .SelectMany(x => x.MapBomItemToRouteCharts)
                    .Where(x => !duplicates.ContainsKey(x.RouteChart_Number)) // TODO: временное решение
                    .ToDictionary(k => k.RouteChart_Number, v => v);

                foreach (var rc in routeCharts)
                {
                    MapBomItemToRouteChart link;
                    BomItemViewModel bomItem;

                    if (runsRouteMaps.TryGetValue(rc.RouteChart_Number, out link))
                    {
                        bomItemsDict.TryGetValue(link.BomItemId, out bomItem);
                    }
                    else
                    {
                        var isRepair = rc.Detals.Last() == 'Р';

                        bomItem = isRepair
                            ? bomItemsDict.Values.FirstOrDefault(x => x.Detals + 'Р' == rc.Detals)
                            : bomItemsDict.Values.FirstOrDefault(x => x.Detals == rc.Detals);
                    }

                    if (bomItem == null)
                        continue;

                    if (rc.IsArchive)
                    {
                        bomItem.FinalDecision = "ремонт";
                        bomItem.QtyRestore = bomItem.QtyMnf;
                    }
                    else if (rc.WP_Name == "24-07  Изолятор брака ремонтных ДСЕ")
                    {
                        bomItem.FinalDecision = "заменить";
                        bomItem.QtyReplace = bomItem.QtyMnf;
                    }
                    else
                        continue;

                    await UpdateDefectProps(new BomItemViewModel(bomItem, null, null, null));
                }

                var targetDecisions = new List<string>() { "заменить", "ремонтное воздействие не требуется", "скомплектовать", "использовать" };
                var bomItemsWithEmptyFinalDecision = _bomItemViewModel.BomItemsView.OfType<BomItemViewModel>().Where(x =>
                    targetDecisions.Contains(x.Decision?.Trim()) && string.IsNullOrEmpty(x.FinalDecision?.Trim())).ToList();
                foreach (var obj in bomItemsWithEmptyFinalDecision)
                {
                    _bomItemViewModel.BomItemDecisionChanged(obj, obj.Decision);
                    await UpdateDefectProps(obj);
                }

                await _bomItemViewModel.LoadBomItemsCommand.ExecuteAsync();
                MessageBox.Show("Готово");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && 
                   PermissionsStore.IsCanFillFinalDecisionBasedOnPmControlUser &&
                   !_bomItemViewModel.BomHeader.StateInfo.IsClosed;
        }
    }
}