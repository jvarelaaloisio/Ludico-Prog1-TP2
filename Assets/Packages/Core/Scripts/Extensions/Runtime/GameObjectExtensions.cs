using UnityEngine;

namespace VarelaAloisio.Core.Extensions
{
	public static class GameObjectExtensions
	{
		public static bool TryGetComponentInParent<T>(this GameObject current, out T component) where T: Component
		{
			component = current.GetComponentInParent<T>();
			return current;
		}
	}
}
