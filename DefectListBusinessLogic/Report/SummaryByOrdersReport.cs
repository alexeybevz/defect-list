using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using ClosedXML.Excel;
using DefectListDomain.Models;
using DefectListDomain.Dtos;

namespace DefectListBusinessLogic.Report
{
    public class SummaryByOrdersReport
    {
        public void Create(IEnumerable<BomHeader> bomHeaders, IEnumerable<IBomItem> items, IEnumerable<ProductDto> products, string pathToReportDirectory)
        {
            var itemsGrpByDetal = items
                .GroupBy(x => new { x.ProductID })
                .Select(x => new BomItem
                {
                    ProductID = x.Key.ProductID,
                    QtyMnf = x.Sum(b => b.QtyMnf),
                    QtyRestore = x.Sum(b => b.QtyRestore),
                    QtyReplace = x.Sum(b => b.QtyReplace),
                    Detal = x.FirstOrDefault()?.Detal,
                    DetalIma = x.FirstOrDefault()?.DetalIma,
                    DetalTyp = x.FirstOrDefault()?.DetalTyp,
                    DetalUm = x.FirstOrDefault()?.DetalUm,
                }).ToList();

            var itemsGrpByDetalAndBomId = items
                .GroupBy(x => new { x.BomId, x.ProductID })
                .Select(x => new
                {
                    x.Key.BomId,
                    x.Key.ProductID,
                    QtyMnf = x.Sum(b => b.QtyMnf),
                    QtyRestore = x.Sum(b => b.QtyRestore),
                    QtyReplace = x.Sum(b => b.QtyReplace),
                    bomHeaders.FirstOrDefault(h => h.BomId == x.Key.BomId)?.Orders,
                    SerialNumbers = string.Join(";", x.Where(z => !string.IsNullOrEmpty(z.SerialNumber)).Select(z => z.SerialNumber).ToList())
                }).ToList();

            var ci = new CultureInfo("ru", false);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Style.Font.FontName = "Arial";
                workbook.Style.Font.FontSize = 10;
                IXLWorksheet worksheet = workbook.Worksheets.Add("Сводный отчет по заказам");
                IXLWorksheet worksheet2 = workbook.Worksheets.Add("Заказы");

                var row = 1;
                string[] headers =
                {
                    "18-й код",
                    "Обозначение",
                    "Наименование",
                    "Тип",
                    "ЕИ",
                    "Общий план",
                    "Соответствует КД",
                    "Ремонт",
                    "Замена",
                    "Заказ",
                    "Заводской номер"
                };

                var col = 1;
                foreach (var header in headers)
                {
                    worksheet.Cell(row, col).SetValue(header);
                    worksheet.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    worksheet.Cell(row, col).Style.Font.Bold = true;
                    col++;
                }

                foreach (var item in itemsGrpByDetal.OrderBy(x => x.ProductID))
                {
                    var product = products.FirstOrDefault(p => p.Id == item.ProductID);
                    var isExistsProduct = product != null;

                    row++;
                    col = 0;

                    worksheet.Cell(row, ++col).SetValue(isExistsProduct ? product.CodeErp : null);
                    worksheet.Cell(row, ++col).SetValue(isExistsProduct ? product.Name : item.Detal);
                    worksheet.Cell(row, ++col).SetValue(isExistsProduct ? product.ExtName : item.DetalIma);
                    worksheet.Cell(row, ++col).SetValue(isExistsProduct ? product.Type : item.DetalTyp);
                    worksheet.Cell(row, ++col).SetValue(isExistsProduct ? product.Um : item.DetalUm);
                    worksheet.Cell(row, ++col).SetValue(Math.Round(item.QtyMnf, 3));
                    worksheet.Cell(row, ++col).SetValue(Math.Round(item.QtyMnf - (item.QtyRestore + item.QtyReplace), 3));
                    worksheet.Cell(row, ++col).SetValue(Math.Round(item.QtyRestore, 3));
                    worksheet.Cell(row, ++col).SetValue(Math.Round(item.QtyReplace, 3));

                    int startGroupRowNumber = ++row;
                    var grpRows = itemsGrpByDetalAndBomId.Where(x => x.ProductID == item.ProductID).ToList();
                    foreach (var obj in grpRows)
                    {
                        col = 0;

                        worksheet.Cell(row, ++col).SetValue(isExistsProduct ? product.CodeErp : null);
                        worksheet.Cell(row, ++col).SetValue(isExistsProduct ? product.Name : item.Detal);
                        worksheet.Cell(row, ++col).SetValue(isExistsProduct ? product.ExtName : item.DetalIma);
                        worksheet.Cell(row, ++col).SetValue(isExistsProduct ? product.Type : item.DetalTyp);
                        worksheet.Cell(row, ++col).SetValue(isExistsProduct ? product.Um : item.DetalUm);

                        worksheet.Cell(row, ++col).SetValue(Math.Round(obj.QtyMnf, 3));
                        worksheet.Cell(row, ++col).SetValue(Math.Round(obj.QtyMnf - (obj.QtyRestore + obj.QtyReplace), 3));
                        worksheet.Cell(row, ++col).SetValue(Math.Round(obj.QtyRestore, 3));
                        worksheet.Cell(row, ++col).SetValue(Math.Round(obj.QtyReplace, 3));

                        worksheet.Cell(row, ++col).SetValue(obj.Orders);
                        worksheet.Cell(row, ++col).SetValue(obj.SerialNumbers);

                        row++;
                    }

                    if (grpRows.Any())
                    {
                        int endGroupRowNumber = --row;
                        worksheet.Rows(startGroupRowNumber, endGroupRowNumber).Group();
                        worksheet.Rows(startGroupRowNumber, endGroupRowNumber).Collapse();
                    }
                }

                //worksheet.Columns().AdjustToContents();

                row = 1;
                col = 1;

                string[] headers2 =
                {
                    "Заказ",
                    "Обозначение изделия",
                    "Наименование изделия",
                    "Серийный номер",
                };

                foreach (var header in headers2)
                {
                    worksheet2.Cell(row, col).SetValue(header);
                    worksheet2.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet2.Cell(row, col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    worksheet2.Cell(row, col).Style.Font.Bold = true;
                    col++;
                }

                foreach (var item in bomHeaders)
                {
                    row++;
                    col = 0;

                    worksheet2.Cell(row, ++col).SetValue(item.Orders);
                    worksheet2.Cell(row, ++col).SetValue(item.RootItem.Izdel);
                    worksheet2.Cell(row, ++col).SetValue(item.RootItem.IzdelIma);
                    worksheet2.Cell(row, ++col).SetValue(item.SerialNumber);
                }

                worksheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

                worksheet2.Columns().AdjustToContents();

                workbook.SaveAs(pathToReportDirectory + $@"\Сводный отчет по заказам от {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.xlsx");
            }
        }
    }
}
