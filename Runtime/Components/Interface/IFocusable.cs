using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace UCGUI
{
    public partial interface IFocusable
    {
        public static Dictionary<string, IFocusable> FocusGroups = new Dictionary<string, IFocusable>();
        public static string[] FocusGroupNames => FocusGroups.Keys.ToArray();

        private static IFocusable _lastFocusedElement;

        /// <summary>
        /// The true current focus group. If a member overrides <see cref="FocusGroup"/> to not be `null`, the respective value will be
        /// used. Otherwise, UCGUI will use <see cref="Defaults.Focus.DefaultGroup"/> for focus behaviour.
        /// </summary>
        public string CurrentGroup => FocusGroup ?? Defaults.Focus.DefaultGroup;
        
        /// <summary>
        /// The focus group to be a part of.
        /// </summary>
        /// <remarks>
        /// If left unspecified, UCGUI will use <see cref="Defaults.Focus.DefaultGroup"/> for focus behaviour.
        /// </remarks>
        [CanBeNull] public string FocusGroup { get; set; }
        [CanBeNull] public UnityEvent OnFocusEvent { get; set; }
        [CanBeNull] public UnityEvent OnUnfocusEvent { get; set; }

        public void HandleFocus();
        public void HandleUnfocus();

        /// IFocusable general functions
        public static void SetLastFocusedElement(IFocusable focusable)
        {
            _lastFocusedElement = focusable;
        }

        public static void FocusLastFocused()
        {
            _lastFocusedElement.Focus();
        }

        public static void UnfocusGroupId(string focusGroup)
        {
            if (FocusGroups.TryGetValue(focusGroup, out var previousFocusable))
            {
                previousFocusable.Unfocus();
            }
        }

        public static void FocusGroupId(string focusGroup)
        {
            if (FocusGroups.TryGetValue(focusGroup, out var previousFocusable))
            {
                previousFocusable.Focus();
            }
        }

        [CanBeNull]
        public static IFocusable GetFocusedElement(string focusGroup)
        {
            return FocusGroups.GetValueOrDefault(focusGroup, null);
        }

        [Serializable]
        public class FocusState<T> where T : struct, Enum
        {
            private readonly Dictionary<T, IFocusable> focusMap = new Dictionary<T, IFocusable>();
            
            private T? _value;

            public string FocusHash { get; } = Guid.NewGuid().ToString();

            public FocusState() : this(null) { }

            public FocusState(T? defaultValue)
            {
                Value = defaultValue;
            }

            public T? Value
            {
                get => _value;
                set
                {
                    if (value.Equals(_value))
                        return;
                    
                    _value = value;
                    if (_value.HasValue)
                    {
                        focusMap.TryGetValue(_value.Value, out var focusable);
                        if (!focusable.IsFocused())
                            focusable.Focus();
                    }
                    else
                    {
                        UnfocusGroupId(FocusHash);
                    }
                }
            }

            public void Add(T key, IFocusable focusable)
            {
                focusable.FocusGroup = FocusHash;
                if (!focusMap.TryAdd(key, focusable))
                {
                    UCGUILogger.LogError($"FocusState.Add(): Cannot add element with key `{key}` because there is already an entry for this key (`{focusMap[key]}`). Make sure key associations are unique!");
                    return;
                };
                
                focusable.CreateFocusEvent();
                focusable.OnFocusEvent?.AddListener(() => Value = key );
                focusable.CreateUnfocusEvent();
                focusable.OnUnfocusEvent?.AddListener(() =>
                {
                    /*
                     * Only set the value to null if the group also looses its focus. If not null, this means there has been a direct
                     * transition from one state another. Unfocusing will therefore already be handled by the newly focused element and
                     * the state can transition directly.
                     */
                    if (GetFocusedElement(focusable.CurrentGroup) == null)
                        Value = null;
                });
            }

            /// <summary>
            /// Attempts on focusing the next element in the state cycle
            /// or resets the focus if the at the end of the cycle and <see cref="nullCycle"/> is true.
            /// </summary>
            /// <param name="nullCycle">Whether to additionally cycle to null after the last element, resetting the focus state.</param>
            public void Next(bool nullCycle = true)
            {
                if (focusMap.Count == 0)
                {
                    UCGUILogger.LogWarning("FocusState.Next(): Focus Map is empty. Cannot focus elements if FocusState is empty! Please add elements using `.Focusable(FocusState, Enum)` on IFocusable elements.");
                    return;
                }
                var values = focusMap.Keys.ToList();
                var current = Value;
                
                if (!current.HasValue)
                {
                    focusMap[values[0]].Focus();
                }
                else
                {
                    int nextIndex = (values.IndexOf(current.Value) + 1);
                    if (nextIndex == values.Count)
                    {
                        if (nullCycle)
                        {
                            focusMap[values[^1]].Unfocus();
                            return;
                        }
                        nextIndex = 0;
                    }
                    focusMap[values[nextIndex]].Focus();
                }
            }
            
            public void Previous(bool nullCycle = true)
            {
                if (focusMap.Count == 0)
                {
                    UCGUILogger.LogWarning("FocusState.Next(): Focus Map is empty. Cannot focus elements if FocusState is empty! Please add elements using `.Focusable(FocusState, Enum)` on IFocusable elements.");
                    return;
                }
                var values = focusMap.Keys.ToList();
                var current = Value;
                
                if (!current.HasValue)
                {
                    focusMap[values[^1]].Focus();
                }
                else
                {
                    int nextIndex = (values.IndexOf(current.Value) - 1);
                    if (nextIndex < 0)
                    {
                        if (nullCycle)
                        {
                            focusMap[values[0]].Unfocus();
                            return;
                        }
                        nextIndex = values.Count - 1;
                    }
                    focusMap[values[nextIndex]].Focus();
                }
            }
        }
    }

    public static class FocusableHelper
    {
        
        /// <summary>
        /// Focuses the element within the <see cref="focusable"/>'s <see cref="IFocusable.SetFocusGroup"/>.
        /// </summary>
        /// <param name="focusable">Element to focus.</param>
        /// <typeparam name="T">Any <see cref="IFocusable"/>.</typeparam>
        public static void Focus<T>(this T focusable) where T : IFocusable
        {
            if (focusable == null)
            {
                UCGUILogger.LogError("Cannot focus null!");
                return;
            }
            

            IFocusable lastFocused = IFocusable.GetFocusedElement(focusable.CurrentGroup);
            IFocusable.FocusGroups[focusable.CurrentGroup] = focusable;
            if (lastFocused != null && !lastFocused.Equals(focusable))
                lastFocused.Unfocus();
            focusable.HandleFocus();
            focusable.OnFocusEvent?.Invoke();
        }

        
        /// <summary>
        /// Unfocuses the element within the given group. <br></br>
        /// Sets <see cref="IFocusable._lastFocusedElement"/> to this element. Can then be re-focused using <see cref="IFocusable.FocusLastFocused"/>.
        /// </summary>
        /// <param name="focusable">Element to unfocus.</param>
        /// <typeparam name="T">Any <see cref="IFocusable"/>.</typeparam>
        public static void Unfocus<T>(this T focusable) where T : IFocusable
        {
            IFocusable.SetLastFocusedElement(focusable);
            if (focusable == null)
                return;
            if (focusable.IsFocused())
                IFocusable.FocusGroups[focusable.CurrentGroup] = null;
            focusable.HandleUnfocus();
            focusable.OnUnfocusEvent?.Invoke();
        }

        /// <summary>
        /// Returns whether the element is currently focused within its base focus group. See <see cref="IFocusable.SetFocusGroup"/>.
        /// </summary>
        /// <param name="focusable">The element to check</param>
        /// <typeparam name="T">Any <see cref="IFocusable"/>.</typeparam>
        /// <returns>
        /// `true` if the element is focused, else `false`.
        /// </returns>
        public static bool IsFocused<T>(this T focusable) where T : IFocusable
        {
            return IFocusable.FocusGroups.ContainsKey(focusable.CurrentGroup) 
                   && IFocusable.FocusGroups[focusable.CurrentGroup] is T f 
                   && f.Equals(focusable);
        }
        
        /// <summary>
        /// Toggles an element's focus.
        /// </summary>
        /// <param name="focusable">The element to check</param>
        /// <typeparam name="T">Any <see cref="IFocusable"/>.</typeparam>
        public static void ToggleFocus<T>(this T focusable) where T : IFocusable
        {
            if (focusable.IsFocused())
                focusable.Unfocus();
            else
                focusable.Focus();
        }
        
        /// <summary>
        /// Links the element to a <see cref="IFocusable.FocusState{T}"/>.
        /// </summary>
        /// <param name="focusable"></param>
        /// <param name="state">The <see cref="IFocusable.FocusState{T}"/> managing focus elements.</param>
        /// <param name="value"><see cref="V"/> enum value. Has to be unique to this object within the <see cref="state"/>.</param>
        /// <typeparam name="T">Any <see cref="IFocusable"/>.</typeparam>
        /// <typeparam name="V">Enum value linked to this specific element to focus.</typeparam>
        /// <returns>
        /// The <see cref="focusable"/> itself.
        /// </returns>
        /// <remarks>
        /// <b>DO NOT USE <see cref="Focus{T}(T)"/> FOR ELEMENTS WHICH ARE PART OF A FOCUS STATE.
        /// This will lead to unwanted behaviour as the elements base foucs group will be used!</b>
        /// </remarks>
        public static T Focusable<T, V>(this T focusable, IFocusable.FocusState<V> state, V value) where V : struct, Enum where T : IFocusable
        {
            state.Add(value, focusable);
            return focusable;
        }


        public static void CreateFocusEvent<T>(this T focusable) where T : IFocusable
        {
            focusable.OnFocusEvent ??= new UnityEvent();
        }
        public static void CreateUnfocusEvent<T>(this T focusable) where T : IFocusable
        {
            focusable.OnUnfocusEvent ??= new UnityEvent();
        }
        
        public static void DrawFocusableDebug<T>(this T forElement) where T : BaseComponent, IFocusable
        {
            if (forElement.debugOptions.HasFlag(DebugOptions.TextOnly))
            {
                RectTransform rect = forElement.gameObject.GetComponent<RectTransform>();
                var currentGroup = forElement.CurrentGroup;
                var leader = IFocusable.GetFocusedElement(currentGroup);
                var focusMessage = $"<u>Focus</u>\nGroup:{currentGroup}\nFocused:{forElement.IsFocused()}";
                string leaderName = leader is MonoBehaviour l ? l.gameObject.name : "null";
                focusMessage += $"\nLeader:{leaderName}";
                Handles.Label(forElement.transform.position + new Vector3(-rect.sizeDelta.x / 2, -12 + rect.sizeDelta.y / 2, 0),  focusMessage, Defaults.Debug.DebugRed());
            }
        }
    }
}