//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Builders.Grid
{
    public class GridItemMetadataHandler : DungeonItemSpawnListener
    {
        public override void SetMetadata(GameObject dungeonItem, DungeonNodeSpawnData spawnData)
        {
            if (spawnData.socket.SocketType == GridDungeonMarkerNames.Door)
            {
                var doorMeta = dungeonItem.GetComponent<GridItemDoorMetadata>();
                if (doorMeta == null)
                {
                    doorMeta = dungeonItem.AddComponent<GridItemDoorMetadata>();
                }

                if (spawnData.socket.metadata is GridBuilderDoorMetadata)
                {
                    var builderDoorMeta = spawnData.socket.metadata as GridBuilderDoorMetadata;
                    doorMeta.cellA = builderDoorMeta.CellA;
                    doorMeta.cellB = builderDoorMeta.CellB;
                }
            }
            else
            {
                if (dungeonItem != null)
                {
                    // Not a door. Make sure we don't have the door metadata attached to it 
                    // (this object might be reused when the theme graph changes)
                    var doorMeta = dungeonItem.GetComponent<GridItemDoorMetadata>();
                    if (doorMeta)
                    {
                        Destroy(doorMeta);
                    }
                }
            }
        }
    }
}
