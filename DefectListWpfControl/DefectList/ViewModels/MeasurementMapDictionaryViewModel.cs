using System;
using System.Collections.Generic;
using DefectListDomain.Models;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class MeasurementMapDictionaryViewModel : ViewModel
    {
        private readonly MeasurementMapDictionary _model;

        public int Id => _model.Id;
        public string Name => _model.Name;
        public int Code_LSF82 => _model.Code_LSF82;
        public int MeasurementsMapTypeFormId => _model.MeasurementsMapTypeFormId;
        public string MeasurementsMapTypeFormName => _model.MeasurementsMapTypeFormName;
        public int Version => _model.Version;
        public bool IsActive => _model.IsActive;
        public string Comment => _model.Comment;
        public DateTime CreateDate => _model.CreateDate;
        public string CreatedBy => _model.CreatedBy;
        public DateTime RecordDate => _model.RecordDate;
        public string UpdatedBy => _model.UpdatedBy;
        public IEnumerable<MeasurementMapDictionaryItem> Items => _model.Items;
        public string BoundRootItemNames => _model.BoundRootItemNames;

        private string _detal;
        public string Detal
        {
            get { return _detal; }
            set { _detal = value; NotifyPropertyChanged(nameof(Detal)); }
        }

        public MeasurementMapDictionaryViewModel(MeasurementMapDictionary model)
        {
            _model = model;
        }
    }
}