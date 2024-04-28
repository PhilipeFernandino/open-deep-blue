using UnityEngine;

namespace Player
{
    public class PlayerMovement2D : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb2D;
        [SerializeField] private float _speed;

        private Vector2 _movementInput;

        public void TryToMove(Vector2 direction)
        {
            _movementInput = direction;
        }

        private void FixedUpdate()
        {
            if (_movementInput.magnitude > 1)
            {
                _movementInput.Normalize();
            }

            var fs = _speed * Time.deltaTime;
            var motion = (_movementInput.y * transform.up + _movementInput.x * transform.right) * fs;
            _rb2D.MovePosition(_rb2D.position + motion.XY());
        }
    }
}