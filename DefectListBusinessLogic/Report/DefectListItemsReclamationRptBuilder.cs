using System;
using System.Collections.Generic;
using System.Linq;
using DefectListDomain.ReportParameters;
using DefectListDomain.Reports;
using ReporterBusinessLogic;
using ReporterDomain.Services.CreateReportService;

namespace DefectListBusinessLogic.Report
{
    public class DefectListItemsReclamationRptBuilder : IReportBuilder<DefectListItemsReclamation>
    {
        private readonly IParmBuilder<DefectListItemsRptParm> _parmBuilder;

        public DefectListItemsReclamationRptBuilder(IParmBuilder<DefectListItemsRptParm> parmBuilder)
        {
            _parmBuilder = parmBuilder;
        }
        public List<DefectListItemsReclamation> Create()
        {
            var parms = _parmBuilder.Create();
            var reports = new List<DefectListItemsReclamation>();
            parms.ForEach(p => CreateReport(reports, p));
            return reports;
        }

        private void CreateReport(List<DefectListItemsReclamation> reports, DefectListItemsRptParm parm)
        {
            var dt = parm.BomItems.ToList().ToDataTable();

            var dtReport = new DataSetReport.DefectListItemsReportDataTable();
            dtReport.Merge(dt);

            string reportName = $@"Ведомость. Рекламация на {parm.Izdel}. {parm.SerialNumber} от {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}";

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