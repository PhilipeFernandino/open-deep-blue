// In AntInputHandler.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class AntInputHandler : MonoBehaviour
{
    [Header("Input Configuration")]
    [SerializeField] private InputActionReference _movementAction;
    [SerializeField] private InputActionReference _interactAction;

    public Vector2 MoveInput { get; private set; }
    public bool InteractTriggered { get; private set; }

    private void OnEnable()
    {
        _movementAction.action.Enable();
        _interactAction.action.Enable();
        _interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        _movementAction.action.Disable();
        _interactAction.action.Disable();
        _interactAction.action.performed -= OnInteract;
    }

    private void Update()
    {
        // Read the movement state every frame. This is fine because it's a continuous state, not a single event.
        MoveInput = _movementAction.action.ReadValue<Vector2>();

        // REMOVED: Do NOT reset the trigger here anymore.
        // InteractTriggered = false; 
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        // When the button is pressed, latch the trigger to true.
        InteractTriggered = true;
    }

    /// <summary>
    /// This is the new public method the agent will call after it has used the input.
    /// </summary>
    public void ConsumeInteractInput()
    {
        InteractTriggered = false;
    }
}