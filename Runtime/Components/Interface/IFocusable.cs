namespace UCGUI
{
    public interface IFocusable
    {

        public static IFocusable[] FocusGroups = new IFocusable[10];

        private static IFocusable _lastFocusedElement;

        public void HandleFocus();
        public void HandleUnfocus();
        public int GetFocusGroup()
        {
            return 0;
        }
        public static void SetLastFocusedElement(IFocusable focusable)
        {
            _lastFocusedElement = focusable;
        }

        public static void FocusLastFocused()
        {
            _lastFocusedElement.Focus();
        }

        public static void UnFocusGroup(int focusGroup)
        {
            FocusGroups[focusGroup]?.UnFocus();
        }

        public static void FocusGroup(int focusGroup)
        {
            FocusGroups[focusGroup]?.Focus();
        }

        public static IFocusable GetFocusedElement(int focusGroup)
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
            return IFocusable.FocusGroups[focusable.GetFocusGroup()] is T f && f.Equals(focusable);
        }
    }
}