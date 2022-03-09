using CommonTools.Logging;
using UnityEngine;

namespace StateMachine.Core.Machines.Sync
{
    /// <summary>
    /// Контекст синхронизации, передает сообщения в синхронизированную машину
    /// </summary>
    public class SyncContext
    {
        private readonly BeautyLog _logger;

        public delegate void SyncMessageEvent(SyncMessage message);

        public event SyncMessageEvent MessageReceived;

        public SyncContext(bool loggingEnabled = false)
        {
            if (loggingEnabled)
            {
                _logger = new BeautyLog("SyncContext", Color.magenta);
            }
        }
        
        public void SendMessage(SyncMessage message)
        {
            _logger?.Log($"{message.Id}, Action: {message.Action}, Bag: {message.Bag}");
            MessageReceived?.Invoke(message);
        }
    }
}