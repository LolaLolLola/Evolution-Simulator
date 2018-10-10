#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Assets._Scripts.Enums
{
    public enum Connection
    {
        Top = 0, Right = 1, Bottom = 2, Left = 3, None = 4
    }

    public static class AllConnection
    {
        public static Connection[] GetStates = new Connection[] { Connection.Top, Connection.Left, Connection.Bottom, Connection.Right };

        public static Connection[] GetExept(Connection state)
        {
            List<Connection> l = GetStates.ToList();
            l.Remove(state);
            return l.ToArray();
        }

        public static Connection[] GetExept(Connection[] states)
        {
            List<Connection> l = GetStates.ToList();
            foreach (Connection item in states)
            {
                l.Remove(item);
            }
            return l.ToArray();
        }
    }
}