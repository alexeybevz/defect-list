using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using DefectListDomain.Models;
using DefectListDomain.ExternalData;

namespace DefectListBusinessLogic.Report
{
    public class DefectListAllItemsReport
    {
        private readonly IGetAllPlanOperationDtoQuery _getAllPlanOperationDtoQuery;

        private string[] _headers =
        {
            "Структура",
            "Обозначение",
            "Наименование",
            "Тип",
            "Заводской номер",
            "Кол-во \n(план)",
            "Кол-во \n(ремонт)",
            "Кол-во \n(замена)",
            "ЕИ",
            "Дефект",
            "Предварительное решение",
            "Окончательное решение",
            "Применяемый тех. процесс",
            "Требуется предъявить ВП",
            "Предъявлено ВП",
            "Примечание",
            "Трудоемкость (Тпз) - ремонт",
            "Трудоемкость (Тшт) - ремонт",
            "Трудоемкость (Тпз) - замена",
            "Трудоемкость (Тшт) - замена"
        };

        public DefectListAllItemsReport(IGetAllPlanOperationDtoQuery getAllPlanOperationDtoQuery)
        {
            _getAllPlanOperationDtoQuery = getAllPlanOperationDtoQuery;
        }

        public void Create(BomHeader bomHeader, IEnumerable<BomItem> data, string pathToReportDirectory)
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Style.Font.FontName = "Arial";
                workbook.Style.Font.FontSize = 10;
                IXLWorksheet worksheet = workbook.Worksheets.Add("Дефектовочная ведомость");
                CreateTableName(worksheet, bomHeader);
                CreateTableHeader(worksheet);

                int i = 4;
                foreach (BomItem item in data)
                {
                    i = i + 1;
                    int j = 1;
                    worksheet.Cell(i, j++).SetValue(item.StructureNumber);
                    worksheet.Cell(i, j++).SetValue(item.Detal);
                    worksheet.Cell(i, j++).SetValue(item.DetalIma);
                    worksheet.Cell(i, j++).SetValue(item.DetalTyp);
                    worksheet.Cell(i, j++).SetValue(item.SerialNumber);
                    worksheet.Cell(i, j++).SetValue(Math.Round(item.QtyMnf, 2));
                    worksheet.Cell(i, j++).SetValue(Math.Round(item.QtyRestore, 2));
                    worksheet.Cell(i, j++).SetValue(Math.Round(item.QtyReplace, 2));
                    worksheet.Cell(i, j++).SetValue(item.DetalUm);
                    worksheet.Cell(i, j++).SetValue(item.Defect);
                    worksheet.Cell(i, j++).SetValue(item.Decision);
                    worksheet.Cell(i, j++).SetValue(item.FinalDecision);
                    worksheet.Cell(i, j++).SetValue(item.TechnologicalProcessUsed);
                    worksheet.Cell(i, j++).SetValue(item.IsRequiredSubmitText);
                    worksheet.Cell(i, j++).SetValue(item.IsSubmittedText);
                    worksheet.Cell(i, j++).SetValue(item.CommentDef);

                   if (!_getAllPlanOperationDtoQuery.IsTehnologicheskayaSborka(item.Detals))
                    {
                        try
                        {
                            var planOpersRestore = _getAllPlanOperationDtoQuery.AskPlanOperations(item.Detals + "Р");
                            var planOpersReplace = _getAllPlanOperationDtoQuery.AskPlanOperations(item.Detals);

                            if (item.QtyRestore > 0 && planOpersRestore != null)
                            {
                                worksheet.Cell(i, j++).SetValue(Math.Round(planOpersRestore.Sum(x => x.Tpz_on_one_det), 3));
                                worksheet.Cell(i, j++).SetValue(Math.Round(planOpersRestore.Sum(x => x.Top_on_one_det * item.QtyRestore), 3));
                            }
                            else
                                j += 2;

                            if (item.QtyReplace > 0 && planOpersReplace != null)
                            {
                                worksheet.Cell(i, j++).SetValue(Math.Round(planOpersReplace.Sum(x => x.Tpz_on_one_det), 3));
                                worksheet.Cell(i, j++).SetValue(Math.Round(planOpersReplace.Sum(x => x.Top_on_one_det * item.QtyReplace), 3));
                            }
                        }
                        catch (Exception e)
                        {
                            var it = item.Id;
                            Console.WriteLine(e);
                            throw;
                        }
                    }

                    if (item.DetalTyp == "издел" || item.DetalTyp == "сб.ед")
                    {
                        var range = worksheet.Range(worksheet.Cell(i, 1), worksheet.Cell(i, 4));
                        range.Style.Font.Bold = true;
                        range.Style.Font.Underline = XLFontUnderlineValues.Single;
                    }
                }

                AddCellStyle(worksheet, i);

                IXLSheetView view = worksheet.SheetView;
                workbook.SaveAs(pathToReportDirectory + $@"\Дефектовочная ведомость по {bomHeader.RootItem.Izdel } № { bomHeader.SerialNumber } от {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.xlsx");
            }
        }

        private void CreateTableName(IXLWorksheet worksheet, BomHeader bomHeader)
        {
            int i = 1;
            string[] arrayStrings =
            {
                $"Дефектовочная ведомость на ремонт { bomHeader.RootItem.Izdel } № { bomHeader.SerialNumber }",
                "от " + DateTime.Today.ToString("dd MMMM yyyy") +" г."
            };

            IXLRange range;
            foreach (string item in arrayStrings)
            {
                range = worksheet.Range(worksheet.Cell(i, 1), worksheet.Cell(i, _headers.Length));
                range.Merge();
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                range.Style.Font.Bold = true;
                worksheet.Cell(i, 1).Value = item;
                i++;
            }
        }

        private void CreateTableHeader(IXLWorksheet worksheet)
        {
            int i = 1;
            foreach (string item in _headers)
            {
                worksheet.Cell(4, i).Value = item;
                worksheet.Cell(4, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(4, i).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(4, i).Style.Font.Bold = true;
                i++;
            }
        }

        private void AddCellStyle(IXLWorksheet worksheet, int i)
        {
            IXLRange range = worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(i, _headers.Length));
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            range = worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(i, _headers.Length));
            range.Cells().Style.Alignment.WrapText = true;

            int j = 1;
            worksheet.Column(j++).Width = 20;
            worksheet.Column(j++).Width = 50;
            worksheet.Column(j++).Width = 50;
            worksheet.Column(j++).Width = 8;
            worksheet.Column(j++).Width = 20;
            worksheet.Column(j++).Width = 15;
            worksheet.Column(j++).Width = 15;
            worksheet.Column(j++).Width = 15;
            worksheet.Column(j++).Width = 15;
            worksheet.Column(j++).Width = 50;
            worksheet.Column(j++).Width = 50;
            worksheet.Column(j++).Width = 50;
            worksheet.Column(j++).Width = 50;
            worksheet.Column(j++).Width = 50;
            worksheet.Column(j++).Width = 50;
            worksheet.Column(j++).Width = 30;
        }
    }
}