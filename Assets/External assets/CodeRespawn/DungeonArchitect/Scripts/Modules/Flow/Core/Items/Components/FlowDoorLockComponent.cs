//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Flow.Items
{
    public class FlowDoorLockComponent : MonoBehaviour
    {
        public string lockId;
        public string[] validKeyIds = new string[0];
        public FlowDoorKeyComponent[] validKeyRefs = new FlowDoorKeyComponent[0];
    }
}
