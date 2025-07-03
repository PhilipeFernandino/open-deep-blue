using UnityEngine;

namespace Extensions
{
	public static class GameObjectExtensions
	{
		public static T GetOrAddComponent<T>(this GameObject gameObject)
			where T : class
		{
			if (gameObject.TryGetComponent(typeof(T), out Component component))
			{
				return component as T;
			}

			return gameObject.AddComponent(typeof(T)) as T;
		}
	}
}