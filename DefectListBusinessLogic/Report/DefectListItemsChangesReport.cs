using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ClosedXML.Excel;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Report
{
    public class DefectListItemsChangesReport
    {
        public void Create(IBomHeader bomHeader, IReadOnlyCollection<BomItem> bomItems, IReadOnlyCollection<BomItemLog> bomItemsLogs, string pathToReport)
        {
            if (!bomItemsLogs.Any())
            {
                MessageBox.Show("Нет данных для формирования отчета");
                return;
            }

            using (var wb = new XLWorkbook())
            {
                var ws = wb.AddWorksheet("Данные");

                CreateHeader(ws);
                CreateBody(bomItems, bomItemsLogs, ws);
                PostFormat(ws);

                wb.SaveAs(pathToReport +
                          $@"\Журнал изменений по {bomHeader.RootItem.Izdel} № {bomHeader.SerialNumber} от {DateTime.Now:yyyy-MM-dd HH-mm-ss}.xlsx");
            }
        }

        private void PostFormat(IXLWorksheet ws)
        {
            ws.Style.Font.FontName = "Arial";
            ws.Style.Font.FontSize = 10;
            ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            // Фиксируем шапку отчета
            ws.SheetView.FreezeRows(1);

            // Включаем автофильтр
            var range = ws.Range(1, 1, 1, ws.LastColumnUsed().ColumnNumber());
            range.SetAutoFilter();

            // Выравнивание содержимого столбцов по центру
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Включаем перенос текста
            range.Style.Alignment.WrapText = true;

            // Устанавливаем ширину столбцов в зависимости от содержимого
            ws.Columns().AdjustToContents();

            // Ширина столбцов "Установленный дефект"
            ws.Column(11).Width = 60;
            ws.Column(12).Width = 60;
        }

        private void CreateHeader(IXLWorksheet ws)
        {
            var headers = new List<string>()
            {
                "Структура",
                "Обозначение ДСЕ",
                "Наименование ДСЕ",
                "Тип ДСЕ",
                "Серийный номер",
                "Кол-во (план)",

                "Кол-во ремонт\n(старое)",
                "Кол-во ремонт\n(новое)",

                "Кол-во замена\n(старое)",
                "Кол-во замена\n(новое)",

                "Установленный дефект\n(старое)",
                "Установленный дефект\n(новое)",

                "Первоначальное решение\n(старое)",
                "Первоначальное решение\n(новое)",

                "Окончательное решение\n(старое)",
                "Окончательное решение\n(новое)",

                "Применяемый тех.процесс\n(старое)",
                "Применяемый тех.процесс\n(новое)",

                "Требуется предъявить ВП\n(старое)",
                "Требуется предъявить ВП\n(новое)",

                "Предъявлено ВП\n(старое)",
                "Предъявлено ВП\n(новое)",

                "Примечание\n(старое)",
                "Примечание\n(новое)",

                "Дата изменения",
                "Изменено пользователем",
            };

            headers.Select((x, ind) => new KeyValuePair<string, int>(x, ind)).ForEach(x => ws.Cell(1, x.Value + 1).SetValue(x.Key));
        }

        private static void CreateBody(IEnumerable<BomItem> bomItems, IReadOnlyCollection<BomItemLog> bomItemsLogs, IXLWorksheet ws)
        {
            var row = 2;
            var i = 1;
            foreach (var bi in bomItems)
            {
                var isSetColorRow = i % 2 == 0;

                var logs = bomItemsLogs.Where(x => x.BomItemId == bi.Id).ToList().OrderBy(x => x.Id).ToList();
                if (!logs.Any())
                {
                    var col = 1;
                    ws.Cell(row, col++).SetValue(bi.StructureNumber);
                    ws.Cell(row, col++).SetValue(bi.Detal);
                    ws.Cell(row, col++).SetValue(bi.DetalIma);
                    ws.Cell(row, col++).SetValue(bi.DetalTyp);
                    ws.Cell(row, col++).SetValue(bi.SerialNumber);
                    ws.Cell(row, col++).SetValue(Math.Round(bi.QtyMnf, 2));

                    if (isSetColorRow)
                        ws.Rows(row, row).Style.Fill.BackgroundColor = XLColor.FromArgb(250, 235, 215);

                    row++;
                }

                foreach (var current in logs)
                {
                    if (isSetColorRow)
                        ws.Rows(row, row).Style.Fill.BackgroundColor = XLColor.FromArgb(250, 235, 215);

                    var col = 1;
                    ws.Cell(row, col++).SetValue(bi.StructureNumber);
                    ws.Cell(row, col++).SetValue(bi.Detal);
                    ws.Cell(row, col++).SetValue(bi.DetalIma);
                    ws.Cell(row, col++).SetValue(bi.DetalTyp);
                    ws.Cell(row, col++).SetValue(bi.SerialNumber);
                    ws.Cell(row, col++).SetValue(Math.Round(bi.QtyMnf, 2));

                    var previous = logs.LastOrDefault(x => x.Id < current.Id);
                    ws.Cell(row, col++).SetValue(Math.Round(previous?.QtyRestore ?? 0, 2));
                    ws.Cell(row, col++).SetValue(Math.Round(current.QtyRestore, 2));

                    ws.Cell(row, col++).SetValue(Math.Round(previous?.QtyReplace ?? 0, 2));
                    ws.Cell(row, col++).SetValue(Math.Round(current.QtyReplace, 2));

                    ws.Cell(row, col++).SetValue(previous?.Defect);
                    ws.Cell(row, col++).SetValue(current.Defect);

                    ws.Cell(row, col++).SetValue(previous?.Decision);
                    ws.Cell(row, col++).SetValue(current.Decision);

                    ws.Cell(row, col++).SetValue(previous?.FinalDecision);
                    ws.Cell(row, col++).SetValue(current.FinalDecision);

                    ws.Cell(row, col++).SetValue(previous?.TechnologicalProcessUsed);
                    ws.Cell(row, col++).SetValue(current.TechnologicalProcessUsed);

                    ws.Cell(row, col++).SetValue(previous?.IsRequiredSubmitText);
                    ws.Cell(row, col++).SetValue(current.IsRequiredSubmitText);

                    ws.Cell(row, col++).SetValue(previous?.IsSubmittedText);
                    ws.Cell(row, col++).SetValue(current.IsSubmittedText);

                    ws.Cell(row, col++).SetValue(previous?.CommentDef);
                    ws.Cell(row, col++).SetValue(current.CommentDef);

                    ws.Cell(row, col++).SetValue(current.CreateDate);
                    ws.Cell(row, col++).SetValue(current.CreatedByName);

                    row++;
                }

                i++;
            }
        }
    }
}