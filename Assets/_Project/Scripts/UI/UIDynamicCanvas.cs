using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIDynamicCanvas : MonoBehaviour
    {
        [SerializeField] private bool _hideSelfOnAwake = true;
        [SerializeField] private bool _controlGraphicRaycaster = true;

        private Canvas _canvas;
        private GraphicRaycaster _graphicRaycaster;

        public Canvas Canvas => _canvas == null ? _canvas = GetComponent<Canvas>() : _canvas;
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster == null ? _graphicRaycaster = GetComponent<GraphicRaycaster>() : _graphicRaycaster;

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
            SetActiveState(!IsEnabled);
        }

        public virtual void SetActiveState(bool state)
        {
            Canvas.enabled = state;

            if (_controlGraphicRaycaster)
            {
                GraphicRaycaster.enabled = state;

                // Unity behaviour. Won't set graphic raycaster correctly without resetting the obj
                gameObject.SetActive(false);
                gameObject.SetActive(true);
            }
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
