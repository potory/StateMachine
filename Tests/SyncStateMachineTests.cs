using NUnit.Framework;
using StateMachine.Core;
using StateMachine.Core.Machines;
using StateMachine.Core.Machines.Sync;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace StateMachine.Tests
{
    public class SyncStateMachineTests
    {
        protected class MockCondition
        {
            internal int Value = -1;
        }

        [Test]
        public void SyncStateMachine_SyncStateIndexing()
        {
            var mockConditionA = new MockCondition();
            var mockContextA = new MockContext();
            var mockSyncContext = new SyncContext(true);

            var chainA = CreateStateChain(mockContextA, mockConditionA);

            var machineA = new SyncMachine<MockContext>(chainA, mockContextA, mockSyncContext, 0);

            for (int i = 0; i < chainA.Length; i++)
            {
                Assert.AreEqual(i, chainA[i].ID);
            }
        }

        [Test]
        public void SyncStateMachine_Synchronize_OneSided()
        {
            var mockConditionA = new MockCondition();
            var mockConditionB = new MockCondition();
            
            var mockContextA = new MockContext();
            var mockContextB = new MockContext();
            
            var mockSyncContext = new SyncContext(true);

            var chainA = CreateStateChain(mockContextA, mockConditionA);
            var chainB = CreateStateChain(mockContextB, mockConditionB);
            
            var machineA = new SyncMachine<MockContext>(chainA, mockContextA, mockSyncContext, 0);
            var machineB = new SyncMachine<MockContext>(chainB, mockContextB, mockSyncContext, 1);

            machineA.Set(chainA[0]);
            machineB.Set(chainB[0]);
            
            machineA.BeginSync();
            machineB.BeginSync();

            for (var i = 0; i < 10; i++)
            {
                switch (i)
                {
                    case 3:
                        mockConditionA.Value = 1;
                        break;
                    case 4:
                        Assert.AreEqual(chainA[0].GetOutputState(1), machineA.Current);
                        Assert.AreEqual(chainB[0].GetOutputState(1), machineB.Current);
                        break;
                    case 5:
                        mockConditionA.Value = 4;
                        break;
                    case 6:
                        Assert.AreEqual(chainA[0].GetOutputState(1).GetOutputState(0), machineA.Current);
                        Assert.AreEqual(chainB[0].GetOutputState(1).GetOutputState(0), machineB.Current);
                        break;
                }
                
                machineA.Update();
                machineB.Update();
            }
            
            machineA.FinishSync();
            machineB.FinishSync();
        }
        
        [Test]
        public void SyncStateMachine_Synchronize_TwoSided()
        {
            var mockConditionA = new MockCondition();
            var mockConditionB = new MockCondition();
            
            var mockContextA = new MockContext();
            var mockContextB = new MockContext();
            
            var mockSyncContext = new SyncContext(true);

            var chainA = CreateStateChain(mockContextA, mockConditionA);
            var chainB = CreateStateChain(mockContextB, mockConditionB);
            
            var machineA = new SyncMachine<MockContext>(chainA, mockContextA, mockSyncContext, 0);
            var machineB = new SyncMachine<MockContext>(chainB, mockContextB, mockSyncContext, 1);

            machineA.Set(chainA[0]);
            machineB.Set(chainB[0]);
            
            machineA.BeginSync();
            machineB.BeginSync();

            for (var i = 0; i < 10; i++)
            {
                switch (i)
                {
                    case 3:
                        mockConditionA.Value = 1;
                        break;
                    case 4:
                        Assert.AreEqual(chainA[0].GetOutputState(1), machineA.Current);
                        Assert.AreEqual(chainB[0].GetOutputState(1), machineB.Current);
                        break;
                    case 5:
                        mockConditionB.Value = 4;
                        break;
                    case 6:
                        Assert.AreEqual(chainA[0].GetOutputState(1).GetOutputState(0), machineA.Current);
                        Assert.AreEqual(chainB[0].GetOutputState(1).GetOutputState(0), machineB.Current);
                        break;
                }
                
                machineA.Update();
                machineB.Update();
            }
            
            machineA.FinishSync();
            machineB.FinishSync();
        }
        
        [Test]
        public void SyncStateMachine_Synchronize_Independent()
        {
            var mockConditionA = new MockCondition();
            var mockContextA = new MockContext();
            var mockSyncContext = new SyncContext(true);

            var chainA = CreateStateChain(mockContextA, mockConditionA);
            var machineA = new SyncMachine<MockContext>(chainA, mockContextA, mockSyncContext, 0);

            machineA.Set(chainA[0]);
            machineA.BeginSync();

            for (var i = 0; i < 10; i++)
            {
                switch (i)
                {
                    case 2:
                        Assert.AreNotEqual(chainA[2], machineA.Current);
                        mockSyncContext.SendMessage(new SyncMessage(1, SyncAction.Move, 2));
                        Assert.AreEqual(chainA[2], machineA.Current);
                        break;
                    case 4:
                        Assert.AreEqual(chainA[2], machineA.Current);
                        mockSyncContext.SendMessage(new SyncMessage(1, SyncAction.Move, 4));
                        Assert.AreEqual(chainA[4], machineA.Current);
                        break;
                }

                machineA.Update();
            }
            
            machineA.FinishSync();
        }

        private static SyncState<MockContext>[] CreateStateChain(MockContext context, MockCondition condition, bool enableLogging = false)
        {
            var arr = new SyncState<MockContext>[6];
            
            arr[0] = new MockState(context, 3, enableLogging);
            arr[1] = new MockState(context, 0, enableLogging);
            arr[2] = new MockState(context, 2, enableLogging);
            arr[3] = new MockState(context, 0, enableLogging);
            
            arr[0].On(() => condition.Value == 0, arr[1]);
            arr[0].On(() => condition.Value == 1, arr[2]);
            arr[0].On(() => condition.Value == 2, arr[3]);

            arr[4] = new MockState(context, 0, enableLogging);
            arr[5] = new MockState(context, 0, enableLogging);
            
            arr[2].On(() => condition.Value == 4, arr[4]);
            arr[2].On(() => condition.Value == 5, arr[5]);

            return arr;
        }
    }
}