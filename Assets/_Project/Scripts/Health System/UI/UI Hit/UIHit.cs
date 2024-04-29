using System;
using TMPro;
using UnityEngine;

namespace Core.HealthSystem.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UIHit : MonoBehaviour
    {
        private TextMeshProUGUI _hitTMP;

        public void Setup(Vector3 position, Color color, float value, Action releaseCallback)
        {
            transform.position = position;
            _hitTMP.alpha = 1f;
            _hitTMP.color = color;
            _hitTMP.text = value.ToString();

            ReleaseTask(releaseCallback);
        }

        private async void ReleaseTask(Action releaseCallback)
        {
            //await UniTask.WhenAll(
            //    _hitTMP
            //        .DOFade(0f, 1f)
            //        .ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, destroyCancellationToken)
            //        .SuppressCancellationThrow(),
            //    transform
            //        .DOMoveY(transform.position.y + 1f, 2f)
            //        .ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, destroyCancellationToken)
            //        .SuppressCancellationThrow());

            if (!destroyCancellationToken.IsCancellationRequested)
            {
                releaseCallback();
            }
        }

        private void Awake()
        {
            _hitTMP = GetComponent<TextMeshProUGUI>();
        }
    }
}