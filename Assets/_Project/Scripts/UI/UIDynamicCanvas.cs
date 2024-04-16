using UnityEngine;

namespace Core.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIDynamicCanvas : MonoBehaviour
    {
        [SerializeField] private bool _hideSelfOnAwake = true;

        private Canvas _canvas;

        public Canvas Canvas => _canvas;
        public bool IsEnabled => _canvas.enabled;

        public virtual void ShowSelf()
        {
            _canvas.enabled = true;
        }

        public virtual void HideSelf()
        {
            _canvas.enabled = false;
        }

        public virtual void ToggleSelf()
        {
            _canvas.enabled = !_canvas.enabled;
        }

        public virtual void SetActiveState(bool state)
        {
            _canvas.enabled = state;
        }

        protected virtual void Awake()
        {
            _canvas = GetComponent<Canvas>();

            if (_hideSelfOnAwake)
            {
                HideSelf();
            }
        }
    }
}
