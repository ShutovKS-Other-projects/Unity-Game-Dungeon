//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Frameworks.Snap
{
    public enum SnapConnectionState
    {
        Wall,
        Door,
        DoorOneWay,
        DoorLocked,
        None
    }

    [System.Serializable]
    public struct SnapConnectionLockedDoorInfo
    {
        public string markerName;
        public GameObject lockedDoorObject;
    }

    public class SnapConnection : MonoBehaviour
    {
        public GameObject doorObject;
        public GameObject wallObject;
        public string category;

        public GameObject oneWayDoorObject;
        public SnapConnectionLockedDoorInfo[] lockedDoors;
        public SnapConnectionState connectionState = SnapConnectionState.None;

        public GameObject UpdateDoorState(SnapConnectionState state)
        {
            return UpdateDoorState(state, "");
        }

        public GameObject UpdateDoorState(SnapConnectionState state, string markerName)
        {
            connectionState = state;
            DeactivateAll();
            if (state == SnapConnectionState.Door)
            {
                SafeSetActive(doorObject, true);
                return doorObject;
            }
            else if (state == SnapConnectionState.Wall)
            {
                SafeSetActive(wallObject, true);
                return wallObject;
            }
            else if (state == SnapConnectionState.DoorOneWay)
            {
                SafeSetActive(oneWayDoorObject, true);
                return oneWayDoorObject;
            }
            else if (state == SnapConnectionState.DoorLocked)
            {
                if (lockedDoors != null)
                {
                    foreach (var lockInfo in lockedDoors)
                    {
                        if (lockInfo.markerName == markerName)
                        {
                            SafeSetActive(lockInfo.lockedDoorObject, true);
                            return lockInfo.lockedDoorObject;
                        }
                    }
                }
            }

            return null;
        }

        void DeactivateAll()
        {
            SafeSetActive(doorObject, false);
            SafeSetActive(wallObject, false);
            SafeSetActive(oneWayDoorObject, false);
            if (lockedDoors != null)
            {
                foreach (var lockedDoor in lockedDoors)
                {
                    SafeSetActive(lockedDoor.lockedDoorObject, false);
                }
            }
        }

        void SafeSetActive(GameObject obj, bool active)
        {
            if (obj != null)
            {
                obj.SetActive(active);
            }
        }

        void OnDrawGizmos()
        {
            if (transform != null)
            {
                var start = transform.position;
                var end = start + transform.forward;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(start, end);
            }

        }
        
        public bool IsWallState()
        {
            return connectionState == SnapConnectionState.Wall;
        }
        
        public bool IsDoorState()
        {
            return connectionState == SnapConnectionState.Door
                   || connectionState == SnapConnectionState.DoorLocked
                   || connectionState == SnapConnectionState.DoorOneWay;
        }

    }
}
