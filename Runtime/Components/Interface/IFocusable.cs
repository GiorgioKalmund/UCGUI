using System.Collections.Generic;
using System.Linq;

namespace UCGUI
{
    public partial interface IFocusable
    {

        public static Dictionary<string, IFocusable> FocusGroups = new Dictionary<string, IFocusable>();
        public static string[] FocusGroupNames => FocusGroups.Keys.ToArray();

        private static IFocusable _lastFocusedElement;

        public void HandleFocus();
        public void HandleUnfocus();
        public virtual string GetFocusGroup()
        {
            return Defaults.Focus.DefaultGroup;
        }
        public static void SetLastFocusedElement(IFocusable focusable)
        {
            _lastFocusedElement = focusable;
        }

        public static void FocusLastFocused()
        {
            _lastFocusedElement.Focus();
        }

        public static void UnFocusGroup(string focusGroup)
        {
            FocusGroups[focusGroup]?.UnFocus();
        }

        public static void FocusGroup(string focusGroup)
        {
            FocusGroups[focusGroup]?.Focus();
        }

        public static IFocusable GetFocusedElement(string focusGroup)
        {
            return FocusGroups[focusGroup];
        }
    }

    public static class FocusableHelper
    {
        public static void Focus<T>(this T focusable) where T : IFocusable
        {
            if (focusable == null)
                return;
            IFocusable.FocusGroups[focusable.GetFocusGroup()]?.UnFocus();
            IFocusable.FocusGroups[focusable.GetFocusGroup()] = focusable;
            focusable.HandleFocus();
        }
        public static void UnFocus<T>(this T focusable) where T : IFocusable
        {
            IFocusable.SetLastFocusedElement(focusable);
            if (focusable == null)
                return;
            IFocusable.FocusGroups[focusable.GetFocusGroup()]= null;
            focusable.HandleUnfocus();
        }

        public static bool IsFocused<T>(this T focusable) where T : IFocusable
        {
            return IFocusable.FocusGroups.ContainsKey(focusable.GetFocusGroup()) 
                   && IFocusable.FocusGroups[focusable.GetFocusGroup()] is T f 
                   && f.Equals(focusable);
        }
        
        public static void ToggleFocus<T>(this T focusable) where T : IFocusable
        {
            if (focusable.IsFocused())
                focusable.UnFocus();
            else
                focusable.Focus();
        }
    }
}