using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using DefectListBusinessLogic.Services;
using DefectListDomain.Models;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Report
{
    public class MeasurementMapReport
    {
        private readonly IMeasurementMapItemPresenterService _presenter;
        private readonly string _templateFolderPath;

        public MeasurementMapReport(IMeasurementMapItemPresenterService presenter)
        {
            _presenter = presenter;
            _templateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates");
        }

        public async Task<bool> CreateAsync(MeasurementMap map, BomItem bomItem, BomHeader bomHeader, string pathToReportDirectory)
        {
            return await Task.Run(() =>
            {
                var templateFilePath = Path.Combine(
                    _templateFolderPath,
                    $"{map.MeasurementsMapTypeFormName}.xlsx");

                if (!File.Exists(templateFilePath))
                    throw new FileNotFoundException(
                        $"Шаблон формы '{map.MeasurementsMapTypeFormName}' не найден по пути: {templateFilePath}");

                var reportFilePath = Path.Combine(pathToReportDirectory,
                    $"Карта измерения {map.Id} от {DateTime.Now:yyyy-MM-dd HH-mm-ss}.xlsx");

                File.Copy(templateFilePath, reportFilePath, overwrite: true);

                switch (map.MeasurementsMapTypeFormName)
                {
                    case "Форма 2":
                    {
                        Form2Report.Create(
                            reportFilePath,
                            map,
                            bomItem,
                            bomHeader,
                            _presenter);
                        break;
                    }
                    case "Форма 2в":
                    {
                        Form2Report.Create(
                            reportFilePath,
                            map,
                            bomItem,
                            bomHeader,
                            _presenter);
                        break;
                    }
                    case "Форма 3":
                    {
                        Form3Report.Create(reportFilePath,
                            map,
                            bomItem,
                            bomHeader,
                            _presenter);
                        break;
                    }
                }

                return true;
            });
        }

        private static class Form3Report
        {
            public static void Create(
                string reportPath,
                MeasurementMap map,
                BomItem bomItem,
                BomHeader bomHeader,
                IMeasurementMapItemPresenterService presenter)
            {
                using (var workbook = new XLWorkbook(reportPath))
                {
                    var ws = workbook.Worksheet(1);

                    FillHeader(ws, bomItem, bomHeader);
                    ExcelHelper.AddOrDeleteEmptyRows(ws, 20, map.Items.Count);
                    FillItems(ws, map, presenter);

                    workbook.Save();
                }
            }

            private static void FillItems(IXLWorksheet ws, MeasurementMap map, IMeasurementMapItemPresenterService presenter)
            {
                const int ROW_START = 6;

                var items = map.Items
                    .OrderBy(i => i.SortOrder)
                    .ToList();

                for (int i = 0; i < items.Count; i++)
                {
                    var display = presenter.Present(items[i]);
                    var row = ws.Row(ROW_START + i);

                    row.Cell(1).SetValue((i + 1).ToString("00"));
                    EngineeringTextExcelRenderer.RenderInto(row.Cell(6), display.PossibleDefectName);
                    EngineeringTextExcelRenderer.RenderInto(row.Cell(39), display.NominalValueName);
                    EngineeringTextExcelRenderer.RenderInto(row.Cell(53), display.ActualValue);
                    row.Cell(62).Value = display.ActualRecommendedRepairMethodName;

                    // Excel не подгоняет высоту строки под объединённые (merged) ячейки автоматически,
                    // поэтому считаем нужную высоту вручную по самому "тяжёлому" merge-диапазону в строке.
                    ExcelHelper.AdjustRowHeight(row, 6, 39, 53, 62);
                }
            }

            private static void FillHeader(IXLWorksheet ws, BomItem bomItem, BomHeader bomHeader)
            {
                ws.NamedRange("ObozIzd1").Ranges.SetValue(bomItem.Detal);
                ws.NamedRange("ObozDetal1").Ranges.SetValue(bomItem.DetalIma);
                ws.NamedRange("ObozKarta1").Ranges.SetValue($"№ {bomItem.SerialNumber}");
                ws.Cell("N2").SetValue($"{bomHeader.RootItem.Izdel} № {bomHeader.SerialNumber}");
            }
        }

        private static class Form2Report
        {
            public static void Create(
                string reportPath,
                MeasurementMap map,
                BomItem bomItem,
                BomHeader bomHeader,
                IMeasurementMapItemPresenterService presenter)
            {
                using (var workbook = new XLWorkbook(reportPath))
                {
                    var ws = workbook.Worksheet(1);

                    FillHeader(ws, bomItem, bomHeader);
                    ExcelHelper.AddOrDeleteEmptyRows(ws, 29, map.Items.Count);
                    FillItems(ws, map, presenter);

                    workbook.Save();
                }
            }

            private static void FillItems(IXLWorksheet ws, MeasurementMap map, IMeasurementMapItemPresenterService presenter)
            {
                const int ROW_START = 15;

                var items = map.Items
                    .OrderBy(i => i.SortOrder)
                    .ToList();

                for (int i = 0; i < items.Count; i++)
                {
                    var display = presenter.Present(items[i]);
                    var row = ws.Row(ROW_START + i);

                    row.Cell(1).Value = (i + 1).ToString("00");
                    EngineeringTextExcelRenderer.RenderInto(row.Cell(6), display.PossibleDefectName);
                    EngineeringTextExcelRenderer.RenderInto(row.Cell(45), display.NominalValueName);
                    EngineeringTextExcelRenderer.RenderInto(row.Cell(53), display.ActualValue);
                    row.Cell(61).Value = display.ActualRecommendedRepairMethodName;
                    row.Cell(72).Value = display.MarkOfWorkCompletion;
                    row.Cell(86).Value = display.ActualValueRecordDate?.ToString("dd.MM.yyyy");
                    row.Cell(93).Value = display.RequirementPostRepairName;

                    // Excel не подгоняет высоту строки под объединённые (merged) ячейки автоматически,
                    // поэтому считаем нужную высоту вручную по самому "тяжёлому" merge-диапазону в строке.
                    ExcelHelper.AdjustRowHeight(row, 6, 45, 53, 61, 86, 93);
                }
            }

            private static void FillHeader(IXLWorksheet ws, BomItem bomItem, BomHeader bomHeader)
            {
                ws.NamedRange("ObozIzd1").Ranges.SetValue(bomItem.Detal);
                ws.NamedRange("ObozDetal1").Ranges.SetValue(bomItem.DetalIma);
                ws.NamedRange("ObozKarta1").Ranges.SetValue($"№ {bomItem.SerialNumber}");
                ws.Cell("AD2").SetValue($"{bomHeader.RootItem.Izdel} № {bomHeader.SerialNumber}");
            }
        }

        private class ExcelHelper
        {
            /// <summary>
            /// Подгоняет высоту строки под содержимое перечисленных ячеек, учитывая ширину
            /// их merge-диапазонов (стандартный row.AdjustToContents() этого не умеет).
            /// </summary>
            public static void AdjustRowHeight(IXLRow row, params int[] columnsToCheck)
            {
                double maxHeight = row.Worksheet.RowHeight; // высота по умолчанию для листа

                foreach (var col in columnsToCheck)
                {
                    var cell = row.Cell(col);
                    var text = cell.GetString();
                    if (string.IsNullOrEmpty(text))
                        continue;

                    var mergedRange = cell.Worksheet.MergedRanges.Single(r => r.Contains(cell));
                    double rangeWidthChars = mergedRange
                        .Columns()
                        .Select((c, idx) => row.Worksheet.Column(mergedRange.FirstColumn().ColumnNumber() + idx).Width)
                        .Sum();

                    var font = cell.Style.Font;
                    double needed = EstimateRowHeight(text, rangeWidthChars, font.FontName, font.FontSize);

                    if (needed > maxHeight)
                        maxHeight = needed;
                }

                row.Height = maxHeight;
            }

            private static readonly Dictionary<string, Font> _fontCache = new Dictionary<string, Font>();

            /// <summary>
            /// Приблизительный расчёт высоты строки под текст в объединённом диапазоне заданной ширины.
            /// Ширина колонки Excel переводится в пиксели по стандартной формуле Microsoft (MDW=7, Calibri 11);
            /// при использовании другого шрифта/размера в шаблоне коэффициент maxDigitWidth стоит перепроверить.
            /// </summary>
            private static double EstimateRowHeight(string text, double mergedWidthInExcelChars, string fontName, double fontSizePt)
            {
                if (string.IsNullOrEmpty(text))
                    return fontSizePt * 1.5;

                var key = $"{fontName}_{fontSizePt}";
                Font font;
                if (!_fontCache.TryGetValue(key, out font))
                {
                    font = new Font(fontName, (float)fontSizePt);
                    _fontCache[key] = font;
                }

                const double maxDigitWidth = 14;
                double widthPx = Math.Truncate(((256 * mergedWidthInExcelChars + Math.Truncate(128 / maxDigitWidth)) / 256) * maxDigitWidth);
                if (widthPx <= 0) widthPx = 1;

                using (var bmp = new Bitmap(1, 1))
                using (var g = Graphics.FromImage(bmp))
                {
                    int totalLines = 0;
                    foreach (var line in text.Split('\n'))
                    {
                        var size = g.MeasureString(line, font, new SizeF(float.MaxValue, float.MaxValue));
                        int wrapped = (int)Math.Ceiling(size.Width / widthPx);
                        totalLines += Math.Max(1, wrapped);
                    }

                    float lineHeightPx = font.GetHeight(g);
                    double lineHeightPt = lineHeightPx * 72.0 / g.DpiY;

                    return totalLines * lineHeightPt * 1.15; // небольшой запас на паддинги
                }
            }

            public static void AddOrDeleteEmptyRows(IXLWorksheet ws, int lastRow, int itemsCount)
            {
                // По умолчанию в шаблоне 15 строк.
                // Если количество строк в карте измерения превышает это количество, создаются новые строки
                // если меньше этого количества - удаляются.

                const int templateCountRows = 15;

                if (itemsCount > templateCountRows)
                {
                    var delta = itemsCount - templateCountRows;

                    for (int i = 1; i <= delta; i++)
                    {
                        ws.Row(lastRow).InsertRowsBelow(1);
                        CloneRow(ws, lastRow, lastRow + 1);
                    }
                }

                if (itemsCount < templateCountRows)
                {
                    var delta = templateCountRows - itemsCount;
                    ws.Range($"{lastRow - delta + 1}:{lastRow}").Delete(XLShiftDeletedCells.ShiftCellsUp);
                }
            }

            public static void CloneRow(
                IXLWorksheet worksheet,
                int sourceRowNumber,
                int targetRowNumber)
            {
                var sourceRow = worksheet.Row(sourceRowNumber);
                var targetRow = worksheet.Row(targetRowNumber);

                // Высота строки
                targetRow.Height = sourceRow.Height;

                int lastColumn = worksheet.LastColumnUsed().ColumnNumber();

                // Копируем стили ячеек
                for (int column = 1; column <= lastColumn; column++)
                {
                    targetRow.Cell(column).Style =
                        sourceRow.Cell(column).Style;
                }

                // Копируем объединения
                foreach (var mergedRange in worksheet.MergedRanges)
                {
                    if (mergedRange.FirstRow().RowNumber() != sourceRowNumber ||
                        mergedRange.LastRow().RowNumber() != sourceRowNumber)
                    {
                        continue;
                    }

                    int firstColumn = mergedRange.FirstColumn().ColumnNumber();
                    int lastMergedColumn = mergedRange.LastColumn().ColumnNumber();

                    worksheet.Range(
                            targetRowNumber,
                            firstColumn,
                            targetRowNumber,
                            lastMergedColumn)
                        .Merge();
                }
            }
        }
    }
}