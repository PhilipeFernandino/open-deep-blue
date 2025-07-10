using Core.Save;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Save
{
    [CreateAssetMenu(menuName = "Core/Events/Save Data Event Channel")]
    public class SaveDataEventChannelSO : ScriptableObject
    {
        public UnityAction<SaveData> OnEventRaised;

        public void RaiseEvent(SaveData data)
        {
            OnEventRaised?.Invoke(data);
        }
    }
}