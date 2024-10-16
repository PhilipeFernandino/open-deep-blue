using UnityEngine;

namespace Core.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIDynamicCanvas : MonoBehaviour
    {
        [SerializeField] private bool _hideSelfOnAwake = true;

        private Canvas _canvas;

        public Canvas Canvas => _canvas == null ? _canvas = GetComponent<Canvas>() : _canvas;
        public bool IsEnabled => _canvas.enabled;

        public virtual void ShowSelf()
        {
            SetActiveState(true);
        }

        public virtual void HideSelf()
        {
            SetActiveState(false);
        }

        public virtual void ToggleSelf()
        {
            SetActiveState(Canvas.enabled);
        }

        public virtual void SetActiveState(bool state)
        {
            Canvas.enabled = state;
        }

        protected virtual void Awake()
        {
            if (_hideSelfOnAwake)
            {
                HideSelf();
            }
        }
    }
}
