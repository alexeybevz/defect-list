using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    public class CopyMeasurementMapDictionaryCommand : DbConnectionPmControlRepositoryBase, ICopyMeasurementMapDictionaryCommand
    {
        private readonly IGetMeasurementMapDictionaryByIdQuery _getMeasurementMapDictionaryByIdQuery;
        private readonly ICreateMeasurementMapDictionaryCommand _createMeasurementMapDictionaryCommand;
        private readonly ICreateMeasurementMapDictionaryItemCommand _createMeasurementMapDictionaryItemCommand;

        public CopyMeasurementMapDictionaryCommand(
            IDbConnectionFactory dbConnectionFactory,
            IGetMeasurementMapDictionaryByIdQuery getMeasurementMapDictionaryByIdQuery,
            ICreateMeasurementMapDictionaryCommand createMeasurementMapDictionaryCommand,
            ICreateMeasurementMapDictionaryItemCommand createMeasurementMapDictionaryItemCommand) : base(dbConnectionFactory)
        {
            _getMeasurementMapDictionaryByIdQuery = getMeasurementMapDictionaryByIdQuery;
            _createMeasurementMapDictionaryCommand = createMeasurementMapDictionaryCommand;
            _createMeasurementMapDictionaryItemCommand = createMeasurementMapDictionaryItemCommand;
        }

        public async Task<int> ExecuteAsync(int sourceMeasurementMapDictionaryId, MeasurementMapDictionary targetDictionary, IReadOnlyList<RootItem> rootItems, string createdBy)
        {
            var sourceDictionary = await _getMeasurementMapDictionaryByIdQuery.ExecuteAsync(sourceMeasurementMapDictionaryId);
            if (sourceDictionary == null)
                throw new InvalidOperationException($"Активный справочник карты измерения Id={sourceMeasurementMapDictionaryId} не найден.");

            var createdDictionary = await _createMeasurementMapDictionaryCommand.ExecuteAsync(targetDictionary, rootItems, createdBy);

            foreach (var item in sourceDictionary.Items)
            {
                item.Id = 0;
                item.MeasurementMapDictionaryId = createdDictionary.Id;

                var alternatives = item.RepairMethodAlternatives?.ToList() ?? new List<RecommendedRepairMethodAlternative>();

                await _createMeasurementMapDictionaryItemCommand.ExecuteAsync(item, alternatives, createdBy);
            }

            return createdDictionary.Id;
        }
    }
}