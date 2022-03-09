using System;
using CommonTools.Logging;
using StateMachine.Core.Exceptions;
using StateMachine.Core.Machines;
using UnityEngine;

namespace StateMachine.Core
{
    /// <summary>
    /// Состояние
    /// </summary>
    /// <typeparam name="TContext">Контекст состояния</typeparam>
    public abstract class State<TContext> where TContext : MachineContext
    {
        private readonly StateUpdateEntity<TContext>[] _outputs;
        private readonly BeautyLog _logger;

        private int _currentOutput;
        
        protected readonly TContext Context;

        public State(TContext context, int outputs = 1, bool enableLogging = false)
        {
            Context = context;
            _outputs = new StateUpdateEntity<TContext>[outputs];

            if (enableLogging)
            {
                _logger = new BeautyLog("State", Color.yellow);
            }
        }
        
        public State<TContext> Update()
        {
            OnUpdate();
            
            foreach (var output in _outputs)
            {
                if (output.Condition == null || !output.Condition())
                {
                    continue;
                }
                
                _logger?.Log($"state condition met for state [{output.State.GetHashCode()}]");
                return output.State;
            }

            return this;
        }

        public void On(Func<bool> condition, State<TContext> state)
        {
            if (_currentOutput >= _outputs.Length)
            {
                throw new StateInitializationException("State outputs overflow. Create more outputs in constructor");
            }

            _outputs[_currentOutput] = new StateUpdateEntity<TContext>(condition, state);
            _currentOutput++;
        }

        public void Enter()
        {
            _logger?.Log($"state enter called [{this.GetHashCode()}][{Context.GetHashCode()}]");
            OnEnter();
        }

        public void Exit()
        {
            _logger?.Log($"state exit called [{this.GetHashCode()}][{Context.GetHashCode()}]");
            OnExit();
        }

        public State<TContext> GetOutputState(int index) => _outputs[index].State;

        protected abstract void OnEnter();
        protected abstract void OnExit();
        protected abstract void OnUpdate();
    }
}