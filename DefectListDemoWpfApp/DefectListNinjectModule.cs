using System;
using System.Configuration;
using System.Collections.Generic;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using DefectListBusinessLogic.Commands;
using DefectListBusinessLogic.Queries;
using DefectListBusinessLogic.Services;
using DefectListBusinessLogic.Fakes;
using DefectListBusinessLogic.Report;
using DefectListDomain.Queries;
using DefectListDomain.Commands;
using DefectListDomain.CreatingReports;
using DefectListDomain.ExternalData;
using DefectListDomain.Models;
using DefectListDomain.Services;
using DefectListWpfControl.DefectList.Factories;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListDemoWpfApp
{
    public class DefectListNinjectModule : NinjectModule
    {
        public override void Load()
        {
            var connections = new Dictionary<DatabaseConnectionName, ConnectionStringSettings>
            {
                {DatabaseConnectionName.PmControl, ConfigurationManager.ConnectionStrings["PMcontrol_Product"]},
            };

            Bind<IDictionary<DatabaseConnectionName, ConnectionStringSettings>>().ToConstant(connections)
                .InSingletonScope();
            Bind<IDbConnectionFactory>().To<DapperDbConnectionFactory>().InSingletonScope();

            Bind<IGetAllBomHeaderSubscribersQuery>().To<GetAllBomHeaderSubscribersQuery>().InTransientScope();
            Bind<ISubscribeUserOnBomHeaderCommand>().To<SubscribeUserOnBomHeaderCommand>().InTransientScope();
            Bind<IUnSubscribeUserOnBomHeaderCommand>().To<UnSubscribeUserOnBomHeaderCommand>().InTransientScope();
            Bind<ICreateNotificationEventSubscriberCommand>().To<CreateNotificationEventSubscriberCommand>().InTransientScope();
            Bind<IDeleteNotificationEventSubscriberCommand>().To<DeleteNotificationEventSubscriberCommand>().InTransientScope();
            Bind<IGetAllNotificationEventSubscribersQuery>().To<GetAllNotificationEventSubscribersQuery>().InTransientScope();
            Bind<BomHeaderSubscribersStore>().ToSelf().InTransientScope();

            Bind<IGetAllBomHeadersQuery>().To<GetAllBomHeadersQuery>().InTransientScope();
            Bind<IGetAllBomItemsWithSerialNumbersQuery>().To<GetAllBomItemsWithSerialNumbersQuery>().InTransientScope();
            Bind<ICreateBomHeaderCommand>().To<CreateBomHeaderCommand>().InTransientScope();
            Bind<IUpdateBomHeaderCommand>().To<UpdateBomHeaderCommand>().InTransientScope();
            Bind<IDeleteBomHeaderCommand>().To<DeleteBomHeaderCommand>().InTransientScope();
            Bind<IUpdateStateBomHeaderCommand>().To<UpdateStateBomHeaderCommand>().InTransientScope();
            Bind<BomHeadersStore>().ToSelf().InSingletonScope();
            Bind<SelectedBomHeaderStore>().ToSelf().InSingletonScope();
            Bind<OpenedBomHeadersStore>().ToSelf().InSingletonScope();

            Bind<IGetAllRootItemsQuery>().To<GetAllRootItemsQuery>().InTransientScope();
            Bind<ICreateRootItemCommand>().To<CreateRootItemCommand>().InTransientScope();
            Bind<IDeleteRootItemCommand>().To<DeleteRootItemCommand>().InTransientScope();
            Bind<RootItemsStore>().ToSelf().InTransientScope();

            Bind<IGetAllBomItemsByBomHeaderQuery>().To<GetAllBomItemsByBomHeaderQuery>().InTransientScope();
            Bind<IGetBomViewService>().To<GetBomViewService>().InTransientScope();
            Bind<IGetAllBomItemLogsQuery>().To<GetAllBomItemLogsQuery>().InTransientScope();
            Bind<IGetAllBomItemsFilterToReportQuery>().To<GetAllBomItemsFilterToReportQuery>().InTransientScope();
            Bind<ISaveBomItemCommand>().To<SaveBomItemCommand>().InTransientScope();
            Bind<IUpdateBomItemNameCommand>().To<UpdateBomItemNameCommand>().InTransientScope();
            Bind<IUpdateBomItemQtyCommand>().To<UpdateBomItemQtyCommand>().InTransientScope();
            Bind<IExpandBomItemNodeCommand>().To<ExpandBomItemNodeCommand>().InTransientScope();
            Bind<ICollapseBomItemNodeCommand>().To<CollapseBomItemNodeCommand>().InTransientScope();
            Bind<IDeleteBomItemCommand>().To<DeleteBomItemCommand>().InTransientScope();
            Bind<ICreateBomItemCommand>().To<CreateBomItemCommand>().InTransientScope();
            Bind<BomItemsStore>().ToSelf().InTransientScope();

            Bind<IGetAllDefectToDecisionMapsQuery>().To<GetAllDefectToDecisionMapsQuery>().InTransientScope();
            Bind<IGetAllStateDetalsQuery>().To<GetAllStateDetalsQuery>().InTransientScope();
            Bind<IGetAllGroupDefectsQuery>().To<GetAllGroupDefectsQuery>().InTransientScope();
            Bind<IGetAllDecisionsQuery>().To<GetAllDecisionsQuery>().InTransientScope();
            Bind<ICreateDefectToDecisionMapCommand>().To<CreateDefectToDecisionMapCommand>().InTransientScope();
            Bind<IUpdateDefectToDecisionMapCommand>().To<UpdateDefectToDecisionMapCommand>().InTransientScope();
            Bind<IDeleteDefectToDecisionMapCommand>().To<DeleteDefectToDecisionMapCommand>().InTransientScope();
            Bind<DefectToDecisionMapsStore>().ToSelf().InTransientScope();
            Bind<SelectedDefectToDecisionMapStore>().ToSelf().InTransientScope();

            Bind<IGetAllMapsBomItemToRouteChartsQuery>().To<GetAllMapsBomItemToRouteChartsQuery>().InTransientScope();
            Bind<ICreateMapBomItemToRouteChartCommand>().To<CreateMapBomItemToRouteChartCommand>().InTransientScope();
            Bind<IDeleteMapBomItemToRouteChartCommand>().To<DeleteMapBomItemToRouteChartCommand>().InTransientScope();
            Bind<IUpdateBomItemIdInMapToRouteChartCommand>().To<UpdateBomItemIdInMapToRouteChartCommand>().InTransientScope();
            Bind<SelectedBomItemStore>().ToSelf().InTransientScope();

            Bind<ICreateBomItemLogCommand>().To<CreateBomItemLogCommand>().InTransientScope();
            Bind<ICreateBomItemDocCommand>().To<CreateBomItemDocCommand>().InTransientScope();
            Bind<ICreateLogActionCommand>().To<CreateLogActionCommand>().InTransientScope();

            Bind<IGetMeasurementMapByBomItemIdQuery>().To<GetMeasurementMapByBomItemIdQuery>().InTransientScope();
            Bind<IGetMeasurementMapDictionaryByCodeQuery>().To<GetMeasurementMapDictionaryByCodeQuery>().InTransientScope();
            Bind<IGetAllMeasurementMapItemLogsQuery>().To<GetAllMeasurementMapItemLogsQuery>().InTransientScope();
            Bind<ICreateMeasurementMapCommand>().To<CreateMeasurementMapCommand>().InTransientScope();
            Bind<ISaveMeasurementMapItemsCommand>().To<SaveMeasurementMapItemsCommand>().InTransientScope();
            Bind<IMeasurementMapItemPresenterService>().To<MeasurementMapItemPresenterService>().InTransientScope();
            Bind<MeasurementMapStore>().ToSelf().InTransientScope();

            Bind<IArchiveMeasurementMapDictionaryCommand>().To<ArchiveMeasurementMapDictionaryCommand>().InTransientScope();
            Bind<ICreateLookupItemCommand>().To<CreateLookupItemCommand>().InTransientScope();
            Bind<ICreateMeasurementMapDictionaryCommand>().To<CreateMeasurementMapDictionaryCommand>().InTransientScope();
            Bind<ICopyMeasurementMapDictionaryCommand>().To<CopyMeasurementMapDictionaryCommand>().InThreadScope();
            Bind<ICreateMeasurementMapDictionaryItemCommand>().To<CreateMeasurementMapDictionaryItemCommand>().InTransientScope();
            Bind<ICreateMeasurementMapDictionaryVersionCommand>().To<CreateMeasurementMapDictionaryVersionCommand>().InTransientScope();
            Bind<IDeleteMeasurementMapDictionaryItemCommand>().To<DeleteMeasurementMapDictionaryItemCommand>().InTransientScope();
            Bind<ISaveLookupItemCommand>().To<SaveLookupItemCommand>().InTransientScope();
            Bind<IUpdateMeasurementMapDictionaryItemCommand>().To<UpdateMeasurementMapDictionaryItemCommand>().InTransientScope();
            Bind<IGetAllMeasurementMapDictionariesQuery>().To<GetAllMeasurementMapDictionariesQuery>().InTransientScope();
            Bind<IGetLookupItemsQuery>().To<GetLookupItemsQuery>().InTransientScope();
            Bind<IGetMeasurementMapDictionaryByIdQuery>().To<GetMeasurementMapDictionaryByIdQuery>().InTransientScope();
            Bind<IGetMeasurementsMapTypeFormsQuery>().To<GetMeasurementsMapTypeFormsQuery>().InTransientScope();
            Bind<MeasurementMapDictionaryStore>().ToSelf().InTransientScope();
            Bind<LookupStore>()
                .ToMethod(ctx => new LookupStore(
                    LookupItemKind.PossibleDefect,
                    ctx.Kernel.Get<IGetLookupItemsQuery>(),
                    ctx.Kernel.Get<ISaveLookupItemCommand>(),
                    ctx.Kernel.Get<ICreateLookupItemCommand>()))
                .InSingletonScope()
                .Named("PossibleDefect");
            Bind<LookupStore>()
                .ToMethod(ctx => new LookupStore(
                    LookupItemKind.NominalValue,
                    ctx.Kernel.Get<IGetLookupItemsQuery>(),
                    ctx.Kernel.Get<ISaveLookupItemCommand>(),
                    ctx.Kernel.Get<ICreateLookupItemCommand>()))
                .InSingletonScope()
                .Named("NominalValue");
            Bind<LookupStore>()
                .ToMethod(ctx => new LookupStore(
                    LookupItemKind.RecommendedRepairMethod,
                    ctx.Kernel.Get<IGetLookupItemsQuery>(),
                    ctx.Kernel.Get<ISaveLookupItemCommand>(),
                    ctx.Kernel.Get<ICreateLookupItemCommand>()))
                .InSingletonScope()
                .Named("RecommendedRepairMethod");
            Bind<LookupStore>()
                .ToMethod(ctx => new LookupStore(
                    LookupItemKind.RequirementPostRepair,
                    ctx.Kernel.Get<IGetLookupItemsQuery>(),
                    ctx.Kernel.Get<ISaveLookupItemCommand>(),
                    ctx.Kernel.Get<ICreateLookupItemCommand>()))
                .InSingletonScope()
                .Named("RequirementPostRepair");
            Bind<MeasurementMapDictionaryListWindowFactory>()
                .ToSelf()
                .InSingletonScope();

            Bind<IReportGenerationLogCommand>().To<ReportGenerationLogCommand>().InTransientScope();
            Bind<IReportExecutionLogger>().To<ReportExecutionLogger>().InTransientScope();

            Bind<ChangesFinalDecisionReport>().ToSelf().InTransientScope();
            Bind<DefectListAllItemsReport>().ToSelf().InTransientScope();
            Bind<DefectListItemsChangesReport>().ToSelf().InTransientScope();
            Bind<DefectListPurchaseItemsReport>().ToSelf().InTransientScope();
            Bind<DefectListScrapItemsReport>().ToSelf().InTransientScope();
            Bind<ExportCreatedRouteMapsReport>().ToSelf().InTransientScope();
            Bind<GetItemInfoByAllProductsReport>().ToSelf().InTransientScope();
            Bind<SummaryByOrdersReport>().ToSelf().InTransientScope();
            Bind<MeasurementMapReport>().ToSelf().InTransientScope();

            Bind<IChangesFinalDecisionReport>().To<ChangesFinalDecisionReportLoggingDecorator>().InTransientScope();
            Bind<IAuxiliaryMaterialsReport>().To<AuxiliaryMaterialsReportLoggingDecorator>().InTransientScope();
            Bind<IDefectListAllItemsReport>().To<DefectListAllItemsReportLoggingDecorator>().InTransientScope();
            Bind<IDefectListItemsChangesReport>().To<DefectListItemsChangesReportLoggingDecorator>().InTransientScope();
            Bind<IDefectListPurchaseItemsReport>().To<DefectListPurchaseItemsReportLoggingDecorator>().InTransientScope();
            Bind<IDefectListScrapItemsReport>().To<DefectListScrapItemsReportLoggingDecorator>().InTransientScope();
            Bind<IExportCreatedRouteMapsReport>().To<ExportCreatedRouteMapsReportLoggingDecorator>().InTransientScope();
            Bind<IGetItemInfoByAllProductsReport>().To<GetItemInfoByAllProductsReportLoggingDecorator>().InTransientScope();
            Bind<ISummaryByOrdersReport>().To<SummaryByOrdersReportLoggingDecorator>().InTransientScope();
            Bind<IMeasurementMapReport>().To<MeasurementMapReportLoggingDecorator>().InTransientScope();
            Bind<IDefectListItemsReport>().To<DefectListItemsReportLoggingDecorator>().InTransientScope();
            Bind<IDefectListItemsReclamationReport>().To<DefectListItemsReclamationReportLoggingDecorator>().InTransientScope();

            BindExternalData();

            Bind<ProductsStore>().ToSelf().InTransientScope();
            Bind<TehprocHeadersStore>().ToSelf().InTransientScope();

            Bind<IBomItemsCloner>().To<BomItemsCloner>().InTransientScope();
            Bind<IBomItemsValidator>().To<BomItemsValidator>().InTransientScope();
            Bind<IBomItemsLoader>().To<BomItemsLoader>().InTransientScope();
            Bind<IBomItemsEditor>().To<BomItemsEditor>().InTransientScope();
            Bind<ILoadBomItemsToBomHeaderCommand>().To<LoadBomItemsToBomHeaderCommand>().InTransientScope();
            Bind<DefectListHeaderViewModel>().ToMethod(LoadDefectListHeaderViewModel).InTransientScope();

            Bind<DefectListHeaderWindow>().ToMethod(ctx =>
            {
                var vm = ctx.Kernel.Get<DefectListHeaderViewModel>();
                var window = new DefectListHeaderWindow() { DataContext = vm };

                window.Closed += (s, e) =>
                {
                    (vm as IDisposable).Dispose();

                    ctx.Kernel.Release(window);
                    ctx.Kernel.Release(vm);

                    window = null;
                    vm = null;
                };

                return window;
            }).InTransientScope();

            Bind<DefectListItemViewModel>().ToMethod(LoadDefectListItemViewModel);

            Bind<DefectListItemWindow>().ToMethod(ctx => new DefectListItemWindow()
            {
                DataContext = ctx.Kernel.Get<DefectListItemViewModel>()
            }).InTransientScope();
        }

        private void BindExternalData()
        {
            // IAsupBomContextFactory - состав ремонтного изделия на дату
            // IGetAllAuxiliaryMaterialDtoQuery - плановые вспомогательные материалы
            // IGetAllIsExpensiveClassifierQuery - дорогостоящая номенклатура из классификатора "ПМ Контроль"
            // IGetAllOgmetMatlDtoQuery - норма расхода материалов ОГМет
            // IGetAllPlanOperationDtoQuery - плановые операции из техпроцессов АСУП
            // IGetAllProductDtoQuery - справочник "Номенклатура" в АСУП
            // IGetAllRouteChartsTrackInfoQuery - получение информации о движении МК в ПМ Контроль
            // IGetAllTehprocHeaderDtoQuery - наличие техпроцессов (ремонт или замена)
            // IGetAllWpDtoQuery - справочник "Подразделения" в ПМ Контроль
            // IRouteMapFactory - сервис для создания МК в АСУП
            // IGetAllDistinctShopEntriesQuery - расцеховка по технологическому маршруту в АСУП
            // IGetAllRouteMapDtoQuery - маршрутные карты АСУП

            var isUseFakeExternalData = true;
            if (isUseFakeExternalData)
            {
                Bind<IAsupBomContextFactory>().To<FakeAsupBomContextFactory>().InTransientScope();
                Bind<IGetAllAuxiliaryMaterialDtoQuery>().To<FakeGetAllAuxiliaryMaterialDtoQuery>();
                Bind<IGetAllIsExpensiveClassifierQuery>().To<FakeGetAllIsExpensiveClassifierQuery>().InTransientScope();
                Bind<IGetAllOgmetMatlDtoQuery>().To<FakeGetAllOgmetMatlDtoQuery>();
                Bind<IGetAllPlanOperationDtoQuery>().To<FakeGetAllPlanOperationDtoQuery>();
                Bind<IGetAllProductDtoQuery>().To<FakeGetAllProductDtoQuery>().InTransientScope();
                Bind<IGetAllRouteChartsTrackInfoQuery>().To<FakeGetAllRouteChartsTrackInfoQuery>().InTransientScope();
                Bind<IGetAllTehprocHeaderDtoQuery>().To<FakeGetAllTehprocHeaderDtoQuery>().InTransientScope();
                Bind<IGetAllWpDtoQuery>().To<FakeGetAllWpDtoQuery>().InTransientScope();
                Bind<IRouteMapFactory>().To<FakeRouteMapFactory>().InTransientScope();
                Bind<IGetAllDistinctShopEntriesQuery>().To<FakeGetAllDistinctShopEntriesQuery>().InTransientScope();
                Bind<IGetAllRouteMapDtoQuery>().To<FakeGetAllRouteMapDtoQuery>().InTransientScope();
            }
        }

        private DefectListHeaderViewModel LoadDefectListHeaderViewModel(IContext ctx)
        {
            return DefectListHeaderViewModel.LoadViewModel(
                ctx.Kernel.Get<BomHeadersStore>(),
                ctx.Kernel.Get<BomItemsStore>(),
                ctx.Kernel.Get<SelectedBomHeaderStore>(),
                ctx.Kernel.Get<BomHeaderSubscribersStore>(),
                ctx.Kernel.Get<RootItemsStore>(),
                ctx.Kernel.Get<OpenedBomHeadersStore>(),
                ctx.Kernel.Get<ProductsStore>(),
                ctx.Kernel.Get<MeasurementMapDictionaryListWindowFactory>()
            );
        }

        private DefectListItemViewModel LoadDefectListItemViewModel(IContext ctx)
        {
            return DefectListItemViewModel.LoadViewModel(
                ctx.Kernel.Get<BomHeadersStore>(),
                ctx.Kernel.Get<SelectedBomItemStore>(),
                ctx.Kernel.Get<BomItemsStore>(),
                ctx.Kernel.Get<DefectToDecisionMapsStore>(),
                ctx.Kernel.Get<SelectedDefectToDecisionMapStore>(),
                ctx.Kernel.Get<ProductsStore>(),
                ctx.Kernel.Get<TehprocHeadersStore>(),
                ctx.Kernel.Get<ISaveBomItemCommand>(),
                ctx.Kernel.Get<IBomItemsValidator>(),
                ctx.Kernel.Get<MeasurementMapStore>(),
                ctx.Kernel.Get<IMeasurementMapItemPresenterService>()
            );
        }
    }
}