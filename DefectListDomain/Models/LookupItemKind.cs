namespace DefectListDomain.Models
{
    // Перечисление поддерживаемых lookup-справочников.
    // Используется в LookupDictionaryViewModel как дискриминатор стратегии —
    // позволяет иметь одну форму (LookupDictionaryWindow) и одну ViewModel
    // для всех четырёх справочников вместо четырёх отдельных классов.
    public enum LookupItemKind
    {
        PossibleDefect,
        NominalValue,
        RecommendedRepairMethod,
        RequirementPostRepair
    }
}