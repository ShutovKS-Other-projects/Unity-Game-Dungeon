//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    /// <summary>
    /// Selector rule allow you to attach selection behavior to decide if a visual node should be inserted into the scene
    /// </summary>
    public class SelectorRule : ScriptableObject
    {
        /// <summary>
        /// Implementations should override this and determine if the node should be selected (inserted into the scene)
        /// </summary>
        /// <param name="socket">The marker data-structure</param>
        /// <param name="propTransform">The combined transform of the visual node that invoked this rule</param>
        /// <param name="model">The dungeon model</param>
        /// <param name="random">The random stream used by the builder. User this random stream for any randomness for consistancy</param>
        /// <returns></returns>
        public virtual bool CanSelect(PropSocket socket, Matrix4x4 propTransform, DungeonModel model, System.Random random)
        {
            return true;
        }
    }
}