using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Report
{
    public class GetItemInfoByAllProductsReport
    {
        public void Create(IEnumerable<BomHeader> bomHeaders, IEnumerable<IBomItem> bomItems, string pathToReportDirectory)
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Style.Font.FontName = "Arial";
                workbook.Style.Font.FontSize = 10;
                var ws = workbook.Worksheets.Add("Инфо о ДСЕ");

                CreateHeader(ws);
                CreateBody(ws, bomHeaders, bomItems);
                PostFormatSheet(ws);

                workbook.SaveAs(pathToReportDirectory + $@"\Анализ номенклатуры от {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.xlsx");
            }
        }

        private void CreateBody(IXLWorksheet ws, IEnumerable<BomHeader> bomHeaders, IEnumerable<IBomItem> bomItems)
        {
            var bomHeadersDict = bomHeaders.ToDictionary(x => x.BomId);
            var row = 2;

            foreach (var bomItem in bomItems)
            {
                var header = bomHeadersDict[bomItem.BomId];
                var col = 1;

                ws.Cell(row, col++).SetValue(bomItem.Detal);
                ws.Cell(row, col++).SetValue(bomItem.DetalIma);
                ws.Cell(row, col++).SetValue(bomItem.DetalTyp);
                ws.Cell(row, col++).SetValue(bomItem.DetalUm);

                ws.Cell(row, col++).SetValue(bomItem.Defect);
                ws.Cell(row, col++).SetValue(bomItem.FinalDecision);
                ws.Cell(row, col++).SetValue(bomItem.QtyRestore);
                ws.Cell(row, col++).SetValue(bomItem.QtyReplace);

                ws.Cell(row, col++).SetValue(header.Contract);
                ws.Cell(row, col++).SetValue(header.Orders);
                ws.Cell(row, col++).SetValue(header.RootItem.Izdel);
                ws.Cell(row, col++).SetValue(header.StateInfo.ApprovalState);

                ws.Cell(row, col++).SetValue(string.Join(";", bomItem.MapBomItemToRouteCharts?.Select(x => x.RouteChart_Number).ToList() ?? new List<string>()));

                row++;
            }
        }

        private void CreateHeader(IXLWorksheet ws)
        {
            var columnNames = new List<string>()
            {
                "Обозначение ДСЕ",
                "Наименование ДСЕ",
                "Тип ДСЕ",
                "Ед.изм. ДСЕ",
                "Дефекты",
                "Окончательное решение",
                "Кол-во ремонт",
                "Кол-во замена",
                "Договор",
                "Заказ",
                "Ремонтное изделие",
                "Статус ведомости",
                "Запущенные МК (из ДВ)"
            };

            var i = 1;
            foreach (var column in columnNames)
            {
                ws.Cell(1, i).Value = column;
                ws.Cell(1, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(1, i).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Cell(1, i).Style.Font.Bold = true;
                ws.Cell(1, i).Style.Alignment.WrapText = true;
                i++;
            }
        }

        private void PostFormatSheet(IXLWorksheet ws)
        {
            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(1);

            var numberLastColumn = ws.LastColumnUsed().ColumnNumber();
            var numberLastRow = ws.LastRowUsed().RowNumber();

            var range = ws.Range(1, 1, numberLastRow, numberLastColumn);
            range.SetAutoFilter();
        }
    }
}