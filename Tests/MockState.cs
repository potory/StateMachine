using StateMachine.Core;
using StateMachine.Core.Machines;
using StateMachine.Core.Machines.Sync;

namespace StateMachine.Tests
{
    public enum MockStatus
    {
        Idle,
        Entered,
        Exited
    }
    
    public class MockState : SyncState<MockContext>
    {
        public MockStatus Status { get; private set; }
        public int Counter { get; set; }
        
        public MockState(MockContext context, int outputs = 1, bool enableLogging = false) : base(context, outputs, enableLogging)
        {
            
        }

        protected override void OnEnter()
        {
            Status = MockStatus.Entered;
        }

        protected override void OnExit()
        {
            Status = MockStatus.Exited;
        }

        protected override void OnUpdate()
        {
            Counter++;
        }
    }

    public class MockContext : MachineContext
    {
    }
}