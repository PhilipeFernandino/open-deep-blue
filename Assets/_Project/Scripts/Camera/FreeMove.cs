using UnityEngine;
using UnityEngine.InputSystem;

public class FreeMove : MonoBehaviour
{
    [SerializeField] private float _speed;

    public void MoveInput(InputAction.CallbackContext context)
    {
        TryToMove(context.ReadValue<Vector2>());
    }

    private void TryToMove(Vector2 direction)
    {
        var position = transform.position;
        position += (Vector3)direction * _speed * Time.deltaTime;
        transform.position = position;
    }
}
