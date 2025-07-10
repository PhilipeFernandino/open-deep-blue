using Core.Events;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Save
{

    public class SaveManager : MonoBehaviour
    {
        [Header("Raise On")]
        [SerializeField] private VoidEventChannelSO _saveRequestChannel;

        [Header("Listen To")]
        [SerializeField] private SaveDataEventChannelSO _sendDataChannel;

        [SerializeField] private GameSessionData _sessionDataHolder;

        private Dictionary<string, SaveData> _collectedData;

        private void OnEnable()
        {
            _sendDataChannel.OnEventRaised += CollectData;
        }

        private void OnDisable()
        {
            _sendDataChannel.OnEventRaised -= CollectData;
        }

        public void InitiateSave()
        {
            _collectedData = new Dictionary<string, SaveData>();
            _saveRequestChannel.RaiseEvent();
            ProcessCollectedData();
        }

        private void CollectData(SaveData data)
        {
            if (data != null)
            {
                _collectedData[data.DataID] = data;
            }
        }

        private void ProcessCollectedData()
        {
            Debug.Log($"Collected {_collectedData.Count} data packets.", this);
            foreach (var entry in _collectedData)
            {
                Debug.Log($"Data ID: {entry.Key}, Type: {entry.Value.GetType().Name}");
            }

            // _sessionDataHolder.gameData = new Dictionary<string, SaveData>(_collectedData);
        }
    }
}
