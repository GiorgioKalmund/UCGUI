using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Unity;

namespace UCGUI.Services
{
    public static class ComponentFinder
    {
        // =============================================================== //
        //                 Regular global key-value storage                //
        // =============================================================== //
        private static Dictionary<string, MonoBehaviour> _register = new();

        public static bool Put(string registrationId, MonoBehaviour behaviour)
        {
            if(!_register.TryAdd(registrationId, behaviour))
            {
                UCGUILogger.LogError($"Trying to register an a global refernce for key {registrationId}, " +
                                       $"but there is already an element registered for that key!" +
                                       $"Consider using a different key or calling ComponentFinder.Delete(key) first!");
                return false;
            }
            return true;
        }
        public static bool Delete(string key) 
        {
            return _register.Remove(key);
        }
        
        [CanBeNull]
        public static T Get<T>(string registeredBehaviourId) where T : MonoBehaviour
        {
            if (_register.TryGetValue(registeredBehaviourId, out var value))
            {
                return (T)value;
            }
            return null;
        }

        [CanBeNull]
        public static MonoBehaviour Get(string registeredBehaviourId) => Get<MonoBehaviour>(registeredBehaviourId);
        
        
        // =============================================================== //
        //                 Global Instances                                //
        // =============================================================== //
        private static Dictionary<Type, MonoBehaviour> _instances = new();

        public static bool PutInstance(MonoBehaviour behaviour)
        {
            if (!_instances.TryAdd(behaviour.GetType(), behaviour))
            {
                UCGUILogger.LogError($"Trying to register an Instance of type {behaviour.GetType()}, " +
                                       $"but there is already an Instance registered for that type" +
                                       $"Consider calling ComponentFinder.DeleteInstance<{behaviour.GetType()}>() first!");
                return false;
            }
            return true;
        } 

        public static T CreateInstance<T>() where T : MonoBehaviour
        {
            T instance = UI.N<T>();
            if (!PutInstance(instance))
            {
                UnityEngine.Object.Destroy(instance.gameObject);
            }
            return instance;
        }

        public static bool DeleteInstance<T>() where T : MonoBehaviour
        {
            if (_instances.TryGetValue(typeof(T), out var instance))
            {
                UnityEngine.Object.Destroy(instance);
                _instances.Remove(typeof(T));
                return true;
            }

            return false;
        }

        [CanBeNull]
        public static T GetInstance<T>() where T : MonoBehaviour
        {
            if (_instances.TryGetValue(typeof(T), out var value))
            {
                return (T)value;
            }
            return null;
        }
    }
}