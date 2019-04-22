using System;

namespace YukoBlazor.ApiInvoker
{
    public class AppState
    {
        public event Action OnStateChanged;

        public event Action OnCatalogUpdated;

        public event Action OnTagsUpdated;

        public void TriggerStateChange()
        {
            OnStateChanged?.Invoke();
        }

        public void TriggerCatalogUpdate()
        {
            OnCatalogUpdated?.Invoke();
        }

        public void TriggerTagsUpdate()
        {
            OnTagsUpdated?.Invoke();
        }
    }
}
