using System.Collections.Generic;
using System.Linq;

namespace Assets._Scripts.Enums.Mous
{
    public enum MousState
    {
        Normal = 0, PickedUp = 1, OverInfo, Scaning = 3
    }

    public static class AllMouseSate
    {
        public static MousState[] GetStates = new MousState[] { MousState.Normal, MousState.PickedUp, MousState.Scaning };

        public static MousState[] GetExept(MousState state)
        {
            List<MousState> l = GetStates.ToList();
            l.Remove(state);
            return l.ToArray();
        }

        public static MousState[] GetExept(MousState[] states)
        {
            List<MousState> l = GetStates.ToList();
            foreach (MousState item in states)
            {
                l.Remove(item);
            }
            return l.ToArray();
        }
    }
}