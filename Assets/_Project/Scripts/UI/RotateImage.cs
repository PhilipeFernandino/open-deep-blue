using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class RotateImage : MonoBehaviour
    {
        [SerializeField] private TweenSettings<Vector3> _rotationSettings;
        [SerializeField] private Graphic _graphic;

        private void Awake()
        {
            Tween.EulerAngles(_graphic.transform, _rotationSettings);
        }
    }
}
