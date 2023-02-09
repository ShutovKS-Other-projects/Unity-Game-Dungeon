//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    /// <summary>
    /// Selector rule allow you to attach selection behavior to decide if a visual node should be inserted into the scene
    /// </summary>
    public class TransformationRule : ScriptableObject
    {
        /// <summary>
        /// Implement this method to provide a transform based on your logic.
        /// </summary>
        /// <param name="socket">The marker data structure</param>
        /// <param name="model">The dungeon model</param>
        /// <param name="propTransform">The combined transform of the visual node that invoked this rule</param>
        /// <param name="random">The random stream used by the builder. User this random stream for any randomness for consistancy</param>
        /// <param name="outPosition">Set your position offset here</param>
        /// <param name="outRotation">Set your rotation offset here</param>
        /// <param name="outScale">Set your scale offset here</param>
        public virtual void GetTransform(PropSocket socket, DungeonModel model, Matrix4x4 propTransform, System.Random random, out Vector3 outPosition, out Quaternion outRotation, out Vector3 outScale)
        {
            outPosition = Vector3.zero;
            outRotation = Quaternion.identity;
            outScale = Vector3.one;
        }

    }
}
