using Cinemachine;
using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
using PrimeTween;
using System;
using UnityEngine;

namespace Core.CameraSystem
{
    public class CameraSystem : Actor, ICameraService
    {
        private CinemachineVirtualCamera _cv;
        private CinemachineFramingTransposer _framingTransposer;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ServiceLocator.Set<ICameraService>(this);

            _cv = FindObjectOfType<CinemachineVirtualCamera>();
            _framingTransposer = _cv.GetCinemachineComponent<CinemachineFramingTransposer>();

            CameraShakedEvent.AddListener(CameraShakedEventHandler);
        }

        private void CameraShakedEventHandler(ref EventContext context, in CameraShakedEvent e)
        {
            ShakeCamera(e.ShakeSettings);
        }

        public void ShakeCamera(ShakeSettings shakeSettings)
        {
            _ = Tween.ShakeCustom(
                _framingTransposer,
                Vector3.zero,
                shakeSettings, (target, val) => target.m_TrackedObjectOffset = val)
                .OnComplete(() => _framingTransposer.m_TrackedObjectOffset = Vector3.zero);
        }
    }

    [DynamicService]
    public interface ICameraService : IService
    {
        public void ShakeCamera(ShakeSettings shakeSettings);
    }
}