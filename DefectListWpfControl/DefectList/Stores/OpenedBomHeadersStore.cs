using System;
using System.Collections.Generic;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;

namespace DefectListWpfControl.DefectList.Stores
{
    public class OpenedBomHeadersStore
    {
        private readonly SelectedBomHeaderStore _selectedBomHeaderStore;
        private readonly Dictionary<int, DefectListItemWindow> _openedBomHeaders;

        public Action RequestOnLoadBomHeaders;

        public OpenedBomHeadersStore(SelectedBomHeaderStore selectedBomHeaderStore)
        {
            _selectedBomHeaderStore = selectedBomHeaderStore;
            _openedBomHeaders = new Dictionary<int, DefectListItemWindow>();
        }

        public DefectListItemWindow GetOpenedForm(int bomHeaderId)
        {
            return _openedBomHeaders.ContainsKey(bomHeaderId) ? _openedBomHeaders[bomHeaderId] : null;
        }

        public DefectListItemWindow Add(int bomHeaderId)
        {
            var form = CreateForm();
            _openedBomHeaders.Add(bomHeaderId, form);
            return form;
        }

        private DefectListItemWindow CreateForm()
        {
            var form = DefectListIocKernel.Get<DefectListItemWindow>();
            ((DefectListItemViewModel)form.DataContext).BomHeader = _selectedBomHeaderStore.SelectedBomHeader;

            form.Closed += (o, args) =>
            {
                var window = (DefectListItemWindow) o;
                var vm = (DefectListItemViewModel) window.DataContext;

                _openedBomHeaders.Remove(vm.BomHeader.BomId);
                RequestOnLoadBomHeaders?.Invoke();
            };

            return form;
        }
    }
}