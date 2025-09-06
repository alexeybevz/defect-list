using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefectListDomain.Commands;
using DefectListDomain.Dtos;
using DefectListDomain.EventArgs;
using DefectListDomain.ExternalData;
using DefectListDomain.Models;
using DefectListDomain.Services;
using ReporterBusinessLogic;

namespace DefectListBusinessLogic.Services
{
    public class BomItemsEditor : IBomItemsEditor
    {
        private BomHeader _bomHeader;
        private BomItem _oldBomItem;
        private BomItem _newBomItem;
        private List<BomItem> _bom;
        private string _login;

        private readonly IGetAllProductDtoQuery _getAllProductDtoQuery;
        private readonly IAsupBomContextFactory _asupBomContextFactory;
        private readonly IBomItemsLoader _bomItemsLoader;
        private readonly IUpdateBomItemNameCommand _updateBomItemNameCommand;
        private readonly IDeleteBomItemCommand _deleteBomItemCommand;
        private readonly IExpandBomItemNodeCommand _expandBomItemNodeCommand;
        private readonly ICollapseBomItemNodeCommand _collapseBomItemNodeCommand;
        private readonly ICreateLogActionCommand _createLogActionCommand;

        public BomItemsEditor(
            IGetAllProductDtoQuery getAllProductDtoQuery,
            IBomItemsLoader bomItemsLoader,
            IUpdateBomItemNameCommand updateBomItemNameCommand,
            IDeleteBomItemCommand deleteBomItemCommand,
            IExpandBomItemNodeCommand expandBomItemNodeCommand,
            ICollapseBomItemNodeCommand collapseBomItemNodeCommand,
            ICreateLogActionCommand createLogActionCommand,
            IAsupBomContextFactory asupBomContextFactory)
        {
            _getAllProductDtoQuery = getAllProductDtoQuery;
            _asupBomContextFactory = asupBomContextFactory;
            _bomItemsLoader = bomItemsLoader;
            _updateBomItemNameCommand = updateBomItemNameCommand;
            _deleteBomItemCommand = deleteBomItemCommand;
            _expandBomItemNodeCommand = expandBomItemNodeCommand;
            _collapseBomItemNodeCommand = collapseBomItemNodeCommand;
            _createLogActionCommand = createLogActionCommand;
        }

        #region Events
        public event EventHandler<ErrorEventArgs> OnError;
        #endregion

        public async Task Add(BomHeader bomHeader, BomItem parentBomItem, BomItem addBomItem, string login)
        {
            if (bomHeader == null) throw new ArgumentNullException(nameof(bomHeader));
            if (parentBomItem == null) throw new ArgumentNullException(nameof(parentBomItem));
            if (addBomItem == null) throw new ArgumentNullException(nameof(addBomItem));

            _bomHeader = bomHeader;
            _oldBomItem = parentBomItem;
            _newBomItem = addBomItem;
            _bom = null;
            _login = login;

            if (parentBomItem.UzelFlag != 1)
            {
                OnError?.Invoke(this, new ErrorEventArgs("Добавление ДСЕ возможно только в сборочную единицу. Операция отменена."));
                return;
            }

            if (parentBomItem.Detal == addBomItem.Detal)
            {
                OnError?.Invoke(this, new ErrorEventArgs("Запрещено добавлять ДСЕ само в себя. Операция отменена."));
                return;
            }

            var parentQty = parentBomItem.QtyMnf;
            var inputQtyOnOneUnit = addBomItem.QtyMnf;

            var newBomItem = await _getAllProductDtoQuery.ExecuteByDetals(SpecifKeyCreator.CreateKey(addBomItem.Detal));

            if (newBomItem == null)
            {
                OnError?.Invoke(this, new ErrorEventArgs("ДСЕ для добавления в состав не обнаружена в конструкторской документации АСУП. Операция отменена."));
                return;
            }

            addBomItem.UzelFlag = (byte)(newBomItem.IsAssembly ? 1 : 0);
            addBomItem.DetalTyp = newBomItem.Type;

            if (addBomItem.UzelFlag == 0)
            {
                var insertedItem = new AsupBomComponentDto()
                {
                    detals = SpecifKeyCreator.CreateKey(newBomItem.Name),
                    detal = newBomItem.Name,
                    imadetal = newBomItem.ExtName,
                    typ = newBomItem.Type,
                    units = newBomItem.Um,
                    plan_kol = inputQtyOnOneUnit,
                    StructureNumber = "1",
                    DetalCodeSL = newBomItem.CodeErp,
                    Detal_Product_Id = newBomItem.Id,
                    Detal_Code_LSF82 = newBomItem.CodeLsf82
                };

                await _bomItemsLoader.Execute(
                    new List<BomItem>(),
                    new List<AsupBomComponentDto>() { insertedItem },
                    parentBomItem.BomId,
                    parentBomItem.Id,
                    1,
                    inputQtyOnOneUnit,
                    _login
                );

                WriteLog(ActionType.Add);
                return;
            }

            if (addBomItem.UzelFlag == 1)
            {
                var newBomItems = _asupBomContextFactory.AskBase(_newBomItem.Detal, (decimal)(parentQty * inputQtyOnOneUnit));

                if (!newBomItems.Any())
                {
                    OnError?.Invoke(this, new ErrorEventArgs("Не обнаружено состава сборочной единицы для добавления в состав. Операция отменена."));
                    return;
                }

                await _bomItemsLoader.Execute(
                    new List<BomItem>(),
                    newBomItems,
                    parentBomItem.BomId,
                    parentBomItem.Id,
                    parentQty,
                    parentQty * inputQtyOnOneUnit,
                    _login
                );

                WriteLog(ActionType.Add);
            }
        }

        public async Task Replace(BomHeader bomHeader, BomItem oldBomItem, BomItem newBomItem, List<BomItem> bom, string login)
        {
            if (bomHeader == null) throw new ArgumentNullException(nameof(bomHeader));
            if (oldBomItem == null) throw new ArgumentNullException(nameof(oldBomItem));
            if (newBomItem == null) throw new ArgumentNullException(nameof(newBomItem));
            if (bom == null) throw new ArgumentNullException(nameof(bom));

            _bomHeader = bomHeader;
            _oldBomItem = oldBomItem;
            _newBomItem = newBomItem;
            _bom = bom;
            _login = login;


            if (oldBomItem.UzelFlag == 0)
            {
                await ItemReplace();
                return;
            }

            if (oldBomItem.UzelFlag == 1)
            {
                await AssemblyUnitReplace();
                return;
            }

            throw new ArgumentException("Неизвестное значение атрибута UserFlag");
        }

        public async Task ReplaceName(BomHeader bomHeader, BomItem oldBomItem, string newDetal, string login)
        {
            if (bomHeader == null) throw new ArgumentNullException(nameof(bomHeader));
            if (oldBomItem == null) throw new ArgumentNullException(nameof(oldBomItem));
            if (string.IsNullOrEmpty(newDetal)) throw new ArgumentNullException(nameof(newDetal));

            var oldDetal = oldBomItem.Detal;

            var obj = await _getAllProductDtoQuery.ExecuteByDetals(SpecifKeyCreator.CreateKey(newDetal));

            if (obj == null)
            {
                OnError?.Invoke(this, new ErrorEventArgs("ДСЕ для замены не обнаружена в конструкторской документации АСУП. Операция отменена."));
                return;
            }

            oldBomItem.Detals = SpecifKeyCreator.CreateKey(obj.Name);
            oldBomItem.Detal = obj.Name;
            oldBomItem.DetalIma = obj.ExtName;
            oldBomItem.DetalTyp = obj.Type;
            oldBomItem.DetalUm = obj.Um;
            oldBomItem.ClassifierID = obj.CodeErp;
            oldBomItem.ProductID = obj.Id;
            oldBomItem.Code_LSF82 = obj.CodeLsf82;

            _updateBomItemNameCommand.Execute(oldBomItem.Id, oldBomItem, login);

            var logActionType = _createLogActionCommand.LogActionTypes.FirstOrDefault(x => x.LogActionTypeName == _createLogActionCommand.LogActionTypesDictionary[ActionType.ReplaceName]);
            _createLogActionCommand.Execute(
                logActionType,
                bomHeader.BomId, 
                "BOM", 
                $@"ID ведомости {{{bomHeader.BomId}}}. Изменено обозначение ДСЕ ID {{{oldBomItem.Id}}} с {{{oldDetal}}} на {{{newDetal}}}",
                $@"ДВ {{{bomHeader.Orders}}} в узле {{{oldBomItem.ParentDetal}}} изменено обозначение ДСЕ с {{{oldDetal}}} на {{{newDetal}}}",
                login);
        }

        public async Task Delete(BomHeader bomHeader, BomItem deletedBomItem, List<BomItem> deletedBomItems, string login)
        {
            await _deleteBomItemCommand.Execute(deletedBomItems, login);

            var logActionType =
                _createLogActionCommand.LogActionTypes.FirstOrDefault(x => x.LogActionTypeName == _createLogActionCommand.LogActionTypesDictionary[ActionType.Delete]);

            _createLogActionCommand.Execute(
                logActionType,
                bomHeader.BomId,
                "BOM",
                $@"ID ведомости {{{bomHeader.BomId}}}. Удалено ID {{{deletedBomItem.Id}}}, обозначение {{{deletedBomItem.Detal}}}, тип {{{deletedBomItem.DetalTyp}}}",
                $@"ДВ {{{bomHeader.Orders}}} в узле {{{deletedBomItem.ParentDetal}}} удалена ДСЕ {{{deletedBomItem.Detal}}}",
                login);
        }

        public async Task Expand(int assemblyBomItemId, string assemblyBomItemStructureNumber, List<BomItem> assemblyBom, bool isExpandAll = false)
        {
            await _expandBomItemNodeCommand.Execute(assemblyBomItemId, assemblyBomItemStructureNumber, assemblyBom, isExpandAll);
        }

        public async Task Collapse(int assemblyBomItemId, List<BomItem> assemblyBom)
        {
            await _collapseBomItemNodeCommand.Execute(assemblyBomItemId, assemblyBom);
        }

        private async Task AssemblyUnitReplace()
        {
            if (_oldBomItem.UzelFlag != 1)
            {
                OnError?.Invoke(this, new ErrorEventArgs("Операция выполняется только для сборочных единиц."));
                return;
            }

            var deletedBomItems = _bom
                .Where(x => x.StructureNumber.StartsWith(_oldBomItem.StructureNumber))
                .ToList();

            var parentItem = _bom.FirstOrDefault(x => x.Id == _oldBomItem.ParentId);
            float parentQty = 1;
            if (parentItem != null)
                parentQty = parentItem.QtyMnf;

            var inputQtyOnOneUnit = _newBomItem.QtyMnf;

            var newBomItem = await _getAllProductDtoQuery.ExecuteByDetals(SpecifKeyCreator.CreateKey(_newBomItem.Detal));

            if (newBomItem == null)
            {
                OnError?.Invoke(this, new ErrorEventArgs("ДСЕ для замены не обнаружена в конструкторской документации АСУП. Операция отменена."));
                return;
            }

            _newBomItem.UzelFlag = (byte)(newBomItem.IsAssembly ? 1 : 0);
            _newBomItem.DetalTyp = newBomItem.Type;

            var newBomItems = _asupBomContextFactory.AskBase(_newBomItem.Detal, (decimal)(parentQty * inputQtyOnOneUnit));

            if (!newBomItems.Any())
            {
                OnError?.Invoke(this, new ErrorEventArgs("Не обнаружено состава сборочной единицы для замены. Операция отменена."));
                return;
            }

            var rootBomItemId = await _bomItemsLoader.Execute(
                deletedBomItems,
                newBomItems,
                _oldBomItem.BomId,
                _oldBomItem.ParentId,
                parentQty,
                parentQty * inputQtyOnOneUnit,
                _login
            );

            WriteLog(ActionType.Replace);
        }

        private async Task ItemReplace()
        {
            if (_oldBomItem.UzelFlag != 0)
            {
                OnError?.Invoke(this, new ErrorEventArgs("Операция не выполняется для сборочных единиц."));
                return;
            }

            var inputQtyOnOneUnit = _newBomItem.QtyMnf;

            var newBomItem = await _getAllProductDtoQuery.ExecuteByDetals(SpecifKeyCreator.CreateKey(_newBomItem.Detal));

            if (newBomItem == null)
            {
                OnError?.Invoke(this, new ErrorEventArgs("ДСЕ для замены не обнаружена в конструкторской документации АСУП. Операция отменена."));
                return;
            }

            _newBomItem.UzelFlag = (byte)(newBomItem.IsAssembly ? 1 : 0);
            _newBomItem.DetalTyp = newBomItem.Type;

            var insertedItem = new AsupBomComponentDto()
            {
                detals = SpecifKeyCreator.CreateKey(newBomItem.Name),
                detal = newBomItem.Name,
                imadetal = newBomItem.ExtName,
                typ = newBomItem.Type,
                units = newBomItem.Um,
                plan_kol = inputQtyOnOneUnit,
                StructureNumber = "1",
                DetalCodeSL = newBomItem.CodeErp,
                Detal_Product_Id = newBomItem.Id,
                Detal_Code_LSF82 = newBomItem.CodeLsf82
            };

            await _bomItemsLoader.Execute(
                new List<BomItem>() { _oldBomItem },
                new List<AsupBomComponentDto>() { insertedItem },
                _oldBomItem.BomId,
                _oldBomItem.ParentId,
                1,
                inputQtyOnOneUnit,
                _login
            );

            WriteLog(ActionType.Replace);
        }

        private void WriteLog(ActionType actionType)
        {
            var logActionType =
                _createLogActionCommand.LogActionTypes.FirstOrDefault(x => x.LogActionTypeName == _createLogActionCommand.LogActionTypesDictionary[actionType]);

            if (actionType == ActionType.Add)
                _createLogActionCommand.Execute(
                    logActionType,
                    _bomHeader.BomId,
                    "BOM",
                    $@"ID ведомости {{{_oldBomItem.BomId}}}. В родительский узел ID {{{_oldBomItem.Id}}} добавлено {{{_newBomItem.DetalTyp}}} {{{_newBomItem.Detal}}}",
                    $@"ДВ {{{_bomHeader.Orders}}} в узле {{{_oldBomItem.ParentDetal}}} добавлена ДСЕ {{{_newBomItem.Detal}}}",
                    _login);

            if (actionType == ActionType.Replace)
                _createLogActionCommand.Execute(logActionType,
                    _bomHeader.BomId,
                    "BOM",
                    $@"ID ведомости {{{_oldBomItem.BomId}}}. В родительском узле ID {{{_oldBomItem.ParentId}}} произведена замена {{{_oldBomItem.DetalTyp}}} {{{_oldBomItem.Detal}}} на {{{_newBomItem.DetalTyp}}} {{{_newBomItem.Detal}}}",
                    $@"ДВ {{{_bomHeader.Orders}}} в узле {{{_oldBomItem.ParentDetal}}} произведена замена ДСЕ {{{_oldBomItem.Detal}}} на {{{_newBomItem.Detal}}}",
                    _login);
        }
    }
}
