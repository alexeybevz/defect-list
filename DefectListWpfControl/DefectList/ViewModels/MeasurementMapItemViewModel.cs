using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DefectListDomain.Models;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class MeasurementMapItemViewModel : ViewModel
    {
        private readonly MeasurementMapItem _model;
        private readonly CustomIdentity _user;

        public int Id => _model.Id;
        public int MeasurementMapId => _model.MeasurementMapId;
        public string CustomNumeration => _model.CustomNumeration;
        public string PossibleDefectName => _model.PossibleDefectName;

        public string NominalValueName => _model.NominalValueName;
        public string RequirementPostRepairName => _model.RequirementPostRepairName;
        public MeasurementMapItemType ItemType => _model.ItemType;
        public int RowVersion => _model.RowVersion;

        // Список вариантов для ComboBox в DataGrid.
        // Пустой список = ячейка нередактируема (нет альтернатив для этой строки).
        // Дефолтное значение (то, что было скопировано из справочника)
        // гарантированно присутствует в списке — см. BuildRepairMethodOptions,
        // иначе пользователь не сможет вернуться к нему после выбора другого варианта.
        public ObservableCollection<RecommendedRepairMethodAlternative> RepairMethodOptions { get; }

        public ObservableCollection<string> ActualValueOptions =>
            ItemType == MeasurementMapItemType.YesNo
                ? new ObservableCollection<string> {string.Empty, _model.NominalValueName, _model.AlternateNominalValueName}
                : new ObservableCollection<string>();

        public bool IsActualValueFreeText => ActualValueOptions.Count == 0;
        public bool DefectStateChanged => IsDefectState(_model.ActualValue) != IsDefectState(_actualValue);
        public bool DefectIsNowPresent => IsDefectState(_actualValue);

        private bool IsDefectState(string actualValue) => ItemType == MeasurementMapItemType.YesNo &&
                actualValue == _model.AlternateNominalValueName;

        // Можно ли редактировать ячейку метода ремонта для этой строки.
        // True только если задан хотя бы один вариант на выбор и он не пустой.
        public bool IsRepairMethodEditable => RepairMethodOptions.Count > 0;

        private int? _actualRecommendedRepairMethodId;
        public int? ActualRecommendedRepairMethodId
        {
            get { return _actualRecommendedRepairMethodId; }
            set
            {
                _actualRecommendedRepairMethodId = value == -1 ? null : value;
                NotifyPropertyChanged(nameof(ActualRecommendedRepairMethodId));
                NotifyPropertyChanged(nameof(ActualRecommendedRepairMethodName));
                NotifyPropertyChanged(nameof(HasChanges));
            }
        }

        // Отображаемое имя — для readonly-режима (если альтернатив нет)
        // и как fallback на случай несовпадения с ItemsSource.
        public string ActualRecommendedRepairMethodName =>
            RepairMethodOptions.FirstOrDefault(x => x.RecommendedRepairMethodId == _actualRecommendedRepairMethodId)
                ?.RecommendedRepairMethodName
            ?? _model.ActualRecommendedRepairMethodName;

        private string _actualValue;
        public string ActualValue
        {
            get { return _actualValue; }
            set
            {
                _actualValue = value;
                NotifyPropertyChanged(nameof(ActualValue));
                NotifyPropertyChanged(nameof(HasChanges));
            }
        }

        private string _markOfWorkCompletion;
        public string MarkOfWorkCompletion
        {
            get { return _markOfWorkCompletion; }
            set
            {
                _markOfWorkCompletion = ComboBoxHelper.ToFilterValue(value);
                NotifyPropertyChanged(nameof(MarkOfWorkCompletion));
                NotifyPropertyChanged(nameof(HasChanges));

                MarkOfWorkCompletionBy = _user.Name;
            }
        }

        private string _markOfWorkCompletionBy;
        public string MarkOfWorkCompletionBy
        {
            get { return _markOfWorkCompletionBy; }
            set
            {
                _markOfWorkCompletionBy = value;
                NotifyPropertyChanged(nameof(MarkOfWorkCompletionBy));
            }
        }

        public ObservableCollection<string> MarkOfWorkCompletionOptions { get; }

        // Были ли изменения относительно последнего сохранения
        public bool HasChanges =>
            _actualValue != _model.ActualValue ||
            _actualRecommendedRepairMethodId != _model.ActualRecommendedRepairMethodId ||
            _markOfWorkCompletion != _model.MarkOfWorkCompletion;

        public MeasurementMapItemViewModel(MeasurementMapItem model, CustomIdentity user)
        {
            _model = model;
            _user = user;

            _actualValue = model.ActualValue;

            _actualRecommendedRepairMethodId = model.ActualRecommendedRepairMethodId;
            RepairMethodOptions = BuildRepairMethodOptions(model);

            MarkOfWorkCompletionOptions = BuildMarkOfWorkCompletionOptions();
            _markOfWorkCompletion = model.MarkOfWorkCompletion;
            _markOfWorkCompletionBy = model.MarkOfWorkCompletionBy;
        }

        // Применить текущие значения обратно в модель (перед сохранением)
        public MeasurementMapItem ToModel()
        {
            _model.ActualValue = _actualValue;
            _model.ActualRecommendedRepairMethodId = _actualRecommendedRepairMethodId;
            _model.ActualRecommendedRepairMethodName = ActualRecommendedRepairMethodName;
            _model.MarkOfWorkCompletion = _markOfWorkCompletion;
            _model.MarkOfWorkCompletionBy = _markOfWorkCompletionBy;
            return _model;
        }

        // Сбросить флаг изменений после успешного сохранения
        public void AcceptChanges()
        {
            // Синхронизируем модель — HasChanges станет false
            _model.ActualValue = _actualValue;
            _model.ActualRecommendedRepairMethodId = _actualRecommendedRepairMethodId;
            _model.ActualRecommendedRepairMethodName = ActualRecommendedRepairMethodName;
            _model.MarkOfWorkCompletion = _markOfWorkCompletion;
            _model.MarkOfWorkCompletionBy = _markOfWorkCompletionBy;
            NotifyPropertyChanged(nameof(HasChanges));
        }

        // Гарантирует, что текущее значение метода ремонта присутствует
        // в списке альтернатив, даже если админ забыл явно добавить его
        // в таблицу MeasurementMapDictionaryItemRepairMethodAlternative.
        // Без этого пользователь, выбрав другое значение, не смог бы
        // вернуться к исходному через ComboBox.
        private ObservableCollection<RecommendedRepairMethodAlternative> BuildRepairMethodOptions(MeasurementMapItem model)
        {
            var options = new List<RecommendedRepairMethodAlternative>();

            if (model.RepairMethodAlternatives != null)
                options.AddRange(model.RepairMethodAlternatives);

            var hasCurrent = model.RecommendedRepairMethodId.HasValue &&
                             options.Any(x => x.RecommendedRepairMethodId == model.RecommendedRepairMethodId.Value);

            if (model.RecommendedRepairMethodId.HasValue &&
                model.ItemType == MeasurementMapItemType.Numeric &&
                !hasCurrent)
            {
                options.Insert(0, new RecommendedRepairMethodAlternative
                {
                    RecommendedRepairMethodId = model.RecommendedRepairMethodId.Value,
                    RecommendedRepairMethodName = model.RecommendedRepairMethodName
                });
            }

            if (options.Any())
                options.Insert(0, new RecommendedRepairMethodAlternative()
                {
                    RecommendedRepairMethodId = -1,
                    RecommendedRepairMethodName = string.Empty
                });

            return new ObservableCollection<RecommendedRepairMethodAlternative>(options);
        }

        private ObservableCollection<string> BuildMarkOfWorkCompletionOptions()
        {
            return ComboBoxHelper.WithNoneOption(new List<string>()
            {
                "Выполнено",
                "Устранено"
            });
        }

        static class ComboBoxHelper
        {
            const string NoneOption = "";

            public static ObservableCollection<string> WithNoneOption(IReadOnlyList<string> items)
            {
                var result = new ObservableCollection<string>() { NoneOption };
                foreach (var item in items)
                {
                    result.Add(item);
                }

                return result;
            }

            public static string ToFilterValue(string selected) =>
                selected == NoneOption ? null : selected;
        }
    }
}