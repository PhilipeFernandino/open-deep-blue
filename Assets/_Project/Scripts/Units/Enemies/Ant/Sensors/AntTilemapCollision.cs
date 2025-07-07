using System;
using UnityEngine;

namespace Core.Units
{
    public class AntTilemapCollision : MonoBehaviour
    {
        public event Action Collided;
        public event Action CollisionStaid;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Tilemap"))
            {
                Collided?.Invoke();
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Tilemap"))
            {
                CollisionStaid?.Invoke();
            }
        }
    }
}