using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Level
{
    public class AntInputHandler : MonoBehaviour
    {
        [Header("Input Actions")]
        [SerializeField] private InputActionReference _movementAction;
        [SerializeField] private InputActionReference _genericInteractAction;
        [SerializeField] private InputActionReference _eatAction;

        public Vector2 MoveInput { get; private set; }
        public bool InteractTriggered { get; private set; }
        public bool EatTriggered { get; private set; }

        private void OnEnable()
        {
            _movementAction.action.Enable();
            _genericInteractAction.action.Enable();
            _eatAction.action.Enable();

            _genericInteractAction.action.performed += OnInteract;
            _eatAction.action.performed += OnEat;
        }

        private void OnDisable()
        {
            _movementAction.action.Disable();
            _genericInteractAction.action.Disable();
            _eatAction.action.Disable();

            _genericInteractAction.action.performed -= OnInteract;
            _eatAction.action.performed -= OnEat;
        }

        private void Update()
        {
            MoveInput = _movementAction.action.ReadValue<Vector2>();
        }

        private void OnInteract(InputAction.CallbackContext context) => InteractTriggered = true;
        private void OnEat(InputAction.CallbackContext context) => EatTriggered = true;

        public void ConsumeInteractInput() => InteractTriggered = false;
        public void ConsumeEatInput() => EatTriggered = false;
    }
}
