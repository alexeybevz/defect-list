using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using DefectListDomain.Dtos;
using System;
using System.Threading.Tasks;

namespace DefectListBusinessLogic.Report
{
    public class ExportCreatedRouteMapsReport
    {
        public async Task<bool> CreateAsync(string rootItemName, IReadOnlyList<RouteMapDto> items, string pathToReportDirectory)
        {
            return await Task.Run(() =>
            {
                if (!items.Any())
                    throw new InvalidDataException("Нет данных для формирования отчета");

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.AddWorksheet("Данные");

                    CreateHeader(ws);
                    CreateBody(ws, rootItemName, items);
                    PostFormat(ws);

                    var path = Path.Combine(pathToReportDirectory,
                        $"Созданные МК от {DateTime.Now:yyyy-MM-dd HH-mm-ss}.xlsx");
                    wb.SaveAs(path);

                    return true;
                }
            });
        }

        private void CreateHeader(IXLWorksheet ws)
        {
            var headers = new List<string>()
            {
                "Обозначение",
                "Наименование",
                "Марка материала",
                "Размер загот.",
                "Кол-во",
                "Заказ",
                "Тип",
                "Изделие",
                "МК 0 ур.",
                "МК 1 ур.",
                "Статус передачи МК в Контроль",
                "Материал",
                "Наличие технологической детали"
            };

            headers.Select((x, ind) => new KeyValuePair<string, int>(x, ind)).ForEach(x => ws.Cell(1, x.Value + 1).SetValue(x.Key));
        }

        private void CreateBody(
            IXLWorksheet ws,
            string rootItemName,
            IReadOnlyList<RouteMapDto> items)
        {
            var row = 2;

            foreach (var item in items)
            {
                var col = 1;
                ws.Cell(row, col++).SetValue(item.Detal);
                ws.Cell(row, col++).SetValue(item.ImaDetal);
                ws.Cell(row, col++).SetValue(item.Marka);
                ws.Cell(row, col++).SetValue(item.RazmZagot);
                ws.Cell(row, col++).SetValue(item.Qty);
                ws.Cell(row, col++).SetValue(item.OrdersCode);
                ws.Cell(row, col++).SetValue(item.Typ);
                ws.Cell(row, col++).SetValue(rootItemName);
                ws.Cell(row, col++).SetValue(item.ParentNomgodur);
                ws.Cell(row, col++).SetValue(item.Nomgodur);
                ws.Cell(row, col++).SetValue(item.SendStatus);
                ws.Cell(row, col++).SetValue(item.DesignationMtrls);
                ws.Cell(row, col++).SetValue(item.IsTehDetal ? "Да" : "Нет");

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