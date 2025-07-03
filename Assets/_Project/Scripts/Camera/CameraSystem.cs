using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
using PrimeTween;
using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Core.CameraSystem
{
    public class CameraSystem : Actor, ICameraService
    {
        private CinemachineVirtualCameraBase _cv;
        private CinemachinePositionComposer _positionComposer;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ServiceLocator.Set<ICameraService>(this);

            _cv = GetComponentInChildren<CinemachineVirtualCameraBase>();
            //_positionComposer = _cv.GetCinemachineComponent<CinemachinePositionComposer>();

            CameraShakedEvent.AddListener(CameraShakedEventHandler);
        }

        private void CameraShakedEventHandler(ref EventContext context, in CameraShakedEvent e)
        {
            ShakeCamera(e.ShakeSettings);
        }

        public void ShakeCamera(ShakeSettings shakeSettings)
        {
            //_ = Tween.ShakeCustom(
            //    _framingTransposer,
            //    Vector3.zero,
            //    shakeSettings, (target, val) => target.m_TrackedObjectOffset = val)
            //    .OnComplete(() => _framingTransposer.m_TrackedObjectOffset = Vector3.zero);
        }
    }

    [DynamicService]
    public interface ICameraService : IService
    {
        public void ShakeCamera(ShakeSettings shakeSettings);
    }
}