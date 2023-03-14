//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    public class InfinityDungeonEditorUpdate : MonoBehaviour
    {
        public InfinityDungeon infinityDungeon;

        public void EditorUpdate()
        {
            if (infinityDungeon != null)
            {
                infinityDungeon.EditorUpdate();
            }
        }
    }
}
