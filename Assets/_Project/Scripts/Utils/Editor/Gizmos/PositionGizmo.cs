using UnityEditor;
using UnityEngine;

namespace Core.Util
{
    public class PositionGizmo : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "this");
            Handles.Label(transform.position, $"({transform.position.x:F}, {transform.position.y:F})");
        }
    }
}
