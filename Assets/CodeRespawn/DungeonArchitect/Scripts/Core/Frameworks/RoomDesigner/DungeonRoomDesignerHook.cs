//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Linq;
using UnityEngine;

namespace DungeonArchitect.RoomDesigner
{
    public class DungeonRoomDesignerHook : DungeonEventListener
    {

        public override void OnPostDungeonLayoutBuild(Dungeon dungeon, DungeonModel model)
        {
            var rooms = GameObject.FindObjectsOfType<DungeonRoomDesigner>().Where(p => p.dungeon == dungeon).ToList();
            rooms.ForEach(r => r.GenerateLayout());
        }

        public override void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers)
        {
            var rooms = GameObject.FindObjectsOfType<DungeonRoomDesigner>().Where(p => p.dungeon == dungeon).ToList();
            rooms.ForEach(r => r.EmitMarkers(markers));
        }
    }
}
