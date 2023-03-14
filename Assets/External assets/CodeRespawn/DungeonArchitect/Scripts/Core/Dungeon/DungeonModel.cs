//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    /// <summary>
    /// Abstract dungeon model.  Create your own implementation of the model depending on your builder's needs
    /// </summary>
	//[System.Serializable]
	public abstract class DungeonModel : MonoBehaviour
	{
        void Reset()
        {
            ResetModel();
        }

        public virtual void ResetModel() { }

	}
}
