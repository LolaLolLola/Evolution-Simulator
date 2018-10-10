#pragma warning disable IDE0044 // Add readonly modifier

#region using

using Assets._Scripts.Bugs.Parts;
using Assets._Scripts.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Assets._Scripts.Bugs
{
    public class BugStructur : IEnumerable<BugPartData>
    {
        private List<BugPartData> mBugParts;
        private IEnumerable<BugPartData> mSortedBugParts;
        private Dictionary<Vector2, Type> mVecParts;
        private bool mSorted;

        public int Count => mBugParts.Count;

        public BugStructur()
        {
            mBugParts = new List<BugPartData>();
            mVecParts = new Dictionary<Vector2, Type>();
        }

        public void AddVecPart(Vector2 pos, Type function)
        {
            mVecParts.Add(pos, function);
        }

        public void AddBugPart(BugPart part)
        {
            mSorted = false;
            mBugParts.Add(part.GetPartData());
        }

        public void AddBugPart(BugPartData part, int id)
        {
            if (GetPartData(id) == null) return;

            mSorted = false;
            part.ID = id;
            mBugParts.Add(part);
        }

        public void AddBugPart(int id, Type fuction, int parentID, Connection con)
        {
            if (GetPartData(id) == null) return;

            mSorted = false;
            mBugParts.Add(new BugPartData(id, fuction, con, parentID));
        }

        public Dictionary<Connection, Type> GetChildren(int id)
        {
            Dictionary<Connection, Type> dic = new Dictionary<Connection, Type>();

            foreach (BugPartData item in from data in mBugParts where data.Connections.ContainsValue(id) select data)
            {
                if (item.Connections[item.ParentConnection] == id)
                {
                    dic.Add(item.ParentConnection, item.Fuction);
                }
            }

            return dic;
        }

        public int AmountOf(Type fuction)
        {
            IEnumerable<BugPartData> e = from data in mBugParts where data.Fuction == fuction select data;
            return e.ToArray().Length;
        }

        private void Sort()
        {
            mSortedBugParts = from data in mBugParts orderby data.ID ascending select data;
            mSorted = true;
        }

        public BugPartData GetPartData(int parentID, Connection con)
        {
            foreach (BugPartData item in from data in mBugParts where data.ParentConnection == con && data.Connections[data.ParentConnection] == parentID select data)
            {
                return item;
            }

            return null;
        }

        public BugPartData GetPartData(int id)
        {
            foreach (BugPartData item in from data in mBugParts where data.ID == id select data)
            {
                return item;
            }

            return null;
        }

        public BugPartData GetPartData(Vector2 pos)
        {
            foreach (BugPartData item in from data in mBugParts where data.Pos == pos select data)
            {
                return item;
            }

            return null;
        }

        #region Max Min

        public int MaxX()
        {
            foreach (BugPartData item in from data in mBugParts orderby data.Pos.x descending select data)
            {
                return (int)item.Pos.x;
            }

            return 0;
        }

        public int MaxY()
        {
            foreach (BugPartData item in from data in mBugParts orderby data.Pos.y descending select data)
            {
                return (int)item.Pos.y;
            }

            return 0;
        }

        public int MinX()
        {
            foreach (BugPartData item in from data in mBugParts orderby data.Pos.x ascending select data)
            {
                return (int)item.Pos.x;
            }

            return 0;
        }

        public int MinY()
        {
            foreach (BugPartData item in from data in mBugParts orderby data.Pos.y ascending select data)
            {
                return (int)item.Pos.y;
            }

            return 0;
        }

        #endregion

        public IEnumerator<BugPartData> GetEnumerator()
        {
            if (!mSorted) Sort();

            return mSortedBugParts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!mSorted) Sort();

            return mSortedBugParts.GetEnumerator();
        }

        public void WriteVecParts()
        {
            mBugParts.Clear();
            mSorted = false;


            int id = 1;
            AddBugPart(1, );

        }

        public void SetVecPart(ref int Id, Connection parentConnection)
        {

        }

        public void CalculatePoses()
        {
            foreach (Connection item in AllConnection.GetStates)
            {
                SetPoses(GetPartData(0, item), new Vector2(0, 0));
            }
        }

        private void SetPoses(BugPartData part, Vector2 parentPos)
        {
            switch (part.ParentConnection)
            {
                case Connection.Top:
                    part.Pos = parentPos + new Vector2(0, 1);
                    break;
                case Connection.Left:
                    part.Pos = parentPos + new Vector2(1, 0);
                    break;
                case Connection.Bottom:
                    part.Pos = parentPos + new Vector2(0, -1);
                    break;
                case Connection.Right:
                    part.Pos = parentPos + new Vector2(-1, 0);
                    break;
            }

            foreach (Connection item in AllConnection.GetExept(part.ParentConnection))
            {
                if (part.Connections[item] == -1) continue;

                SetPoses(GetPartData(part.Connections[item]), part.Pos);
            }
        }
    }

    public class BugPartData
    {
        public int ID { get; set; }
        public Type Fuction { get; private set; }
        public Dictionary<Connection, int> Connections { get; private set; }
        public Connection ParentConnection { get; private set; }
        public Vector2 Pos { get; set; }

        public BugPartData(int iD, Type fnuction, Connection parent, int top, int right, int bottom, int left)
        {
            ID = iD;
            Fuction = fnuction;
            ParentConnection = parent;
            Connections = new Dictionary<Connection, int>
            {
                {Connection.Top, top},
                {Connection.Right, right},
                {Connection.Bottom, bottom},
                {Connection.Left, left}
            };
        }

        public BugPartData(int iD, Type fnuction, Connection parent, int parentID)
        {
            ID = iD;
            Fuction = fnuction;
            ParentConnection = parent;
            Connections = new Dictionary<Connection, int>
            {
                {Connection.Top, -1},
                {Connection.Right, -1},
                {Connection.Bottom, -1},
                {Connection.Left, -1}
            };
            Connections[parent] = parentID;

        }

        public static bool operator ==(BugPartData first, BugPartData second)
        {
            if ((object)first == null || (object)second == null) return false;

            if (first.Connections[first.ParentConnection] != second.Connections[second.ParentConnection] ||
                first.ParentConnection != second.ParentConnection ||
                first.Fuction != second.Fuction)
                return false;

            return true;
        }

        public static bool operator !=(BugPartData first, BugPartData second)
        {
            if ((object)first != null || (object)second != null) return false;

            if (first.Connections[first.ParentConnection] == second.Connections[second.ParentConnection] &&
                first.ParentConnection == second.ParentConnection &&
                first.Fuction == second.Fuction)
                return false;

            return true;
        }
    }
}