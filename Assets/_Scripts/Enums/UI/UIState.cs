#region using

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Assets._Scripts.Enums.UI
{
    public enum UIState
    {
        Hidden = 0, BugMenu = 1, Scan = 2
    }

    public static class AllUIState
    {
        public static UIState[] GetStates = new UIState[] { UIState.Hidden, UIState.BugMenu, UIState.Scan };

        public static UIState[] GetExept(UIState state)
        {
            List<UIState> l = GetStates.ToList();
            l.Remove(state);
            return l.ToArray();
        }

        public static UIState[] GetExept(UIState[] states)
        {
            List<UIState> l = GetStates.ToList();
            foreach (UIState item in states)
            {
                l.Remove(item);
            }
            return l.ToArray();
        }
    }
}