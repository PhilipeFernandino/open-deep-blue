using UnityEngine;

public class SimplePlayer : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private Vector2 _movement;

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        Walk();
    }

    private void GetInput()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");
    }

    private void Walk()
    {
        gameObject.transform.Translate(
            _movement.x * _moveSpeed * Time.deltaTime,
            _movement.y * _moveSpeed * Time.deltaTime,
            0
        );
    }
}
