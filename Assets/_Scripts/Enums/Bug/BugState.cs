using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Scripts.Enums.Bug
{
    public enum BugState
    {
        Loading = -1, Normal = 0, Horny = 1, InAir = 2, OverDelet = 3,  Scaneing = 4
    }


    public static class AllBugSate
    {
        public static BugState[] GetStates = new BugState[] { BugState.Loading, BugState.Normal, BugState.Horny, BugState.InAir, BugState.OverDelet, BugState.Scaneing };

        public static BugState[] GetExept(BugState state)
        {
            List<BugState> l = GetStates.ToList();
            l.Remove(state);
            return l.ToArray();
        }

        public static BugState[] GetExept(BugState[] states)
        {
            List<BugState> l = GetStates.ToList();
            foreach (BugState item in states)
            {
                l.Remove(item);
            }
            return l.ToArray();
        }
    }
}
