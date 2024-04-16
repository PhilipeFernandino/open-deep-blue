using Coimbra.Services;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public static class ServiceLocatorUtilities
    {
        public static T GetServiceAssert<T>()
            where T : class, IService
        {
            var t = ServiceLocator.Get<T>();

            Debug.Assert(t != null, $"{nameof(ServiceLocatorUtilities)} - {nameof(T)} not found");

            return t;
        }
    }
}