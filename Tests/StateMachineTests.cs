using NUnit.Framework;
using StateMachine.Core;
using StateMachine.Core.Machines;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace StateMachine.Tests
{
    public class StateMachineTests
    {
        [Test]
        public void StateMachine_Creates()
        {
            var stateMachine = new Machine<MockContext>(new MockContext());
            
            Assert.IsNotNull(stateMachine);
        }

        [Test]
        public void StateMachine_State_SetNull_Empty()
        {
            var context = new MockContext();
            
            var stateMachine = new Machine<MockContext>(context);
            
            stateMachine.Set(null);
        }
        
        [Test]
        public void StateMachine_State_SetNull_Filled()
        {
            var context = new MockContext();

            var stateMachine = new Machine<MockContext>(context);

            var state = new MockState(context);
            Assert.AreEqual(MockStatus.Idle, state.Status);

            stateMachine.Set(state);
            
            Assert.AreEqual(MockStatus.Entered, state.Status);
            
            stateMachine.Set(null);
            
            Assert.AreEqual(MockStatus.Exited, state.Status);
        }
        
        [Test]
        public void StateMachine_State_SetEvents()
        {
            var context = new MockContext();

            var stateMachine = new Machine<MockContext>(context);
            
            var entered = 0;
            var exited = 0;
            
            stateMachine.StateEntered += (_) => entered++;
            stateMachine.StateEntered += (_) => exited++;
            
            var state = new MockState(context);

            stateMachine.Set(state);
            stateMachine.Set(null);
            
            Assert.AreEqual(1, entered);
            Assert.AreEqual(1, exited);
        }
        
        [Test]
        public void StateMachine_State_SetNotNull()
        {
            var context = new MockContext();
            
            var stateMachine = new Machine<MockContext>(context);
            var state = new MockState(context);
            Assert.AreEqual(MockStatus.Idle, state.Status);
            
            stateMachine.Set(state);

            Assert.IsNotNull(stateMachine);
            Assert.AreEqual(MockStatus.Entered, state.Status);
        }
        
        [Test]
        public void StateMachine_State_Update_Empty()
        {
            var context = new MockContext();
            
            var stateMachine = new Machine<MockContext>(context);
            stateMachine.Set(null);

            for (var i = 0; i < 10; i++)
            {
                stateMachine.Update();
            }
        }
        
        [Test]
        public void StateMachine_State_Update_Filled()
        {
            var updates = 0;
            var context = new MockContext();
            
            var stateMachine = new Machine<MockContext>(context);
            stateMachine.StateUpdated += (_) => updates++;
            var state = new MockState(context);
            Assert.AreEqual(MockStatus.Idle, state.Status);
            
            stateMachine.Set(state);

            Assert.IsNotNull(stateMachine);
            Assert.AreEqual(MockStatus.Entered, state.Status);
            Assert.AreEqual(0, state.Counter);

            const int iterations = 10;

            for (var i = 0; i < iterations; i++)
            {
                stateMachine.Update();
            }
            
            Assert.AreEqual(iterations, updates);
            Assert.AreEqual(iterations, state.Counter);
        }
        
        [Test]
        public void StateMachine_State_Update_Transition_Simple()
        {
            const int iterations = 10;
            var context = new MockContext();
            
            var stateMachine = new Machine<MockContext>(context);
            
            var stateA = CreateState(context);
            var stateB = CreateState(context);
            
            stateA.On(() => stateA.Counter == iterations / 2, stateB);

            stateMachine.Set(stateA);

            Assert.IsNotNull(stateMachine);
            Assert.AreEqual(MockStatus.Entered, stateA.Status);
            Assert.AreEqual(0, stateA.Counter);

            for (var i = 0; i < iterations; i++)
            {
                stateMachine.Update();
            }
            
            Assert.AreEqual(MockStatus.Exited, stateA.Status);
            Assert.AreEqual(MockStatus.Entered, stateB.Status);

            Assert.AreEqual(iterations/2, stateA.Counter);
            Assert.AreEqual(iterations/2, stateB.Counter);
        }
        

        private static MockState CreateState(MockContext context)
        {
            var state = new MockState(context);
            Assert.AreEqual(MockStatus.Idle, state.Status);
            return state;
        }
    }
}
