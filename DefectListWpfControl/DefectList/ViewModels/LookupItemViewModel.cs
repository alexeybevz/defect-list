using DefectListDomain.Models;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    // ViewModel одной строки в редакторе lookup-справочника.
    // Содержит буферное поле Name (редактируется пользователем),
    // флаг IsUsed (readonly — показывает, почему нельзя переименовать напрямую),
    // и флаг IsActive (для отображения неактивных записей серым).
    public class LookupItemViewModel : ViewModel
    {
        private readonly LookupItem _model;

        public int Id => _model.Id;
        public bool IsUsed => _model.IsUsed;
        public bool IsActive => _model.IsActive;

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged(nameof(Name));
                NotifyPropertyChanged(nameof(HasChanges));
            }
        }

        public bool HasChanges => _name != _model.Name;

        // Подсказка: объясняем пользователю, что произойдёт при сохранении
        public string SaveHint => IsUsed
            ? "Запись используется в картах измерений. При сохранении старая запись будет архивирована, а новая — создана."
            : "Запись не используется. Будет выполнено простое переименование.";

        public LookupItemViewModel(LookupItem model)
        {
            _model = model;
            _name = model.Name;
        }

        public void AcceptChanges()
        {
            _model.Name = _name;
            NotifyPropertyChanged(nameof(HasChanges));
        }
    }
}