using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ClosedXML.Excel;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Report
{
    public class ChangesFinalDecisionReport
    {
        public void Create(IReadOnlyCollection<FinalDecisionChanging> data, string pathToFileReport)
        {
            if (!data.Any())
            {
                MessageBox.Show("Нет данных для формирования отчета");
                return;
            }

            using (var wb = new XLWorkbook())
            {
                var ws = wb.AddWorksheet("Данные");

                CreateHeader(ws);
                CreateBody(ws, data);
                PostFormat(ws);

                wb.SaveAs(pathToFileReport);
            }
        }

        private void CreateHeader(IXLWorksheet ws)
        {
            var headers = new List<string>()
            {
                "ID ДВ",
                "Заказ",
                "Изделие",
                "Обозначение ДСЕ",
                "Наименование ДСЕ",
                "Тип ДСЕ",
                "Кол-во план",
                "Ед.изм. ДСЕ",
                "Окончательное решение\n(старое)",
                "Окончательное решение\n(новое)",
                "Маршрутные карты",
                "Дата изменения",
                "Изменено пользователем",
            };

            headers.Select((x, ind) => new KeyValuePair<string, int>(x, ind)).ForEach(x => ws.Cell(1, x.Value + 1).SetValue(x.Key));
        }

        private void CreateBody(IXLWorksheet ws, IEnumerable<FinalDecisionChanging> data)
        {
            var row = 2;

            foreach (var d in data)
            {
                var col = 1;
                ws.Cell(row, col++).SetValue(d.BomId);
                ws.Cell(row, col++).SetValue(d.Orders);
                ws.Cell(row, col++).SetValue(d.Izdel);
                ws.Cell(row, col++).SetValue(d.Detal);
                ws.Cell(row, col++).SetValue(d.DetalIma);
                ws.Cell(row, col++).SetValue(d.DetalTyp);
                ws.Cell(row, col++).SetValue(Math.Round(d.QtyMnf, 2));
                ws.Cell(row, col++).SetValue(d.DetalUm);
                ws.Cell(row, col++).SetValue(d.FinalDecision);
                ws.Cell(row, col++).SetValue(d.NextFinalDecision);
                ws.Cell(row, col++).SetValue(d.Nomgodurs);
                ws.Cell(row, col++).SetValue(d.CreateDate);
                ws.Cell(row, col++).SetValue(d.CreatedByName);

                row++;
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
        }
    }
}