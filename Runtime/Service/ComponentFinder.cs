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
        private static Dictionary<string, BaseComponent> _register = new();

        public static bool Put(string registrationId, BaseComponent baseComponent)
        {
            if(!_register.TryAdd(registrationId, baseComponent))
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
        public static T Get<T>(string registeredComponentId) where T : BaseComponent
        {
            if (_register.TryGetValue(registeredComponentId, out var value))
            {
                return (T)value;
            }
            return null;
        }

        [CanBeNull]
        public static BaseComponent Get(string registeredComponentId) => Get<BaseComponent>(registeredComponentId);
        
        
        // =============================================================== //
        //                 Global Instances                                //
        // =============================================================== //
        private static Dictionary<Type, BaseComponent> _instances = new();

        public static bool PutInstance(BaseComponent baseComponent)
        {
            if (!_instances.TryAdd(baseComponent.GetType(), baseComponent))
            {
                UCGUILogger.LogError($"Trying to register an Instance of type {baseComponent.GetType()}, " +
                                       $"but there is already an Instance registered for that type" +
                                       $"Consider calling ComponentFinder.DeleteInstance<{baseComponent.GetType()}>() first!");
                return false;
            }
            return true;
        } 

        public static T CreateInstance<T>() where T : BaseComponent
        {
            T instance = UI.N<T>();
            if (!PutInstance(instance))
            {
                UnityEngine.Object.Destroy(instance.gameObject);
            }
            return instance;
        }

        public static bool DeleteInstance<T>() where T : BaseComponent
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
        public static T GetInstance<T>() where T : BaseComponent
        {
            if (_instances.TryGetValue(typeof(T), out var value))
            {
                return (T)value;
            }
            return null;
        }
    }
}