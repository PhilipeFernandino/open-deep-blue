using PrimeTween;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    [RequireComponent(typeof(Image))]
    public class UIFilledImage : MonoBehaviour
    {
        private Image _image;

        private float _durationInSeconds;
        private float _fillAmountStart;
        private float _fillAmountTarget;

        public event Action Finished;

        public Image Image => _image;

        public void Setup(float fillAmountStart, float fillAmountTarget, float durationInSeconds, bool play = false, Action onFinishedCallback = null)
        {
            _durationInSeconds = durationInSeconds;
            _fillAmountStart = fillAmountStart;
            _fillAmountTarget = fillAmountTarget;
            Finished += onFinishedCallback;

            if (play)
                Play();
        }

        public void Play()
        {
            _image.fillAmount = _fillAmountStart;
            FillTask();

        }
        private async void FillTask()
        {
            await Tween.UIFillAmount(_image, _fillAmountTarget, _durationInSeconds);

            Finished?.Invoke();
        }

        private void Awake()
        {
            _image = GetComponent<Image>();
        }
    }
}