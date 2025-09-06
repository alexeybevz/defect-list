using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Report
{
    public class DefectListPurchaseItemsReport
    {
        public void Create(BomHeader bomHeader, IReadOnlyCollection<BomItem> data, string pathToReportDirectory)
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Style.Font.FontName = "Times New Roman";
                workbook.Style.Font.FontSize = 14;
                IXLWorksheet worksheet = workbook.Worksheets.Add("Отчет ПКИ для ОМТС");
                CreateTableName(worksheet, bomHeader);
                CreateTableHeader(worksheet);

                int i = 4;
                foreach (BomItem item in data.OrderBy(x => x.Detal))
                {
                    i = i + 1;
                    worksheet.Cell(i, 1).SetValue(item.Detal);
                    worksheet.Cell(i, 2).SetValue(item.DetalIma);
                    worksheet.Cell(i, 3).SetValue(Math.Round(item.QtyMnf, 2));
                    worksheet.Cell(i, 4).SetValue(item.DetalUm);
                    worksheet.Cell(i, 5).Value = $@"Установленный дефект: {item.Defect}{(string.IsNullOrEmpty(item.Defect) ? "" : ".")}
Решение по устранению: {item.Decision}";

                    for (int j = 1; j <= 2; j++)
                    {
                        worksheet.Cell(i, j).Style.Font.Bold = true;
                        worksheet.Cell(i, j).Style.Font.Underline = XLFontUnderlineValues.Single;
                    }
                }
                
                AddCellStyle(worksheet, i);
                AddFootnote(worksheet, i);

                IXLSheetView view = worksheet.SheetView;
                view.ZoomScale = 75;

                workbook.SaveAs(pathToReportDirectory + $@"\Отчет ПКИ для ОМТС по {bomHeader.RootItem.Izdel } № { bomHeader.SerialNumber } от {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.xlsx");
            }
        }

        private void CreateTableName(IXLWorksheet  worksheet, BomHeader bomHeader)
        {
            int i = 1;
            string[] arrayStrings =
            {
                $"Дефектовочная ведомость ПКИ на ремонт { bomHeader.RootItem.Izdel } № { bomHeader.SerialNumber }",
                "от " + DateTime.Today.ToString("dd MMMM yyyy") +" г."
            };

            IXLRange range;
            foreach (string item in arrayStrings)
            {
                range = worksheet.Range("A"+ i + ":G"+ i +"");
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
                @"Обозначение детали, 
сборочной единицы",
                @"Наименование детали, 
сборочной единицы",
                "Количество",
                "ЕИ",
                @"Установленный дефект. Решение по устранению/
использованию (выполнить работы по т/пр./ заменить/
использовать). Основание для решения (пункт РД, 
отсутствие)",
                "Примечание",
                @"Отметка о 
выполнении работ 
(по сборочным
единицам)"
            };

            int i = 0;
            foreach (string item in arrayStrings)
            {
                i = i + 1;
                worksheet.Cell(4, i).Value = item;
                worksheet.Cell(4, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(4, i).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(4, i).Style.Font.Bold = true;
            }
        }

        private void AddFootnote(IXLWorksheet worksheet, int i) //Создание сноски в конце листа.
        {
            string[] arrayStrings = 
            {
                "ЗГД по производству",
                "Представитель отдела 21",
                "Представитель цеха 87",
                "Представитель ОТК",
                "Представитель ОГТ"
            };

            foreach (string item in arrayStrings)
            {
                i = i + 2;
                worksheet.Cell(i, 5).Value = item;
                worksheet.Cell(i, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Range("F" + i + ":G"+ i +"").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(i + 1, 7).Value = "Ф.И.О";
                worksheet.Cell(i + 1, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
        }

        private void AddCellStyle(IXLWorksheet worksheet, int i)
        {
            IXLRange range = worksheet.Range("A4:G" + i + "");

            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            worksheet.Column(1).Width = 50;
            worksheet.Column(2).Width = 50;
            worksheet.Column(3).Width = 20;
            worksheet.Column(4).Width = 20;
            worksheet.Column(5).Width = 80;
            worksheet.Column(6).Width = 20;
            worksheet.Column(7).Width = 25;
            range.Cells().Style.Alignment.WrapText = true;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }
    }
}