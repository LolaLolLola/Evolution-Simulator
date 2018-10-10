#pragma warning disable IDE0044 // Add readonly modifier

#region using

using Assets._Scripts.Bugs.Parts;
using Assets._Scripts.Bugs.Parts.MainPart;
using Assets._Scripts.Enums;
using Assets._Scripts.Interfaces;
using System;
using UnityEngine;

#endregion

namespace Assets._Scripts.Bugs
{
    public abstract class BugPart : MonoBehaviour
    {
        protected float pDistance;
        protected bool pConnectionSet;

        public MainBugPart Main { get; private set; }

        public IConnectable Parent { get; protected set; }
        protected Connection pParentConnection { get; private set; }

        public int ID { get; private set; }

        public BugPart()
        {
            pConnectionSet = false;
            pDistance = 0.6f;
        }

        public virtual bool SetConnection(Connection connection, IConnectable connector, GameObject parent, MainBugPart main)
        {
            Main = main;
            ID = main.PartID;

            if (connection == Connection.None)
            {
                Destroy(gameObject);
                return false;
            } 

            transform.SetParent(parent.transform);

            transform.position = GetConnectionPos(connection, parent.transform.position, pDistance * parent.transform.lossyScale.x);

            connector.ConnectTo(connection, ID);
            Parent = connector;

            pParentConnection = connection;
            pConnectionSet = true;

            return true;
        }

        public static Vector3 GetConnectionPos(Connection connection, Vector3 centerPos, float fixDistance)
        {
            switch (connection)
            {
                case Connection.Top:
                    return centerPos + new Vector3(0, 0, fixDistance);
                case Connection.Right:
                    return centerPos + new Vector3(fixDistance, 0, 0);
                case Connection.Bottom:
                    return centerPos + new Vector3(0, 0, -fixDistance);
                case Connection.Left:
                    return centerPos + new Vector3(-fixDistance, 0, 0);
            }

            return Vector3.zero;
        }

        public BugPartData GetPartData()
        {
            if (this is IConnectable)
            {
                IConnectable con = this as IConnectable;
                return new BugPartData(ID, GetType(), pParentConnection, con[Connection.Top], con[Connection.Right], con[Connection.Bottom], con[Connection.Left]);
            }
            else
            {
                if (Parent is BugPart)
                {
                    return new BugPartData(ID, GetType(), pParentConnection, (Parent as BugPart).ID);
                }
                else
                {
                    return new BugPartData(ID, GetType(), pParentConnection, (Parent as MainBugPart).ID);
                }
            }
        }
    }
}