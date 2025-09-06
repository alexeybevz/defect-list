using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DefectListBusinessLogic.Services;
using DefectListDomain.ExternalData;
using DefectListDomain.Models;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class ConsolidateBomItemViewModel : ViewModel
    {
        private readonly IGetAllProductDtoQuery _getAllProductDtoQuery;
        private readonly IEnumerable<KeyValuePair<int, string>> _depts;

        public ConsolidateBomItemViewModel(IGetAllProductDtoQuery getAllProductDtoQuery, IEnumerable<KeyValuePair<int, string>> depts)
        {
            _getAllProductDtoQuery = getAllProductDtoQuery;
            _depts = depts;

            TargetDetalUpdated += TargetDetalUpdatedAction;
        }

        private async void TargetDetalUpdatedAction(string detal)
        {
            try
            {
                ProductId = (await _getAllProductDtoQuery.ExecuteByDetals(SpecifKeyCreator.CreateKey(detal)))?.Id ?? 0;
                TargetDetals = SpecifKeyCreator.CreateKey(detal);
                IsSelected = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Произошла ошибка при обращении к базе данных во время работы с полем 'Обозначение для запуска МК в АСУП'");
            }
        }

        private readonly Action<string> TargetDetalUpdated;

        public string RowGuid { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value && _qtyForLaunch > 0 && IsExistsTargetDetal;
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }

        private string _detal;
        public string Detal
        {
            get { return _detal; }
            set
            {
                _detal = value;
                NotifyPropertyChanged(nameof(Detal));
            }
        }

        private string _detalIma;
        public string DetalIma
        {
            get { return _detalIma; }
            set
            {
                _detalIma = value;
                NotifyPropertyChanged(nameof(DetalIma));
            }
        }

        private string _detalTyp;
        public string DetalTyp
        {
            get { return _detalTyp; }
            set
            {
                _detalTyp = value;
                NotifyPropertyChanged(nameof(DetalTyp));
            }
        }

        private decimal _qtyMnf;
        public decimal QtyMnf
        {
            get { return _qtyMnf; }
            set
            {
                _qtyMnf = value;
                NotifyPropertyChanged(nameof(QtyMnf));
                SetQtyForLaunch();
            }
        }

        private decimal _qtyLaunched;
        public decimal QtyLaunched
        {
            get { return _qtyLaunched; }
            set
            {
                _qtyLaunched = value;
                NotifyPropertyChanged(nameof(QtyLaunched));
                SetQtyForLaunch();
            }
        }

        private decimal _qtyForLaunch;
        public decimal QtyForLaunch
        {
            get { return _qtyForLaunch; }
            set
            {
                if (BomItems.Count() > 1 && QtyMnf != value)
                    _qtyForLaunch = 0;
                else
                    _qtyForLaunch = value;

                IsSelected = true;
                NotifyPropertyChanged(nameof(QtyForLaunch));
            }
        }

        private void SetQtyForLaunch()
        {
            var value = QtyMnf - QtyLaunched;
            QtyForLaunch = value <= 0 ? 0 : value;
        }

        private string _targetDetals;
        public string TargetDetals
        {
            get { return _targetDetals; }
            set
            {
                _targetDetals = value;
                NotifyPropertyChanged(nameof(TargetDetals));
            }
        }

        private string _targetDetal;
        public string TargetDetal
        {
            get { return _targetDetal; }
            set
            {
                _targetDetal = value;
                NotifyPropertyChanged(nameof(TargetDetal));
                TargetDetalUpdated?.Invoke(value);
            }
        }

        private int _targetWpId;
        public int TargetWpId
        {
            get { return _targetWpId; }
            set
            {
                _targetWpId = value;
                NotifyPropertyChanged(nameof(TargetWpId));
                TargetWp = _depts.FirstOrDefault(x => x.Key == value);
            }
        }

        private KeyValuePair<int, string> _targetWp;
        public KeyValuePair<int, string> TargetWp
        {
            get { return _targetWp; }
            set
            {
                _targetWp = value;
                NotifyPropertyChanged(nameof(TargetWp));
            }
        }

        public string TargetWpName => _depts.FirstOrDefault(x => x.Key == TargetWpId).Value;

        private string _normalizeDecision;
        public string NormalizeDecision
        {
            get { return _normalizeDecision; }
            set
            {
                _normalizeDecision = value;
                NotifyPropertyChanged(nameof(NormalizeDecision));
            }
        }

        private bool _isWorkNeed;
        public bool IsWorkNeed
        {
            get { return _isWorkNeed; }
            set
            {
                _isWorkNeed = value;
                NotifyPropertyChanged(nameof(IsWorkNeed));
            }
        }

        public string IsWorkNeedName => IsWorkNeed ? "Нет" : "Да";

        private bool _isPrint;
        public bool IsPrint
        {
            get { return _isPrint; }
            set
            {
                _isPrint = value;
                NotifyPropertyChanged(nameof(IsPrint));
            }
        }
        public string IsPrintName => IsPrint ? "Да" : "Нет";

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                NotifyPropertyChanged(nameof(Comment));
            }
        }

        private string _createdRouteMap;
        public string CreatedRouteMap
        {
            get { return _createdRouteMap; }
            set
            {
                _createdRouteMap = value;
                NotifyPropertyChanged(nameof(CreatedRouteMap));
            }
        }

        private int _createdRouteMapId;
        public int CreatedRouteMapId
        {
            get { return _createdRouteMapId; }
            set
            {
                _createdRouteMapId = value;
                NotifyPropertyChanged(nameof(CreatedRouteMapId));
            }
        }

        private int _productId;

        public int ProductId
        {
            get { return _productId; }
            set
            {
                _productId = value;
                NotifyPropertyChanged(nameof(ProductId));
                NotifyPropertyChanged(nameof(IsExistsTargetDetal));
                NotifyPropertyChanged(nameof(IsExistsTargetDetalName));
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }

        public bool IsExistsTargetDetal => ProductId > 0;
        public string IsExistsTargetDetalName => IsExistsTargetDetal ? "Да" : "Нет";

        public IEnumerable<IBomItem> BomItems { get; set; }
    }
}