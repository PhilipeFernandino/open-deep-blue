using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Core.Player
{
    public class Movement2D : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb2D;

        [SerializeField] private float _timeKnRate = 0.02f;
        [SerializeField] float _forceKnRate = 0.1f;

        public Vector2 Position => _rb2D.position;
        public Vector2 LastMovementInput { get; private set; }
        public Vector2 FacingDirection { get; private set; }


        private float _speed;

        // Let's keep this here so the rigidbody doesn't get touched by multiple components at the same time 
        // or to not need to control who is controlling the rb at any given moment
        private bool _isDashing = false;
        private bool _isKnockbackApplied = false;
        private float _dashDuration;
        private Vector2 _dashVelocity;

        private Vector2 _movementInput;


        private CancellationTokenSource _knockbackCts;

        public void Setup(float speed)
        {
            _speed = speed;
        }

        public void Teleport(Vector2 position)
        {
            _rb2D.position = position;
        }

        public void TryToMove(Vector2 direction)
        {
            _movementInput = direction;
            LastMovementInput = _movementInput.NormalizeExcess();

            if (direction != Vector2.zero)
            {
                FacingDirection = direction;
            }
        }

        public void Dash(Vector2 velocity, float duration, Action callback = null)
        {
            Debug.Log($"{GetType()} - Start dash V: {velocity}, D: {duration}");
            _isDashing = true;
            _dashVelocity = velocity;
            _dashDuration = duration;

            EndDashTask(callback);
        }

        public async void AddKnockback(Vector2 force)
        {
            Debug.Log($"{GetType()} - {this.name}: Added knockback: {force}");

            _rb2D.linearVelocity = force * _forceKnRate;
            _isKnockbackApplied = true;
            float magnitude = force.magnitude;


            _knockbackCts?.Cancel();
            _knockbackCts?.Dispose();
            _knockbackCts = new();

            var token = _knockbackCts.Token;

            bool wasCancelled = await UniTask.Delay(
                TimeSpan.FromSeconds(magnitude * _timeKnRate),
                cancellationToken: token)
                .SuppressCancellationThrow();

            if (!wasCancelled)
            {
                _isKnockbackApplied = false;
                _rb2D.linearVelocity = Vector2.zero;
            }
        }

        private async void EndDashTask(Action callback)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_dashDuration));
            _isDashing = false;
            callback?.Invoke();
        }

        public void ResetMovement()
        {
            _movementInput = Vector2.zero;
        }

        private void FixedUpdate()
        {
            if (_isKnockbackApplied)
            {
                return;
            }

            if (_isDashing)
            {
                var motion = (_dashVelocity.y * transform.up + _dashVelocity.x * transform.right) * Time.fixedDeltaTime;
                _rb2D.MovePosition(_rb2D.position + motion.XY());
            }
            else
            {
                _movementInput = _movementInput.NormalizeExcess();

                var movement = _speed * Time.fixedDeltaTime;
                var motion = (_movementInput.y * transform.up + _movementInput.x * transform.right) * movement;
                _rb2D.MovePosition(_rb2D.position + motion.XY());
            }
        }
    }
}