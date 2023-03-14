//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.Isaac
{
    public abstract class IsaacRoomLayoutBuilder : MonoBehaviour
    {

        public abstract IsaacRoomLayout GenerateLayout(IsaacRoom room, System.Random random, int roomWidth, int roomHeight);
    }
}