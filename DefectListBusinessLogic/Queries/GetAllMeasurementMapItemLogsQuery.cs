using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllMeasurementMapItemLogsQuery : DbConnectionPmControlRepositoryBase, IGetAllMeasurementMapItemLogsQuery
    {
        public GetAllMeasurementMapItemLogsQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<IReadOnlyCollection<MeasurementMapItemLog>> ExecuteByMeasurementMapIdAsync(int measurementMapId)
        {
            const string sql = @"
                SELECT
                    i.Id,
                    i.MeasurementMapId,
                    i.MeasurementMapItemId,
                    i.[Action],
                    i.CustomNumeration,
                    i.SortOrder,
                    i.PossibleDefectId,
                    i.PossibleDefectName,
                    i.NominalValueId,
                    i.NominalValueName,
                    i.RecommendedRepairMethodId,
                    i.RecommendedRepairMethodName,
                    i.RequirementPostRepairId,
                    i.RequirementPostRepairName,
                    i.ActualValue,
                    i.ActualRecommendedRepairMethodId,
                    i.ActualRecommendedRepairMethodName,
                    i.MarkOfWorkCompletion,
                    i.MarkOfWorkCompletionBy,
                    u2.ActiveDirectoryCN AS MarkOfWorkCompletionByName,
                    i.CreateDate,
                    i.CreatedBy,
                    u.ActiveDirectoryCN AS CreatedByName,
                    i.AlternateNominalValueId,
                    i.AlternateNominalValueName,
                    i.ItemType
                FROM MeasurementMapItemLog i
                LEFT JOIN Users u ON u.[Login] = i.CreatedBy
                LEFT JOIN Users u2 ON u2.[Login] = i.MarkOfWorkCompletionBy
                WHERE i.MeasurementMapId = @MeasurementMapId
                ORDER BY i.SortOrder, i.Id";

            using (var db = await CreateOpenConnectionAsync())
            {
                return (await db.QueryAsync<MeasurementMapItemLog>(sql, new { MeasurementMapId = measurementMapId })).ToList();
            }
        }

        public async Task<IReadOnlyCollection<MeasurementMapItemLog>> ExecuteByBomItemIdAsync(int bomItemId)
        {
            const string sql = @"
                SELECT
                    i.Id,
                    i.MeasurementMapId,
                    i.MeasurementMapItemId,
                    i.[Action],
                    i.CustomNumeration,
                    i.SortOrder,
                    i.PossibleDefectId,
                    i.PossibleDefectName,
                    i.NominalValueId,
                    i.NominalValueName,
                    i.RecommendedRepairMethodId,
                    i.RecommendedRepairMethodName,
                    i.RequirementPostRepairId,
                    i.RequirementPostRepairName,
                    i.ActualValue,
                    i.ActualRecommendedRepairMethodId,
                    i.ActualRecommendedRepairMethodName,
                    i.MarkOfWorkCompletion,
                    i.MarkOfWorkCompletionBy,
                    u2.ActiveDirectoryCN AS MarkOfWorkCompletionByName,
                    i.CreateDate,
                    i.CreatedBy,
                    u.ActiveDirectoryCN AS CreatedByName
                FROM MeasurementMap m
                INNER JOIN MeasurementMapItemLog i ON i.MeasurementMapId = m.Id 
                LEFT JOIN Users u ON u.[Login] = i.CreatedBy
                LEFT JOIN Users u2 ON u2.[Login] = i.MarkOfWorkCompletionBy
                WHERE m.BomItemId = @BomItemId";

            using (var db = await CreateOpenConnectionAsync())
            {
                return (await db.QueryAsync<MeasurementMapItemLog>(sql, new { BomItemId = bomItemId })).ToList();
            }
        }
    }
}