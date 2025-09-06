using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class CreateBomHeaderCommand : DbConnectionPmControlRepositoryBase, ICreateBomHeaderCommand
    {
        public CreateBomHeaderCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(BomHeader bomHeader)
        {
            var parameters = new
            {
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
                CreateDate = bomHeader.CreateDate,
                CreatedBy = bomHeader.CreatedBy,
                RecordDate = bomHeader.RecordDate,
                UpdatedBy = bomHeader.CreatedBy,
                Contract = bomHeader.Contract,
                ContractDateOpen = bomHeader.ContractDateOpen,
                HeaderType = bomHeader.HeaderType,
            };

            try
            {
                using (var db = await CreateOpenConnectionAsync())
                {
                    var query = @"
                INSERT INTO dbo.BomHeader (Orders, SerialNumber, SerialNumberAfterRepair, RootItemId, IzdelQty, StateDetalsId, Comment, DateOfSpecif, DateOfTehproc, DateOfMtrl, DateOfPreparation, CreateDate, CreatedBy, RecordDate, UpdatedBy, Contract, ContractDateOpen, HeaderType)
                VALUES (@Orders, @SerialNumber, @SerialNumberAfterRepair, @RootItemId, @IzdelQty, @StateDetalsId, @Comment, @DateOfSpecif, @DateOfTehproc, @DateOfMtrl, @DateOfPreparation, @CreateDate, @CreatedBy, @RecordDate, @UpdatedBy, @Contract, @ContractDateOpen, @HeaderType)
                SELECT CAST(SCOPE_IDENTITY() AS int)
                ";

                    using (var transaction = db.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        bomHeader.BomId = await db.QuerySingleAsync<int>(query, parameters, transaction);
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