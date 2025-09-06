using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Commands;
using DefectListDomain.Queries;
using BomHeaderSubscriber = DefectListDomain.Models.BomHeaderSubscriber;

namespace DefectListWpfControl.DefectList.Stores
{
    public class BomHeaderSubscribersStore
    {
        private readonly IGetAllBomHeaderSubscribersQuery _getAllBomHeaderSubscribersQuery;
        private readonly ISubscribeUserOnBomHeaderCommand _subscribeUserOnBomHeaderCommand;
        private readonly IUnSubscribeUserOnBomHeaderCommand _unSubscribeUserOnBomHeaderCommand;

        private readonly List<BomHeaderSubscriber> _bomHeaderSubscribers;
        public IEnumerable<BomHeaderSubscriber> BomHeaderSubscribers => _bomHeaderSubscribers;

        public event Action BomHeaderSubscribersLoaded;
        public event Action<BomHeaderSubscriber> UserSubscribedOnBomHeader;
        public event Action<BomHeaderSubscriber> UserUnSubscribedOnBomHeader;

        public BomHeaderSubscribersStore(
            IGetAllBomHeaderSubscribersQuery getAllBomHeaderSubscribersQuery,
            ISubscribeUserOnBomHeaderCommand subscribeUserOnBomHeaderCommand,
            IUnSubscribeUserOnBomHeaderCommand unSubscribeUserOnBomHeaderCommand)
        {
            _getAllBomHeaderSubscribersQuery = getAllBomHeaderSubscribersQuery;
            _subscribeUserOnBomHeaderCommand = subscribeUserOnBomHeaderCommand;
            _unSubscribeUserOnBomHeaderCommand = unSubscribeUserOnBomHeaderCommand;

            _bomHeaderSubscribers = new List<BomHeaderSubscriber>();
        }

        public async Task Load()
        {
            IEnumerable<BomHeaderSubscriber> bomHeaderSubscribers = await _getAllBomHeaderSubscribersQuery.Execute();

            _bomHeaderSubscribers.Clear();
            _bomHeaderSubscribers.AddRange(bomHeaderSubscribers);

            BomHeaderSubscribersLoaded?.Invoke();
        }

        public async Task Subscribe(BomHeaderSubscriber bomHeaderSubscriber)
        {
            await _subscribeUserOnBomHeaderCommand.Execute(bomHeaderSubscriber);
            _bomHeaderSubscribers.Add(bomHeaderSubscriber);
            UserSubscribedOnBomHeader?.Invoke(bomHeaderSubscriber);
        }

        public async Task UnSubscribe(BomHeaderSubscriber bomHeaderSubscriber)
        {
            await _unSubscribeUserOnBomHeaderCommand.Execute(bomHeaderSubscriber);
            _bomHeaderSubscribers.RemoveAll(x => x.BomId == bomHeaderSubscriber.BomId && x.UserId == bomHeaderSubscriber.UserId);
            UserUnSubscribedOnBomHeader?.Invoke(bomHeaderSubscriber);
        }
    }
}