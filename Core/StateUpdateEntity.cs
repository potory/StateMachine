using System;
using StateMachine.Core.Machines;

namespace StateMachine.Core
{
    /// <summary>
    /// Контейнер для хранения информации о связях состояний
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public readonly struct StateUpdateEntity<TContext> where TContext : MachineContext
    {
        public readonly Func<bool> Condition;
        public readonly State<TContext> State;

        public StateUpdateEntity(Func<bool> condition, State<TContext> state)
        {
            Condition = condition;
            State = state;
        }
    }
}