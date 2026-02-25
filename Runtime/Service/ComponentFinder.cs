using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UCGUI.Services
{
    public static class ComponentFinder
    {
        // =============================================================== //
        //                 Regular global key-value storage                //
        // =============================================================== //
        private static Dictionary<string, MonoBehaviour> _register = new();

        /// <summary>
        /// Stores a refernce to the MonoBehaviour in a global hashmap.
        /// </summary>
        /// <param name="registrationId">The unique id to store the behaviour.</param>
        /// <param name="behaviour">The behaviour to store.</param>
        /// <returns>True if the operation could be completed, else false.</returns>
        public static bool Put(string registrationId, MonoBehaviour behaviour)
        {
            if(!_register.TryAdd(registrationId, behaviour))
            {
                UCGUILogger.LogError($"Trying to register an a global reference for key {registrationId}, " +
                                       $"but there is already an element registered for that key!" +
                                       $"Consider using a different key or calling ComponentFinder.Delete(key) first!");
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Removes a given key from the global hashmap.
        /// </summary>
        /// <param name="key">The key of which to remove the kv-pair.</param>
        /// <returns>True if the operation could be completed, else false.</returns>
        public static bool Delete(string key) 
        {
            return _register.Remove(key);
        }
        
        /// <summary>
        /// Returns the value for a given key in the global hashmap.
        /// </summary>
        /// <remarks>Be aware of Unity's lifetimes! A returned behaviour might become null if the attached GameObject was deleted.</remarks>
        /// <param name="registeredBehaviourId">The key / id under which the behaviour is stored.</param>
        /// <typeparam name="T">Any MonoBehaviour</typeparam>
        /// <returns>The MonoBehaviour for the given key, cast as <see cref="T"/>, if the operation could be completed, else null.</returns>
        [CanBeNull]
        public static T Get<T>(string registeredBehaviourId) where T : MonoBehaviour
        {
            if (_register.TryGetValue(registeredBehaviourId, out var value))
            {
                return (T)value;
            }
            return null;
        }

        /// <summary>
        /// Returns the value for a given key in the global hashmap.
        /// </summary>
        /// <remarks>Be aware of Unity's lifetimes! A returned behaviour might become null if the attached GameObject was deleted.</remarks>
        /// <param name="registeredBehaviourId">The key / id under which the behaviour is stored.</param>
        /// <returns>The MonoBehaviour for the given key if the operation could be completed, else null.</returns>
        [CanBeNull]
        public static MonoBehaviour Get(string registeredBehaviourId) => Get<MonoBehaviour>(registeredBehaviourId);
        
        
        // =============================================================== //
        //                        Global Instances                         //
        // =============================================================== //
        private static Dictionary<Type, MonoBehaviour> _instances = new();

        /// <summary>
        /// Creates a Singleton-like Instance of for this class, storing it in a static map.
        /// </summary>
        /// <param name="behaviour">The <see cref="MonoBehaviour"/> to make an Instance.</param>
        /// <param name="replaceOld">Whether to delete and replace the previous Instance (if present) before creating a new one. Defaults to true.</param>
        /// <returns>Whether the instance creation was successful.</returns>
        /// <remarks><b>Beware: Try combining it with <see cref="DeleteInstance"/> during "OnDestroy" to avoid unwanted behaviour or calling it with <see cref="replaceOld"/> set to true.</b></remarks>
        public static bool PutInstance<T>(T behaviour, bool replaceOld = true) where T : MonoBehaviour
        {
            if (replaceOld)
                DeleteInstance<T>();
            
            if (!_instances.TryAdd(behaviour.GetType(), behaviour))
            {
                UCGUILogger.LogError($"Trying to register an Instance of type {behaviour.GetType()}, " +
                                       $"but there is already an Instance registered for that type. " +
                                       $"Consider calling ComponentFinder.DeleteInstance<{behaviour.GetType()}>() first or calling" +
                                       $"ComponentFinder.PutInstance(yourInstance, true)!");
                return false;
            }
            return true;
        } 

        /// <summary>
        /// Creates a new Singleton-like Instance for this class.
        /// </summary>
        /// <typeparam name="T">The <see cref="MonoBehaviour"/> type to create an Instance for.</typeparam>
        /// <returns>The newly created instance.</returns>
        public static T CreateInstance<T>(bool replaceOld = false) where T : MonoBehaviour
        {
            T instance = Object.Instantiate(new GameObject("Instance - " + typeof(T).Name)).AddComponent<T>();
            if (!PutInstance(instance, replaceOld))
            {
                Object.Destroy(instance.gameObject);
            }
            return instance;
        }

        /// <summary>
        /// Deletes an Instance with its corresponding GameObject.
        /// </summary>
        /// <typeparam name="T">The <see cref="MonoBehaviour"/> type to delete the Instance of.</typeparam>
        /// <returns>Whether the deletion was successful, aka. if there was an Instance to destroy in the first place.</returns>
        public static bool DeleteInstance<T>() where T : MonoBehaviour
        {
            if (_instances.TryGetValue(typeof(T), out var instance))
            {
                Object.Destroy(instance);
                _instances.Remove(typeof(T));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Finds and Instance for the given type.
        /// </summary>
        /// <typeparam name="T">The <see cref="MonoBehaviour"/> type to grab the Instance of.</typeparam>
        /// <returns>The Instance of that type if one was created, else null.</returns>
        [CanBeNull]
        public static T GetInstance<T>() where T : MonoBehaviour
        {
            if (_instances.TryGetValue(typeof(T), out var value))
            {
                return (T)value;
            }
            UCGUILogger.LogWarning($"ComponentFinder: Attempted to get and Instance which was not registered yet! " +
                                       $"Try calling ComponentFinder.CreateInstance<{typeof(T).Name}> or using ComponentFinder.PutInstance()!");
            return null;
        }
    }
}