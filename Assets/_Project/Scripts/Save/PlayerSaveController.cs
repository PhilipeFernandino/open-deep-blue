using Core.Events;
using UnityEngine;

namespace Core.Save
{
    public class PlayerSaveController : MonoBehaviour
    {
        [Header("Listen To")]
        [SerializeField] private VoidEventChannelSO _saveRequestChannel;

        [Header("Raise On")]
        [SerializeField] private SaveDataEventChannelSO _sendPlayerDataChannel;

        private void OnEnable()
        {
            _saveRequestChannel.OnEventRaised += PrepareAndSendData;
        }

        private void OnDisable()
        {
            _saveRequestChannel.OnEventRaised -= PrepareAndSendData;
        }

        private void PrepareAndSendData()
        {
            var data = new PlayerSaveData
            {
                DataID = "PlayerData",
            };

            _sendPlayerDataChannel.RaiseEvent(data);
        }
    }
}
