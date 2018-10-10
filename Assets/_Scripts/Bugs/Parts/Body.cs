#pragma warning disable IDE0044 // Add readonly modifier

#region using

using Assets._Scripts.Bugs.Parts.MainPart;
using Assets._Scripts.Enums;
using Assets._Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#endregion

namespace Assets._Scripts.Bugs.Parts
{
    public class Body : BugPart, IConnectable
    {
        private Dictionary<Connection, int> mChildrenIDs;

        public int this[Connection value] { get { return mChildrenIDs[value]; } }

        private void Awake()
        {
            mChildrenIDs = new Dictionary<Connection, int>
            {
                { Connection.Top, -1 },
                { Connection.Right, -1 },
                { Connection.Bottom, -1 },
                { Connection.Left, -1 }
            };
        }

        public async Task<Connection[]> GetOpenConnections()
        {
            while (!pConnectionSet || Main == null)
            {
                await Task.Delay(25);
            }

            return MainBugPart.GetOpenConnections(mChildrenIDs, Main.Locations, transform.position, pDistance, transform.lossyScale.x);
        }

        public bool ConnectTo(Connection connection, int id)
        {
            if (mChildrenIDs[connection] != -1) return false;

            mChildrenIDs[connection] = id;
            return true;
        }

        public override bool SetConnection(Connection connection, IConnectable connector, GameObject parent, MainBugPart main)
        {
            pDistance = 0.7f;
            try
            {
                mChildrenIDs[connection] = parent.GetComponent<BugPart>().ID;
            }
            catch
            {
                mChildrenIDs[connection] = main.ID;
            }
            return base.SetConnection(connection, connector, parent, main);
        }
    }
}