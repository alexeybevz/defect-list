using System;
using System.Globalization;
using System.Windows.Input;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class SplitBomItemViewModel : ViewModel
    {
        #region RawDataProperties
        private string _rawInitialQty;
        public string RawInitialQty
        {
            get { return _rawInitialQty; }
            set
            {
                _rawInitialQty = value;
                NotifyPropertyChanged(nameof(RawInitialQty));
            }
        }

        private string _rawCurrentBomItemQty;
        public string RawCurrentBomItemQty
        {
            get { return _rawCurrentBomItemQty; }
            set
            {
                _rawCurrentBomItemQty = value;
                NotifyPropertyChanged(nameof(RawCurrentBomItemQty));

                bool parseResult;
                var num = ParseStrToDecimalNumber(_rawCurrentBomItemQty, out parseResult);
                CurrentBomItemQty = parseResult ? num : 0;
            }
        }

        private string _rawNewBomItemQty;
        public string RawNewBomItemQty
        {
            get { return _rawNewBomItemQty; }
            set
            {
                _rawNewBomItemQty = value;
                NotifyPropertyChanged(nameof(RawNewBomItemQty));

                bool parseResult;
                var num = ParseStrToDecimalNumber(_rawNewBomItemQty, out parseResult);
                NewBomItemQty = parseResult ? num : 0;
            }
        }
        #endregion

        #region DataProperties
        private decimal _initialQty;
        public decimal InitialQty
        {
            get { return _initialQty; }
            set
            {
                _initialQty = value;
                NotifyPropertyChanged(nameof(InitialQty));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private decimal _currentBomItemQty;
        public decimal CurrentBomItemQty
        {
            get { return _currentBomItemQty; }
            set
            {
                _currentBomItemQty = value;
                NotifyPropertyChanged(nameof(CurrentBomItemQty));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private decimal _newBomItemQty;
        public decimal NewBomItemQty
        {
            get { return _newBomItemQty; }
            set
            {
                _newBomItemQty = value;
                NotifyPropertyChanged(nameof(NewBomItemQty));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }
        #endregion

        public bool CanSubmit => CurrentBomItemQty + NewBomItemQty == InitialQty;

        public ICommand ExecuteCommand { get; }
        public Action<decimal, decimal> BomItemSplitted;

        public SplitBomItemViewModel(decimal initialQty)
        {
            InitialQty = initialQty;
            CurrentBomItemQty = initialQty;
            NewBomItemQty = 0;

            RawInitialQty = InitialQty.ToString(CultureInfo.InvariantCulture);
            RawCurrentBomItemQty = InitialQty.ToString(CultureInfo.InvariantCulture);
            RawNewBomItemQty = NewBomItemQty.ToString(CultureInfo.InvariantCulture);

            ExecuteCommand = new DelegateCommand(delegate
            {
                BomItemSplitted?.Invoke(CurrentBomItemQty, NewBomItemQty);
            });
        }

        private decimal ParseStrToDecimalNumber(string str, out bool result)
        {
            decimal number;
            result = decimal.TryParse(str, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out number);
            return number;
        }
    }
}