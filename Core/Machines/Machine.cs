using CommonTools.Extensions;

namespace StateMachine.Core.Machines
{
    /// <summary>
    /// Стейт-машина
    /// </summary>
    /// <typeparam name="TContext">Контекст состояний</typeparam>
    public class Machine<TContext> where TContext : MachineContext
    {
        public delegate void MachineEvent(State<TContext> state);

        private readonly TContext _context;

        public event MachineEvent StateEntered;
        public event MachineEvent StateExited;
        public event MachineEvent StateUpdated;
        
        public State<TContext> Current { get; private set; }

        public Machine(TContext context)
        {
            _context = context;
        }

        public void Set(State<TContext> state)
        {
            Move(state);
        }

        public void Update()
        {
            var state = Current?.Update();
            StateUpdated?.Invoke(Current);

            if (state == Current)
            {
                return;
            }
            
            Move(state);
        }

        protected void Move(State<TContext> state, bool silent = false)
        {
            Current?.Exit();
            (!silent && Current != null).Then(() => StateExited?.Invoke(Current));

            Current = state;
            Current?.Enter();
            (!silent && Current != null).Then(() => StateEntered?.Invoke(Current));
        }
    }
}
