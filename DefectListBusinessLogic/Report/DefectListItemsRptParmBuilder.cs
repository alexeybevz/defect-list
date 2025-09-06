using System.Collections.Generic;
using DefectListDomain.ReportParameters;
using ReporterDomain.Services.CreateReportService;

namespace DefectListBusinessLogic.Report
{
    public class DefectListItemsRptParmBuilder : IParmBuilder<DefectListItemsRptParm>
    {
        private readonly DefectListItemsRptParm _rptParm;

        public DefectListItemsRptParmBuilder(DefectListItemsRptParm rptParm)
        {
            _rptParm = rptParm;
        }

        public List<DefectListItemsRptParm> Create()
        {
            return new List<DefectListItemsRptParm>(){ _rptParm };
        }
    }
}