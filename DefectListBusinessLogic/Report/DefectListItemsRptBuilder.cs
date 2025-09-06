using System;
using System.Collections.Generic;
using System.Linq;
using DefectListDomain.ReportParameters;
using DefectListDomain.Reports;
using ReporterBusinessLogic;
using ReporterDomain.Services.CreateReportService;

namespace DefectListBusinessLogic.Report
{
    public class DefectListItemsRptBuilder : IReportBuilder<DefectListItems>
    {
        private readonly IParmBuilder<DefectListItemsRptParm> _parmBuilder;

        public DefectListItemsRptBuilder(IParmBuilder<DefectListItemsRptParm> parmBuilder)
        {
            _parmBuilder = parmBuilder;
        }
        public List<DefectListItems> Create()
        {
            var parms = _parmBuilder.Create();
            var reports = new List<DefectListItems>();
            parms.ForEach(p => CreateReport(reports, p));
            return reports;
        }

        private void CreateReport(List<DefectListItems> reports, DefectListItemsRptParm parm)
        {
            if (parm.IsUseFinalDecision)
            {
                var countRowsWithEmptyFinalDecision =
                    parm.BomItems.Count(x => string.IsNullOrEmpty(x.FinalDecision?.Trim()));
                if (countRowsWithEmptyFinalDecision > 0)
                    throw new ArgumentException(
                        "Формирование отчета отменено, т.к. в составе присутствуют позиции с незаполненным полем 'Окончательное решение по устранению'.");

                var rowsWithEmptySerialNumber = parm.BomItems.Where(x =>
                    x.IsExpensive && string.IsNullOrEmpty(x.SerialNumber?.Trim()) &&
                    x.FinalDecision?.Trim() == "заменить").ToList();
                if (rowsWithEmptySerialNumber.Count > 0)
                {
                    var str = string.Join(Environment.NewLine,
                        rowsWithEmptySerialNumber.Select(x => x.Detal).ToArray());
                    throw new ArgumentException(
                        "Формирование отчета отменено, т.к. в составе присутствуют позиции с окончательным решением 'заменить', у которых не заполнено поле 'Серийный номер':\n\n" +
                        str);
                }
            }

            var dt = parm.BomItems.ToList().ToDataTable();

            var dtReport = new DataSetReport.DefectListItemsReportDataTable();
            dtReport.Merge(dt);

            string reportName = $@"Дефектовочная ведомость. {parm.Izdel}. {parm.SerialNumber} от {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}";

            reports.AddIfHasRecords(dt, reportName,
                new Dictionary<string, object>()
                {
                    { "SerialNumber", parm.SerialNumber ?? string.Empty },
                    { "Izdel", parm.IzdelInitial ?? string.Empty },
                    { "Contract", parm.Contract ?? string.Empty },
                    { "ContractDateOpen", parm.ContractDateOpen },
                    { "IsUseFinalDecision", parm.IsUseFinalDecision },
                    { "DateOfPreparation", parm.DateOfPreparation },
                });
        }
    }
}