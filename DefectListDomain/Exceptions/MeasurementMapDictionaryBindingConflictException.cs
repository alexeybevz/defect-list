using System;

namespace DefectListDomain.Exceptions
{
    // Бросается когда пытаются создать справочник с привязкой к изделию,
    // для которого уже существует справочник на ту же номенклатуру (Code_LSF82).
    // Инвариант гарантируется уникальным индексом UX_MMDictionaryBinding_Root_Code
    // на уровне БД; это исключение — человекочитаемая обёртка над SqlException 2601/2627.
    public class MeasurementMapDictionaryBindingConflictException : Exception
    {
        public MeasurementMapDictionaryBindingConflictException(string message) : base(message) { }
    }
}