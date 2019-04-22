using System;

namespace YukoBlazor.ApiInvoker
{
    public class AppState
    {
        public event Action OnStateChanged;

        public event Action OnCatalogUpdated;

        public void TriggerStateChange()
        {
            OnStateChanged?.Invoke();
        }

        public void TriggerOnCatalogUpdate()
        {
            OnCatalogUpdated?.Invoke();
        }
    }
}
