using UnityEngine;
using UnityEngine.InputSystem;

public class AntInputHandler : MonoBehaviour
{
    [Header("Input Configuration")]
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private InputActionReference interactAction;

    public Vector2 MoveInput { get; private set; }
    public bool InteractTriggered { get; private set; }

    private void OnEnable()
    {
        movementAction.action.Enable();
        interactAction.action.Enable();

        interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        movementAction.action.Disable();
        interactAction.action.Disable();

        interactAction.action.performed -= OnInteract;
    }

    private void Update()
    {
        MoveInput = movementAction.action.ReadValue<Vector2>();

        InteractTriggered = false;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        InteractTriggered = true;
    }
}