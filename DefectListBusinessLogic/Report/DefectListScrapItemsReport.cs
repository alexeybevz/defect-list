using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Report
{
    public class DefectListScrapItemsReport
    {
        public void Create(BomHeader bomHeader, IReadOnlyCollection<BomItem> data, string pathToReportDirectory)
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Style.Font.FontName = "Times New Roman";
                workbook.Style.Font.FontSize = 14;
                IXLWorksheet worksheet = workbook.Worksheets.Add("Отчет о браке");
                CreateTableName(worksheet, bomHeader);
                CreateTableHeader(worksheet);

                int i = 5;
                foreach (BomItem item in data)
                {
                    i = i + 1;
                    worksheet.Cell(i, 1).SetValue(item.Detal);
                    worksheet.Cell(i, 2).SetValue(item.DetalIma);
                    worksheet.Cell(i, 3).SetValue(item.DetalTyp);
                    worksheet.Cell(i, 4).SetValue(Math.Round(item.QtyReplace, 2));
                    worksheet.Cell(i, 5).SetValue(item.DetalUm);
                    worksheet.Cell(i, 6).Value = item.Defect;
                }

                AddCellStyle(worksheet, i);

                CreateTableFooter(worksheet, i);

                IXLSheetView view = worksheet.SheetView;
                view.ZoomScale = 80;
                workbook.SaveAs(pathToReportDirectory + $@"\Отчет о браке ДСЕ по {bomHeader.RootItem.Izdel } № { bomHeader.SerialNumber } от {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.xlsx");
            }
        }

        private void CreateTableFooter(IXLWorksheet worksheet, int lastRowNumber)
        {
            worksheet.Cell(lastRowNumber + 2, 4).Value = "Представитель цеха 87 ______________________________";
            worksheet.Cell(lastRowNumber + 5, 4).Value = "Представитель ОТК     ______________________________";
        }

        private void CreateTableName(IXLWorksheet worksheet, BomHeader bomHeader)
        {
            int i = 1;
            string[] arrayStrings =
            {
                "Непригодные к использованию детали и сборочные единицы из состава",
                $"{bomHeader.RootItem.IzdelInitial} {bomHeader.RootItem.IzdelIma} № {bomHeader.SerialNumber}.",
                $"Договор {bomHeader.Contract} от {bomHeader.ContractDateOpen:dd.MM.yyyy}"
            };

            IXLRange range;
            foreach (string item in arrayStrings)
            {
                range = worksheet.Range("A" + i + ":F" + i + "");
                range.Merge();
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                range.Style.Font.Bold = true;
                worksheet.Cell(i, 1).Value = item;
                i = i + 1;
            }
        }

        private void CreateTableHeader(IXLWorksheet worksheet)
        {
            string[] arrayStrings =
            {
                "Обозначение",
                "Наименование",
                "Тип",
                "Кол-во",
                "ЕИ",
                "Дефект",
            };

            int i = 1;
            foreach (string item in arrayStrings)
            {
                worksheet.Cell(5, i).Value = item;
                worksheet.Cell(5, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(5, i).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(5, i).Style.Font.Bold = true;
                i = i + 1;
            }
        }

        private void AddCellStyle(IXLWorksheet worksheet, int i)
        {
            IXLRange range = worksheet.Range("A5:F" + i + "");
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Cells().Style.Alignment.WrapText = true;

            worksheet.Column(1).Width = 50;
            worksheet.Column(2).Width = 50;
            worksheet.Column(3).Width = 15;
            worksheet.Column(4).Width = 15;
            worksheet.Column(5).Width = 15;
            worksheet.Column(6).Width = 50;
            range.Cells().Style.Alignment.WrapText = true;
        }
    }
}
