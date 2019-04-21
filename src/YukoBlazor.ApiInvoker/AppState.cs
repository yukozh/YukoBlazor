using System;

namespace YukoBlazor.ApiInvoker
{
    public class AppState
    {
        public event Action OnStateChanged;

        public void TriggerStateChange()
        {
            OnStateChanged?.Invoke();
        }
    }
}
