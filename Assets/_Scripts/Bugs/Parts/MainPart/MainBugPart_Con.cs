#pragma warning disable IDE0044 // Add readonly modifier

#region using

using Assets._Scripts.Enums;
using Assets._Scripts.Generators;
using Assets._Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#endregion

namespace Assets._Scripts.Bugs.Parts.MainPart
{
    public abstract partial class MainBugPart
    {
        private static bool CheckLocation(List<Vector3> locations, Vector3 checkPos)
        {
            bool b = true;

            foreach (Vector3 item in locations)
            {
                if (M.Mathn.Distance(item.x, item.z, checkPos.x, checkPos.z) < 0.5f) b = false;
            }

            return b;
        }

        public async Task<Connection[]> GetOpenConnections() => GetOpenConnections(mChildrenIDs, Locations, transform.position, 0.6f, transform.lossyScale.x);
        public static Connection[] GetOpenConnections(Dictionary<Connection, int> connections, List<Vector3> locations, Vector3 parentPos, float distance, float paretenScale)
        {
            List<Connection> buffer = new List<Connection>();

            foreach (Connection item in Enum.GetValues(typeof(Connection)))
            {
                if (item != Connection.None)
                {
                    Vector3 futurePos = BugPart.GetConnectionPos(item, parentPos, distance * paretenScale);

                    if (connections[item] == -1 && CheckLocation(locations, futurePos))
                    {
                        buffer.Add(item);
                    }
                }
            }

            if (buffer.Count == 0)
            {
                buffer.Add(Connection.None);
            }

            return buffer.ToArray();
        }

        public bool ConnectTo(Connection connection, int id)
        {
            if (mChildrenIDs[connection] != -1) throw new Exception("Connection Overflow");

            mChildrenIDs[connection] = id;
            return true;
        }

        private async Task<Tuple<IConnectable, Connection[]>> GetRandomConnector()
        {
            IConnectable Connector = M.Mathn.SelectRandmo(mConnectables.ToArray(), GeneratorController.SafeRandom());
            Connection[] openConnections = await Connector.GetOpenConnections();

            for (int d = 0; d < 10; d++)
            {
                if (openConnections[0] != Connection.None) break;

                Connector = M.Mathn.SelectRandmo(mConnectables.ToArray(), GeneratorController.SafeRandom());
                openConnections = await Connector.GetOpenConnections();
            }
            return new Tuple<IConnectable, Connection[]>(Connector, openConnections);
        }
    }
}