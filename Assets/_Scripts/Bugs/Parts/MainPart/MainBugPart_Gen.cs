#pragma warning disable IDE0044 // Add readonly modifier

#region using

using Assets._Scripts.Enums;
using Assets._Scripts.Generators;
using Assets._Scripts.Interfaces;
using System;
using System.Threading.Tasks;
using UnityEngine;

#endregion

namespace Assets._Scripts.Bugs.Parts.MainPart
{
    public abstract partial class MainBugPart
    {
        private async Task GenerateParts()
        {
            for (int i = 0; i < pTargetBodys; i++)
            {
                await CreateBugPart(typeof(Body));
            }

            for (int i = 0; i < pTaregtEyes; i++)
            {
                await CreateBugPart(typeof(Eye));
            }
        }

        private void GenerateParts(BugStructur bugStructur)
        {
            foreach (BugPartData part in mBugStructur)
            {
                CreateBugPart(part.Fuction, part.ParentConnection, part.Connections[part.ParentConnection]);
            }
        }

        private async Task CreateBugPart(Type fuction)
        {
            BugPart part = Instantiate(GetPrefab(fuction), new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)).GetComponent<BugPart>();

            Tuple<IConnectable, Connection[]> tuple = await GetRandomConnector();
            RegisterPartRandom(part, tuple.Item2, tuple.Item1);
        }

        private void CreateBugPart(Type fuction, Connection parentConnection, int parentID)
        {
            BugPart part = Instantiate(GetPrefab(fuction), new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)).GetComponent<BugPart>();

            RegisterPart(part, parentConnection, parentID);
        }

        private void RegisterPartRandom(BugPart part, Connection[] posibleConnections, IConnectable parent)
        {
            if (part.SetConnection(M.Mathn.SelectRandmo(posibleConnections, GeneratorController.SafeRandom()), parent, ((MonoBehaviour)parent).gameObject, this))
            {
                Locations.Add(part.transform.position);
                mBugParts.Add(part);

                if (part is IConnectable)
                    mConnectables.Add(part as IConnectable);

                if (mFuctionParts.ContainsKey(part.GetType()))
                    mFuctionParts[part.GetType()].Add(part);
            }
        }

        private void RegisterPart(BugPart part, Connection parentConnection, int parentID)
        {
            if (GetBugPart(parentID) == null) throw new Exception("NULL");
            if (part.SetConnection(parentConnection, GetBugPart(parentID) as IConnectable, ((MonoBehaviour)GetBugPart(parentID)).gameObject, this))
            {
                Locations.Add(part.transform.position);
                mBugParts.Add(part);

                if (part is IConnectable)
                    mConnectables.Add(part as IConnectable);

                if (mFuctionParts.ContainsKey(part.GetType()))
                    mFuctionParts[part.GetType()].Add(part);
            }
        }
    }
}