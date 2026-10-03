using System.Collections.Generic;
using System.Text.RegularExpressions;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Services
{
    // Парсер инженерной нотации допусков, скопированной из Word-формул
    // (через объект Equation) в значения справочника NominalValue.
    //
    // Поддерживаемые конструкции (расширять по факту появления новых случаев,
    // не угадывая заранее — см. обсуждение с заказчиком):
    //
    //   〖X〗^(Y)   →  X обычным текстом, затем Y верхним индексом
    //   〖X〗_(Y)   →  X обычным текстом, затем Y нижним индексом
    //   любая другая строка (включая начинающуюся с ↗, либо без 〖〗 вовсе)
    //               →  выводится как обычный текст без интерпретации
    //   〖X〗^(A@B) →  X обычным текстом, затем A и B — каждое тем же типом
    //                  индекса (верхний/нижний — определяется по <маркеру>
    //                  так же, как выше), но между A и B вставляется
    //                  перенос строки. Используется, когда в одной формуле
    //                  записаны два значения допуска друг под другом
    //
    // Многострочные значения (через \n или \r\n) разбиваются на отдельные
    // строки, между которыми вставляется фрагмент NewLine.
    //
    // Этот класс ничего не знает ни про WPF, ни про Excel — он только
    // превращает строку в список EngineeringTextFragment.
    // Конкретный рендеринг (Inlines для WPF, RichText для Excel)
    // делается отдельными классами, которые принимают результат этого парсера.
    public static class EngineeringTextParser
    {
        // 〖 = U+3016, 〗 = U+3017
        // Группа 1 — содержимое скобок (база)
        // Группа 2 — маркер: ^ (верхний индекс) или _ (нижний индекс) или ^(диапазон)
        // Группа 3 — содержимое круглых скобок после маркера
        //            (если между значениями есть разделитель @, то левое значение идет в верхний индекс, правое - в нижний
        private static readonly Regex SupSubPattern =
            new Regex(@"〖([^〗]*)〗([\^_])\(([^)]*)\)", RegexOptions.Compiled);

        public static IReadOnlyList<EngineeringTextFragment> Parse(string value)
        {
            var fragments = new List<EngineeringTextFragment>();

            if (string.IsNullOrEmpty(value))
                return fragments;

            // Нормализуем переводы строк перед разбивкой,
            // т.к. данные могли попасть в БД и с \r\n, и с \n
            var lines = value.Replace("\r\n", "\n").Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                    fragments.Add(new EngineeringTextFragment(EngineeringTextFragmentKind.NewLine, string.Empty));

                ParseLine(lines[i], fragments);
            }

            return fragments;
        }

        private static void ParseLine(string line, List<EngineeringTextFragment> fragments)
        {
            int pos = 0;

            foreach (Match match in SupSubPattern.Matches(line))
            {
                // Текст перед найденной группой (если есть) — обычный
                if (match.Index > pos)
                {
                    var pre = line.Substring(pos, match.Index - pos);
                    if (pre.Length > 0)
                        fragments.Add(new EngineeringTextFragment(EngineeringTextFragmentKind.Normal, pre));
                }

                var baseText = match.Groups[1].Value;
                var marker = match.Groups[2].Value;
                var supSubText = match.Groups[3].Value;

                fragments.Add(new EngineeringTextFragment(EngineeringTextFragmentKind.Normal, baseText));

                var kind = ResolveKind(marker);
                AppendStackedValues(supSubText, kind, fragments, baseText);

                pos = match.Index + match.Length;
            }

            // Остаток строки после последней найденной группы
            // (или вся строка целиком, если групп не найдено вообще —
            // это покрывает случаи типа "G 5 +0,1" и "↗0,025-0,06")
            if (pos < line.Length)
            {
                var tail = line.Substring(pos);
                if (tail.Length > 0)
                    fragments.Add(new EngineeringTextFragment(EngineeringTextFragmentKind.Normal, tail));
            }
        }

        private static EngineeringTextFragmentKind ResolveKind(string marker)
        {
            return marker.IndexOf('^') >= 0
                ? EngineeringTextFragmentKind.Super
                : EngineeringTextFragmentKind.Sub;
        }

        // Разделитель диапазона значений внутри одного индекса (напр. "A@B" →
        // "A" - верхний индекс, "B" - нижний индекс
        private const char StackedValueSeparator = '@';

        // Разбивает содержимое индекса по StackedValueSeparator ('@') на
        // отдельные значения, которые должны отображаться друг под другом
        // в верхнем и нижнем индекс (Super и Sub).
        // Между значениями вставляется NewLine-фрагмент.
        // Если разделителя нет, добавляется один фрагмент.
        private static void AppendStackedValues(
            string content,
            EngineeringTextFragmentKind kind,
            List<EngineeringTextFragment> fragments,
            string baseText)
        {
            if (content.Contains("@"))
            {
                // Символ @ внутри индекса означает перенос строки.
                // Разделитель не выводится.
                var indexLines = content.Split('@');

                if (indexLines.Length == 2)
                {
                    fragments.Add(new EngineeringTextFragment(EngineeringTextFragmentKind.Super,
                        string.Join(" / ", indexLines)));

                    //fragments.Add(new EngineeringTextFragment(
                    //    EngineeringTextFragmentKind.Super,
                    //    indexLines[0]));

                    //fragments.Add(new EngineeringTextFragment(
                    //    EngineeringTextFragmentKind.NewLine,
                    //    string.Empty));

                    // После переноса строки Excel начинает новую строку
                    // с нулевой позиции. Добавляем отступ, чтобы второе
                    // значение находилось под первым значением индекса,
                    // а не под базовым текстом.
                    //fragments.Add(new EngineeringTextFragment(
                    //    EngineeringTextFragmentKind.Normal,
                    //    new string(' ', baseText.Length * 3)));

                    //fragments.Add(new EngineeringTextFragment(
                    //    EngineeringTextFragmentKind.Sub,
                    //    indexLines[1]));
                }
            }
            else
            {
                fragments.Add(new EngineeringTextFragment(kind, content));
            }
        }
    }
}