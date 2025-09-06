using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using DefectListDomain.Models;
using DefectListDomain.ExternalData;

namespace DefectListBusinessLogic.Report
{
    public class AdditionalMaterialsReport
    {
        private readonly IGetAllAuxiliaryMaterialDtoQuery _getAllAuxiliaryMaterialDtoQuery;
        private readonly IGetAllOgmetMatlDtoQuery _getAllOgmetMatlDtoQuery;

        public AdditionalMaterialsReport(
            IGetAllAuxiliaryMaterialDtoQuery getAllAuxiliaryMaterialDtoQuery,
            IGetAllOgmetMatlDtoQuery getAllOgmetMatlDtoQuery)
        {
            _getAllAuxiliaryMaterialDtoQuery = getAllAuxiliaryMaterialDtoQuery;
            _getAllOgmetMatlDtoQuery = getAllOgmetMatlDtoQuery;
        }

        public void Create(IBomHeader bomHeader, IReadOnlyCollection<BomItem> bomItemsView, string pathToReport)
        {
            var data = GetData(bomItemsView).ToList();

            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Style.Font.FontName = "Arial";
                workbook.Style.Font.FontSize = 10;
                var ws = workbook.Worksheets.Add("Суммарные данные");
                var ws2 = workbook.Worksheets.Add("Суммарные данные по цехам");
                var ws3 = workbook.Worksheets.Add("Расшифровка");
                

                CreateWs(ws, data);
                CreateWs2(ws2, data);
                CreateWs3(ws3, data);

                workbook.SaveAs(pathToReport + $@"\ДВ по {bomHeader.RootItem.Izdel } № { bomHeader.SerialNumber } Плановый расход вспом. матер. (прямые расходы) от {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.xlsx");
            }
        }

        private void CreateWs(IXLWorksheet ws, IEnumerable<MtrlP> data)
        {
            var grpData = data
                .GroupBy(x => new {x.Item, x.UnionName, x.Um})
                .OrderBy(x => x.Key.UnionName)
                .ToList();

            var columnNames = new List<string>()
            {
                "18-й код материала",
                "Объединенное наименование материала",
                "Норма расхода на кол-во дет.",
                "Ед.изм."
            };

            CreateHeader(ws, columnNames);

            var i = 2;
            foreach (var dt in grpData)
            {
                var col = 1;

                ws.Cell(i, col++).SetValue(dt.Key.Item);
                ws.Cell(i, col++).SetValue(dt.Key.UnionName);
                ws.Cell(i, col++).SetValue(dt.Sum(d => d.Qty));
                ws.Cell(i, col++).SetValue(dt.Key.Um);

                i++;
            }

            PostFormatSheet(ws);
        }

        private void CreateWs2(IXLWorksheet ws, IEnumerable<MtrlP> data)
        {
            var grpData = data
                .GroupBy(x => new { x.WpCeh, x.Item, x.UnionName, x.Um })
                .OrderBy(x => x.Key.UnionName)
                .ToList();

            var columnNames = new List<string>()
            {
                "Цех",
                "18-й код материала",
                "Объединенное наименование материала",
                "Норма расхода на кол-во дет.",
                "Ед.изм."
            };

            CreateHeader(ws, columnNames);

            var i = 2;
            foreach (var dt in grpData)
            {
                var col = 1;

                ws.Cell(i, col++).SetValue(dt.Key.WpCeh);
                ws.Cell(i, col++).SetValue(dt.Key.Item);
                ws.Cell(i, col++).SetValue(dt.Key.UnionName);
                ws.Cell(i, col++).SetValue(dt.Sum(d => d.Qty));
                ws.Cell(i, col++).SetValue(dt.Key.Um);

                i++;
            }

            PostFormatSheet(ws);
        }

        private void CreateWs3(IXLWorksheet ws, IEnumerable<MtrlP> data)
        {
            var columnNames = new List<string>()
            {
                "18-й код ДСЕ",
                "Объединенное наименование ДСЕ",
                "Кол-во ДСЕ",
                "18-й код материала",
                "Объединенное наименование материала",
                "Норма расхода на ед.",
                "Норма расхода на кол-во дет.",
                "Ед.изм."
            };

            CreateHeader(ws, columnNames);

            var list = data
                .OrderBy(x => x.ParentUnionName)
                .ThenBy(x => x.UnionName)
                .ToList();

            var i = 2;
            foreach (var dt in list)
            {
                var col = 1;

                ws.Cell(i, col++).SetValue(dt.ParentItem);
                ws.Cell(i, col++).SetValue(dt.ParentUnionName);
                ws.Cell(i, col++).SetValue(dt.ParentQty);
                ws.Cell(i, col++).SetValue(dt.Item);
                ws.Cell(i, col++).SetValue(dt.UnionName);
                ws.Cell(i, col++).SetValue(dt.QtyOnUnit);
                ws.Cell(i, col++).SetValue(dt.Qty);
                ws.Cell(i, col++).SetValue(dt.Um);

                i++;
            }

            PostFormatSheet(ws);
        }

        private void CreateHeader(IXLWorksheet ws, IEnumerable<string> columnNames)
        {
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

        private IEnumerable<MtrlP> GetData(IReadOnlyCollection<BomItem> bomItemsView)
        {
            var bomItems = bomItemsView
                            .Where(x => x.FinalDecision == "заменить" && x.ProductID.HasValue && x.Code_LSF82.HasValue)
                            .GroupBy(x => new { x.ProductID, x.Code_LSF82 })
                            .Select(x => new { x.Key.ProductID, x.Key.Code_LSF82, SumQtyReplace = (decimal)x.Sum(s => s.QtyReplace) })
                            .ToList();

            var auxiliaryMaterials = _getAllAuxiliaryMaterialDtoQuery
                .Execute()
                .Where(x => x.CostAccoutingCategory == "Прямые расходы")
                .ToList();

            var ogmetMatls = _getAllOgmetMatlDtoQuery
                .AskMatls()
                .Where(x => x.MatlTypeDescription == "Вспомогательный")
                .ToList();

            var result = new List<MtrlP>();

            foreach (var bomItem in bomItems)
            {
                var bomItemAuxiliaryMaterials = auxiliaryMaterials.Where(x => x.ParentProductId == bomItem.ProductID).ToList();

                result.AddRange(bomItemAuxiliaryMaterials.Select(x => new MtrlP()
                {
                    ParentCodeLsf82 = x.ParentCodeLsf82,
                    ParentItem = x.ParentItem,
                    ParentUnionName = x.ParentUnionName,
                    ParentQty = bomItem.SumQtyReplace,

                    CodeLsf82 = x.CodeLsf82,
                    Item = x.Item,
                    UnionName = x.UnionName,
                    QtyOnUnit = x.QtyOnUnit,
                    Qty = Math.Round(x.QtyOnUnit * bomItem.SumQtyReplace, 5),
                    Um = x.Um,
                    WpCeh = x.WpCeh
                }).ToList());

                var bomItemOgmetMatls = ogmetMatls.Where(x => x.ParentCodeLsf82 == bomItem.Code_LSF82.Value).ToList();
                result.AddRange(bomItemOgmetMatls.Select(x => new MtrlP()
                {
                    ParentCodeLsf82 = x.ParentCodeLsf82,
                    ParentItem = x.ParentItem,
                    ParentUnionName = x.ParentUnionName,
                    ParentQty = bomItem.SumQtyReplace,

                    CodeLsf82 = x.CodeLsf82,
                    Item = x.Item,
                    UnionName = x.UnionName,
                    QtyOnUnit = x.QtyOnUnit,
                    Qty = Math.Round(x.QtyOnUnit * bomItem.SumQtyReplace, 5),
                    Um = x.Um,
                    WpCeh = x.WpCeh
                }).ToList());
            }

            var mtrls = result.GroupBy(x => new
            {
                x.ParentCodeLsf82,
                x.ParentItem,
                x.ParentUnionName,
                x.ParentQty,
                x.CodeLsf82,
                x.Item,
                x.UnionName,
                x.QtyOnUnit,
                x.Um,
                x.WpCeh
            }).Select(x => new MtrlP()
            {
                ParentCodeLsf82 = x.Key.ParentCodeLsf82,
                ParentItem = x.Key.ParentItem,
                ParentUnionName = x.Key.ParentUnionName,
                ParentQty = x.Key.ParentQty,

                CodeLsf82 = x.Key.CodeLsf82,
                Item = x.Key.Item,
                UnionName = x.Key.UnionName,
                QtyOnUnit = x.Key.QtyOnUnit,
                Qty = x.Sum(s => s.Qty),
                Um = x.Key.Um,
                WpCeh = x.Key.WpCeh
            }).ToList();

            return mtrls;
        }

        private class Mtrl
        {
            public int CodeLsf82 { get; set; }
            public string Item { get; set; }
            public string UnionName { get; set; }
            public decimal QtyOnUnit { get; set; }
            public decimal Qty { get; set; }
            public string Um { get; set; }
            public string WpCeh { get; set; }
        }

        private class MtrlP : Mtrl
        {
            public int ParentCodeLsf82 { get; set; }
            public string ParentItem { get; set; }
            public string ParentUnionName { get; set; }
            public decimal ParentQty { get; set; }
        }
    }
}