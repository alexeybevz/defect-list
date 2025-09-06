using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Commands.BomHeaderCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class BomHeaderDetailsFormViewModel : ViewModel, IDataErrorInfo
    {
        public ICommand SubmitCommand { get; }
        public ICommand OpenChoiceRootItemFormCommand { get; }

        public BomHeaderDetailsFormViewModel() { }

        public BomHeaderDetailsFormViewModel(ProductsStore productsStore, RootItemsStore rootItemsStore, ICommand submitCommand)
        {
            SubmitCommand = submitCommand;
            OpenChoiceRootItemFormCommand = new OpenChoiceRootItemFormCommand(this, productsStore, rootItemsStore);

            DateOfSpecif = DateTime.Now.Date;
            ContractDateOpen = DateTime.Now.Date;
        }

        private string _orders;
        public string Orders
        {
            get
            {
                return _orders;
            }
            set
            {
                _orders = value;
                NotifyPropertyChanged(nameof(Orders));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private string _contract;
        public string Contract
        {
            get
            {
                return _contract;
            }
            set
            {
                _contract = value;
                NotifyPropertyChanged(nameof(Contract));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private DateTime? _contractDateOpen;
        public DateTime? ContractDateOpen
        {
            get
            {
                return _contractDateOpen;
            }
            set
            {
                _contractDateOpen = value;
                NotifyPropertyChanged(nameof(ContractDateOpen));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private string _serialNumber;
        public string SerialNumber
        {
            get
            {
                return _serialNumber;
            }
            set
            {
                _serialNumber = value;
                NotifyPropertyChanged(nameof(SerialNumber));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private string _serialNumberAfterRepair;
        public string SerialNumberAfterRepair
        {
            get
            {
                return _serialNumberAfterRepair;
            }
            set
            {
                _serialNumberAfterRepair = value;
                NotifyPropertyChanged(nameof(SerialNumberAfterRepair));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private RootItem _rootItem;
        public RootItem RootItem
        {
            get
            {
                return _rootItem;
            }
            set
            {
                _rootItem = value;
                NotifyPropertyChanged(nameof(RootItem));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private string _comment;
        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
                NotifyPropertyChanged(nameof(Comment));
            }
        }

        private DateTime? _dateOfSpecif;
        public DateTime? DateOfSpecif
        {
            get
            {
                return _dateOfSpecif;
            }
            set
            {
                _dateOfSpecif = value;
                NotifyPropertyChanged(nameof(DateOfSpecif));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private DateTime? _dateOfPreparation;
        public DateTime? DateOfPreparation
        {
            get
            {
                return _dateOfPreparation;
            }
            set
            {
                _dateOfPreparation = value;
                NotifyPropertyChanged(nameof(DateOfPreparation));
            }
        }

        private string _headerType;
        public string HeaderType
        {
            get
            {
                return _headerType;
            }
            set
            {
                _headerType = value;
                NotifyPropertyChanged(nameof(HeaderType));
                NotifyPropertyChanged(nameof(CanSubmit));
            }
        }

        private bool _isSubmitting;
        public bool IsSubmitting
        {
            get
            {
                return _isSubmitting;
            }
            set
            {
                _isSubmitting = value;
                NotifyPropertyChanged(nameof(IsSubmitting));
            }
        }

        public bool CanSubmit =>
            !string.IsNullOrEmpty(Orders) &&
            !string.IsNullOrEmpty(SerialNumber) &&
            !string.IsNullOrEmpty(SerialNumberAfterRepair) &&
            RootItem != null &&
            !string.IsNullOrEmpty(Contract) &&
            ContractDateOpen != null &&
            DateOfSpecif != null &&
            !string.IsNullOrEmpty(HeaderType) &&
            IsAllValidProperties(this);

        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged(nameof(ErrorMessage));
                NotifyPropertyChanged(nameof(HasErrorMessage));
            }
        }

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public List<string> PossibleHeaderTypes => new List<string>()
        {
            "Ремонт",
            "Рекламация"
        };

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(Orders):
                        if (Orders?.Length > 100)
                            error = $"Поле 'Заказ'. Превышена допустимая длина строки - {100}";
                        break;
                    case nameof(Contract):
                        if (Contract?.Length > 100)
                            error = $"Поле 'Договор'. Превышена допустимая длина строки - {100}";
                        break;
                    case nameof(SerialNumber):
                        if (SerialNumber?.Length > 100)
                            error = $"Поле 'Серийный номер'. Превышена допустимая длина строки - {100}";
                        break;
                    case nameof(SerialNumberAfterRepair):
                        if (SerialNumberAfterRepair?.Length > 100)
                            error = $"Поле 'Серийный номер после ремонта'. Превышена допустимая длина строки - {100}";
                        break;
                    case nameof(Comment):
                        if (Comment?.Length > 200)
                            error = $"Поле 'Комментарий'. Превышена допустимая длина строки - {200}";
                        break;
                    case nameof(DateOfSpecif):
                        if (DateOfSpecif < new DateTime(2000, 01, 01))
                            error = "Поле 'Состав на дату'. Указана некорректная дата";
                        break;
                }

                return error;
            }
        }

        public bool IsAllValidProperties(BomHeaderDetailsFormViewModel vm)
        {
            var columns = new List<string>
            {
                vm[nameof(Orders)],
                vm[nameof(Contract)],
                vm[nameof(SerialNumber)],
                vm[nameof(SerialNumberAfterRepair)],
                vm[nameof(Comment)],
                vm[nameof(DateOfSpecif)]
            };

            var errors = columns.Where(e => !string.IsNullOrEmpty(e)).ToList();
            return errors.Count == 0;
        }
    }
}