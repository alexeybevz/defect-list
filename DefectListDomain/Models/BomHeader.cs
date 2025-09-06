using System;
using System.Collections.Generic;

namespace DefectListDomain.Models
{
    public class BomHeader : IBomHeader
    {
        public int BomId { get; set; }
        public string Orders { get; set; }
        public string SerialNumber { get; set; }
        public string SerialNumberAfterRepair { get; set; }
        public RootItem RootItem { get; set; }
        public int IzdelQty { get; set; }
        public int StateDetalsId { get; set; }
        public string Comment { get; set; }
        public decimal TotalRowsCount { get; set; }
        public decimal FilledRowsCount { get; set; }
        public DateTime DateOfSpecif { get; set; }
        public DateTime DateOfTehproc { get; set; }
        public DateTime DateOfMtrl { get; set; }
        public DateTime? DateOfPreparation { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime RecordDate { get; set; }
        public BomHeaderState State { get; set; }
        public string Contract { get; set; }
        public DateTime ContractDateOpen { get; set; }
        public BomHeaderStateInfo StateInfo { get; set; }
        public DateTime? CreateDateFirstRouteMap { get; set; }
        public string HeaderType { get; set; }
    }

    public interface IBomHeader
    {
        int BomId { get; set; }
        string Orders { get; set; }
        string SerialNumber { get; set; }
        string SerialNumberAfterRepair { get; set; }
        RootItem RootItem { get; set; }
        int IzdelQty { get; set; }
        int StateDetalsId { get; set; }
        string Comment { get; set; }
        decimal TotalRowsCount { get; set; }
        decimal FilledRowsCount { get; set; }
        DateTime DateOfSpecif { get; set; }
        DateTime DateOfTehproc { get; set; }
        DateTime DateOfMtrl { get; set; }
        DateTime? DateOfPreparation { get; set; }
        string CreatedBy { get; set; }
        string CreatedByName { get; set; }
        DateTime CreateDate { get; set; }
        string UpdatedBy { get; set; }
        string UpdatedByName { get; set; }
        DateTime RecordDate { get; set; }
        BomHeaderState State { get; set; }
        string Contract { get; set; }
        DateTime ContractDateOpen { get; set; }
        BomHeaderStateInfo StateInfo { get; set; }
        DateTime? CreateDateFirstRouteMap { get; set; }
        string HeaderType { get; set; }
    }

    public class BomHeaderStateInfo
    {
        private readonly BomHeaderState _state;
        private readonly decimal _totalRowsCount;
        private readonly decimal _filledRowsCount;

        public BomHeaderStateInfo(BomHeaderState state, decimal totalRowsCount, decimal filledRowsCount)
        {
            _state = state;
            _totalRowsCount = totalRowsCount;
            _filledRowsCount = filledRowsCount;
        }

        public bool IsWip => _state == BomHeaderState.WorkInProgress && FillPercentage < 100;
        public bool IsWaitApproved => _state == BomHeaderState.WorkInProgress && FillPercentage == 100;
        public bool IsApproved => _state == BomHeaderState.Approved && FillPercentage == 100;
        public bool IsClosed => _state == BomHeaderState.Closed;

        public string ApprovalState
        {
            get
            {
                if (IsApproved)
                    return BomHeaderApprovalStates[BomHeaderApprovalState.Approved];
                if (IsWip)
                    return BomHeaderApprovalStates[BomHeaderApprovalState.Wip];
                if (IsWaitApproved)
                    return BomHeaderApprovalStates[BomHeaderApprovalState.WaitApproved];
                if (IsClosed)
                    return BomHeaderApprovalStates[BomHeaderApprovalState.Closed];
                return BomHeaderApprovalStates[BomHeaderApprovalState.Error];
            }
        }

        public decimal FillPercentage => _totalRowsCount == 0 ? 0 : Math.Floor((_filledRowsCount / _totalRowsCount) * (decimal)100.0);
        public string FillPercentageText => $"{FillPercentage}%";

        private readonly Dictionary<BomHeaderApprovalState, string> BomHeaderApprovalStates =
            new Dictionary<BomHeaderApprovalState, string>()
            {
                { BomHeaderApprovalState.Approved, "Утверждено" },
                { BomHeaderApprovalState.Closed, "Закрыто" },
                { BomHeaderApprovalState.WaitApproved, "Ожидает утверждения" },
                { BomHeaderApprovalState.Wip, "В работе" },
                { BomHeaderApprovalState.Error, "Ошибка определения статуса" },
            };
    }

    public enum BomHeaderApprovalState
    {
        Approved,
        Closed,
        WaitApproved,
        Wip,
        Error
    }
}