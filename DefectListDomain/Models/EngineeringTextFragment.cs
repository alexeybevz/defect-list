namespace DefectListDomain.Models
{
    // Один фрагмент разобранного значения NominalValue (или любого другого
    // текста с инженерной разметкой допусков).
    // Kind определяет, как фрагмент должен отображаться:
    //   Normal  — обычный текст
    //   Super   — верхний индекс (надстрочный допуск, было 〖X〗^(Y))
    //   Sub     — нижний индекс (подстрочный допуск, было 〖X〗_(Y))
    //   NewLine — явный перевод строки между исходными строками значения
    public enum EngineeringTextFragmentKind
    {
        Normal,
        Super,
        Sub,
        NewLine
    }

    public class EngineeringTextFragment
    {
        public EngineeringTextFragmentKind Kind { get; }
        public string Text { get; }

        public EngineeringTextFragment(EngineeringTextFragmentKind kind, string text)
        {
            Kind = kind;
            Text = text;
        }
    }
}