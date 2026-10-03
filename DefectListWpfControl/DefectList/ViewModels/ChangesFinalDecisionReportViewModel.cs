using System;
using System.Collections.Generic;
using DefectListWpfControl.DefectList.Commands.ReportCommands;
using DefectListWpfControl.DefectList.Commons;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class ChangesFinalDecisionReportViewModel : ViewModel
    {
        public IReadOnlyCollection<string> DetalTypOptions { get; }

        private string _selectedDetalTyp;
        public string SelectedDetalTyp
        {
            get { return _selectedDetalTyp; }
            set
            {
                _selectedDetalTyp = value;
                NotifyPropertyChanged(nameof(SelectedDetalTyp));
            }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyPropertyChanged(nameof(StartDate));
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                NotifyPropertyChanged(nameof(EndDate));
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged(nameof(IsLoading));
            }
        }

        private string _executingStatus;
        public string ExecutingStatus
        {
            get { return _executingStatus; }
            set
            {
                _executingStatus = value;
                NotifyPropertyChanged(nameof(ExecutingStatus));
            }
        }

        public AsyncCommandBase CreateReportCommand { get; }

        public ChangesFinalDecisionReportViewModel(BomItemsStore bomItemsStore, ProductsStore productsStore, string userName)
        {
            var currentDate = DateTime.Now;
            StartDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            EndDate = StartDate.AddMonths(1).AddSeconds(-1);

            DetalTypOptions = ComboBoxHelper.WithAllOption(new List<string>()
            {
                "издел",
                "дет",
                "докум",
                "матер",
                "покуп",
                "сб.ед",
                "литье"
            });
            SelectedDetalTyp = ComboBoxHelper.AllOption;

            CreateReportCommand = new ChangesFinalDecisionReportCommand(this, bomItemsStore, productsStore, userName);
        }
    }
}