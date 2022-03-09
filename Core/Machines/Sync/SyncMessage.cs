
using System;

namespace StateMachine.Core.Machines.Sync
{
    /// <summary>
    /// Сообщение синхронизации, контейнер для информации о действии удаленной стейт-машины
    /// </summary>
    [Serializable]
    public readonly struct SyncMessage
    {
        public readonly int Id;
        public readonly SyncAction Action;
        public readonly object Bag;

        public SyncMessage(int id, SyncAction action, object bag)
        {
            Action = action;
            Bag = bag;
            Id = id;
        }
    }
}