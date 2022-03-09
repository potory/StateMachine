namespace StateMachine.Core.Machines.Sync
{
    /// <summary>
    /// Состояние для синхронизированной стейт-машины с идентификатором
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class SyncState<TContext> : State<TContext> where TContext : MachineContext
    {
        public int ID { get; set; }

        protected SyncState(TContext context, int outputs = 1, bool enableLogging = false) : base(context, outputs, enableLogging)
        {
            
        }
    }
}