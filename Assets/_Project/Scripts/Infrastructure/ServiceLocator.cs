using System;
using System.Collections.Generic;
using UnityEngine;

namespace OctanGames.Infrastructure
{
    public class ServiceLocator : MonoBehaviour
    {
        private const string SERVICE_LOCATOR_GAMEOBJECT = "ServiceLocator";

        private static ServiceLocator _instance;

        private Dictionary<Type, object> _registrations;
        private Dictionary<Type, Func<object>> _lazyRegistrations;

        private static ServiceLocator Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = new GameObject(SERVICE_LOCATOR_GAMEOBJECT).AddComponent<ServiceLocator>();
                DontDestroyOnLoad(_instance.gameObject);

                return _instance;
            }
        }

        public static void Bind<T>(T obj)
        {
            Instance.BindInternal(obj);
        }

        public static void BindLazy<T>(Func<T> func)
        {
            Instance.BindLazyInternal(func);
        }

        public static T GetInstance<T>()
        {
            return Instance.GetInstanceInternal<T>();
        }

        private void BindInternal<T>(T obj)
        {
            Type type = typeof(T);
            if (ContainsType(type))
            {
#if DEV_LOG
                Debug.LogWarning("Re-registration of type " + type);
#endif
            }

            _registrations ??= new Dictionary<Type, object>();
            _registrations.Add(type, obj);
        }

        private void BindLazyInternal<T>(Func<T> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            Type type = typeof(T);
            if (ContainsType(type))
            {
#if DEV_LOG
                Debug.LogWarning("Re-registration of type " + type);
#endif
            }

            _lazyRegistrations ??= new Dictionary<Type, Func<object>>();
            _lazyRegistrations.Add(type, () => func());
        }

        private T GetInstanceInternal<T>()
        {
            Type type = typeof(T);
            if (_lazyRegistrations != null && _lazyRegistrations.ContainsKey(type))
            {
                var result = (T) _lazyRegistrations[type]();
                BindInternal(result);
                _lazyRegistrations.Remove(type);
                return result;
            }

            if (_registrations != null && _registrations.ContainsKey(type))
            {
                return (T) _registrations[type];
            }

            throw new ContainerException("Container doesn't contain type " + type);
        }

        private bool ContainsType(Type type)
        {
            return _registrations != null && _registrations.ContainsKey(type) ||
                   _lazyRegistrations != null && _lazyRegistrations.ContainsKey(type);
        }
    }
}