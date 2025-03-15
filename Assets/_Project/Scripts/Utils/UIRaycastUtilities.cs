using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Util
{
    public static class UIRaycastUtilities
    {
        private static List<RaycastResult> results = new();

        public static bool PointerIsOverUI(Vector2 screenPos)
        {
            if (UIRaycastFirst(screenPos, out GameObject hitObject))
            {
                return hitObject.layer == LayerMask.NameToLayer("UI");
            }
            else
            {
                return false;
            }
        }

        public static List<RaycastResult> UIRaycastAll(Vector2 screenPos)
        {
            var pointerData = ScreenPosToPointerData(screenPos);
            EventSystem.current.RaycastAll(pointerData, results);
            return results;
        }

        public static bool UIRaycastFirst(Vector2 screenPos, out GameObject hitObject)
        {
            var results = UIRaycastAll(screenPos);

            if (results.Count > 0)
            {
                hitObject = results[0].gameObject;
                return true;
            }
            else
            {
                hitObject = null;
                return false;
            }
        }

        public static bool UIRaycastFirstTryGetComponent<T>(Vector2 screenPos, out T component) where T : MonoBehaviour
        {
            if (UIRaycastFirst(screenPos, out GameObject hitObject))
            {
                return hitObject.TryGetComponent(out component);
            }
            else
            {
                component = null;
                return false;
            }
        }

        public static bool UIRaycastAllTryGetComponent<T>(Vector2 screenPos, out T component) where T : MonoBehaviour
        {
            var results = UIRaycastAll(screenPos);

            foreach (var hitObject in results)
            {
                if (hitObject.gameObject.TryGetComponent(out component))
                {
                    return true;
                }
            }

            component = null;
            return false;
        }

        static PointerEventData ScreenPosToPointerData(Vector2 screenPos)
           => new(EventSystem.current) { position = screenPos };
    }
}
