using Assets._Scripts.Bugs;
using Assets._Scripts.Bugs.Parts;
using Assets._Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Interfaces
{
    public interface IConnectable
    {
        int this[Connection value] { get; }

        Task<Connection[]> GetOpenConnections();
        bool ConnectTo(Connection connection, int id);
    }
}
