using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using DefectListBusinessLogic.Services;
using DefectListDomain.Models;

namespace DefectListWpfControl.DefectList.Commons
{
    // Конвертер для отображения NominalValue (и любого другого значения
    // с инженерной нотацией допусков) в TextBlock с настоящими
    // верхними/нижними индексами.
    //
    // Используется не как обычный Binding.Converter (TextBlock.Text
    // принимает только string, не коллекцию Inline), а через attached-like
    // паттерн: применяется в коде, который заполняет TextBlock.Inlines
    // напрямую. См. EngineeringTextBlockBehavior ниже для XAML-варианта.
    public static class EngineeringTextRenderer
    {
        // Насколько визуально уменьшать размер шрифта у над-/подстрочного
        // фрагмента относительно базового. 0.75 — стандартное соотношение
        // для верхних/нижних индексов в типографике.
        private const double SupSubFontScale = 0.75;

        public static void RenderInto(TextBlock textBlock, string rawValue)
        {
            textBlock.Inlines.Clear();

            if (string.IsNullOrEmpty(rawValue))
                return;

            var fragments = EngineeringTextParser.Parse(rawValue);
            var baseFontSize = textBlock.FontSize;

            foreach (var fragment in fragments)
            {
                switch (fragment.Kind)
                {
                    case EngineeringTextFragmentKind.NewLine:
                        textBlock.Inlines.Add(new LineBreak());
                        break;

                    case EngineeringTextFragmentKind.Normal:
                        textBlock.Inlines.Add(new Run(fragment.Text));
                        break;

                    case EngineeringTextFragmentKind.Super:
                        textBlock.Inlines.Add(new Run(fragment.Text)
                        {
                            BaselineAlignment = BaselineAlignment.Superscript,
                            FontSize = baseFontSize * SupSubFontScale
                        });
                        break;

                    case EngineeringTextFragmentKind.Sub:
                        textBlock.Inlines.Add(new Run(fragment.Text)
                        {
                            BaselineAlignment = BaselineAlignment.Subscript,
                            FontSize = baseFontSize * SupSubFontScale
                        });
                        break;
                }
            }
        }
    }

    // Attached Behavior, позволяющий задать разбираемый текст прямо в XAML
    // через биндинг, без code-behind:
    //
    //     <TextBlock commons:EngineeringTextBehavior.Text="{Binding NominalValueName}" />
    //
    // Это нужно, потому что TextBlock.Text — string-свойство и не может
    // принять размеченный список Inline напрямую через обычный Binding.
    public static class EngineeringTextBehavior
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached(
                "Text",
                typeof(string),
                typeof(EngineeringTextBehavior),
                new PropertyMetadata(null, OnTextChanged));

        public static string GetText(DependencyObject obj) => (string)obj.GetValue(TextProperty);
        public static void SetText(DependencyObject obj, string value) => obj.SetValue(TextProperty, value);

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;

            if (textBlock != null)
                EngineeringTextRenderer.RenderInto(textBlock, e.NewValue as string);
        }
    }
}