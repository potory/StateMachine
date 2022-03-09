using System;
using System.Linq;
using CommonTools.Extensions;
using StateMachine.Core.Exceptions;

namespace StateMachine.Core.Machines.Sync
{
    /// <summary>
    /// Синхронизированная стейт-машина
    /// </summary>
    /// <typeparam name="TContext">Контекст состояний</typeparam>
    public class SyncMachine<TContext> : Machine<TContext> where TContext : MachineContext
    {
        private readonly int _id;
        private readonly SyncContext _sync;
        private readonly SyncState<TContext>[] _states;

        private SyncStatus _status;

        public SyncMachine(SyncState<TContext>[] states, TContext context, SyncContext sync, int id) : base(context)
        {
            _states = states.For((index, x) => x.ID = index);

            _id = id;
            _sync = sync;

            _sync.MessageReceived += OnMessageReceived;

            StateEntered += OnStateEntered;
        }

        public void BeginSync() => _status = SyncStatus.Active;
        public void FinishSync() => _status = SyncStatus.Inactive;

        private void OnStateEntered(State<TContext> state)
        {
            if (state == null)
            {
                _sync.SendMessage(new SyncMessage(_id, SyncAction.Move, -1));
                return;
            }
            
            var syncState = state as SyncState<TContext> ?? throw new Exception();
            
            if (_status == SyncStatus.Inactive)
            {
                return;
            }

            _sync.SendMessage(new SyncMessage(_id, SyncAction.Move, syncState.ID));
        }

        private void OnMessageReceived(SyncMessage message)
        {
            if (message.Id == _id)
            {
                return;
            }

            if (message.Action == SyncAction.Move)
            {
                MoveSync(message);
            }
        }

        private void MoveSync(SyncMessage message)
        {
            var id = (int) message.Bag;
            id.MustBeIn<int, StateSynchronizationException>(-1, int.MaxValue);

            if (id == -1)
            {
                Move(null, silent: true);
                return;
            }
            
            var state = _states.First(x => x.ID == id);

            Move(state, silent: true);
        }
    }
}