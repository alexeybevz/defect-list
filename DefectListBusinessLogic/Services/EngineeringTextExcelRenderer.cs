using System;
using ClosedXML.Excel;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Services
{
    // Рендерер инженерной нотации допусков в ячейку Excel через ClosedXML.
    //
    // ВНИМАНИЕ: API ClosedXML для RichText менялся между версиями.
    // В версии 0.87.1 (используется в проекте) RichText — это СВОЙСТВО
    // (cell.RichText), а не метод GetRichText() — последний появился
    // только начиная с версии 0.96.0. Аналогично, тип enum называется
    // XLFontVerticalTextAlignmentValues (не XLFontVerticalTextAlignment),
    // а устанавливается через метод SetVerticalAlignment(...), а не через
    // присвоение свойству VerticalAlignment.
    //
    // Использует cell.RichText, который поддерживает несколько "ранов"
    // (runs) текста с разным форматированием в одной ячейке — включая
    // вертикальное выравнивание Superscript/Subscript, что и нужно для
    // настоящего верхнего/нижнего индекса в самом xlsx-файле
    // (не просто визуально похоже, а реальный Excel-формат рана,
    // распознаваемый и при открытии файла в самом Excel).
    public static class EngineeringTextExcelRenderer
    {
        public static void RenderInto(IXLCell cell, string rawValue)
        {
            // Очищаем текущее содержимое ячейки перед записью RichText —
            // иначе ClosedXML может задвоить значение, если в ячейке
            // уже что-то было присвоено через Cell.Value ранее.
            cell.Value = string.Empty;

            if (string.IsNullOrEmpty(rawValue))
                return;

            // WrapText включаем заранее, до добавления текста — некоторые версии
            // ClosedXML кэшируют стиль ячейки в момент создания первого рана RichText,
            // и более поздняя установка WrapText на саму ячейку может не подхватиться
            // для уже добавленных ранов при сохранении.
            cell.Style.Alignment.WrapText = true;

            var fragments = EngineeringTextParser.Parse(rawValue);
            var richText = cell.RichText;

            foreach (var fragment in fragments)
            {
                switch (fragment.Kind)
                {
                    case EngineeringTextFragmentKind.NewLine:
                        // ClosedXML не имеет отдельного "переноса строки" как рана —
                        // перенос строки внутри ячейки кодируется символом разрыва строки
                        // непосредственно в тексте рана. ВАЖНО: голый "\n" не всегда
                        // распознаётся как разрыв строки при сохранении в xlsx —
                        // надёжно работает Environment.NewLine ("\r\n" на Windows).
                        richText.AddText(Environment.NewLine);
                        break;

                    case EngineeringTextFragmentKind.Normal:
                        richText.AddText(fragment.Text);
                        break;

                    case EngineeringTextFragmentKind.Super:
                        richText.AddText(fragment.Text)
                            .SetVerticalAlignment(XLFontVerticalTextAlignmentValues.Superscript);
                        break;

                    case EngineeringTextFragmentKind.Sub:
                        richText.AddText(fragment.Text)
                            .SetVerticalAlignment(XLFontVerticalTextAlignmentValues.Subscript);
                        break;
                }
            }
        }
    }
}