using Coimbra.Services;
using UnityEngine;

namespace Core.Util
{
    public static class ServiceLocatorUtilities
    {
        public static T GetServiceAssert<T>()
            where T : class, IService
        {
            var t = ServiceLocator.Get<T>();

            Debug.Assert(t != null, $"{nameof(ServiceLocatorUtilities)} - {typeof(T)} not found");

            return t;
        }
    }
}