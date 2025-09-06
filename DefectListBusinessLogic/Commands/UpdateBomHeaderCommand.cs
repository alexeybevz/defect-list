using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class UpdateBomHeaderCommand : DbConnectionPmControlRepositoryBase, IUpdateBomHeaderCommand
    {
        public UpdateBomHeaderCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(BomHeader bomHeader)
        {
            var parameters = new
            {
                BomId = bomHeader.BomId,
                Orders = bomHeader.Orders,
                SerialNumber = bomHeader.SerialNumber,
                SerialNumberAfterRepair = bomHeader.SerialNumberAfterRepair,
                RootItemId = bomHeader.RootItem.Id,
                IzdelQty = bomHeader.IzdelQty,
                StateDetalsId = 1,
                Comment = bomHeader.Comment,
                DateOfSpecif = bomHeader.DateOfSpecif,
                DateOfTehproc = bomHeader.DateOfTehproc,
                DateOfMtrl = bomHeader.DateOfTehproc,
                DateOfPreparation = bomHeader.DateOfPreparation,
                RecordDate = bomHeader.RecordDate,
                UpdatedBy = bomHeader.UpdatedBy,
                Contract = bomHeader.Contract,
                ContractDateOpen = bomHeader.ContractDateOpen,
                HeaderType = bomHeader.HeaderType,
            };

            try
            {
                using (var db = await CreateOpenConnectionAsync())
                {
                    var query = @"
                    UPDATE dbo.BomHeader
                    SET Orders = @Orders,
                        SerialNumber = @SerialNumber,
                        SerialNumberAfterRepair = @SerialNumberAfterRepair,
                        RootItemId = @RootItemId,
                        IzdelQty = @IzdelQty,
                        StateDetalsId = @StateDetalsId,
                        Comment = @Comment,
                        DateOfSpecif = @DateOfSpecif,
                        DateOfTehproc = @DateOfTehproc,
                        DateOfMtrl = @DateOfMtrl,
                        DateOfPreparation = @DateOfPreparation,
                        RecordDate = @RecordDate,
                        UpdatedBy = @UpdatedBy,
                        Contract = @Contract,
                        ContractDateOpen = @ContractDateOpen,
                        HeaderType = @HeaderType
                    WHERE BomId = @BomId
                    ";

                    using (var transaction = db.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        await db.ExecuteAsync(query, parameters, transaction);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("IX_BomHeader_Orders"))
                    throw new InvalidOperationException($"Дефектовочная ведомость с заказом '{bomHeader.Orders}' уже существует");

                if (e.Message.Contains("IX_BomHeader_SerialNumber"))
                    throw new InvalidOperationException($"Дефектовочная ведомость с серийным номером '{bomHeader.SerialNumber}' уже существует");

                if (e.Message.Contains("FK_BomHeader_RootItem"))
                    throw new InvalidOperationException($"Указанное ремонтное изделие '{bomHeader.RootItem.Izdel}' отсутствует в справочнике. Выполнение данной операции прервано.");

                throw;
            }
        }
    }
}