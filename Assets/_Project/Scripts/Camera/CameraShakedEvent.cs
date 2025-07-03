using Coimbra.Services.Events;
using PrimeTween;

namespace Core.CameraSystem
{
    public readonly partial struct CameraShakedEvent : IEvent
    {
        public readonly ShakeSettings ShakeSettings;

        public CameraShakedEvent(ShakeSettings shakeSettings)
        {
            ShakeSettings = shakeSettings;
        }
    }
}