using Core.Player;
using System.Collections;
using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(Movement2D))]
    public class RotateToMovementDirection : MonoBehaviour
    {
        [SerializeField] private Transform _rotateTransform;

        private Movement2D _movement;

        private void Start()
        {
            _movement = GetComponent<Movement2D>();
        }

        private void FixedUpdate()
        {
            var direction = _movement.FacingDirection;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _rotateTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}