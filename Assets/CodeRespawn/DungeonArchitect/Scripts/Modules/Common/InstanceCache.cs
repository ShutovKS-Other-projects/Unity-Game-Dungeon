//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace DungeonArchitect.Utils
{
    /// <summary>
    /// Caches instances by their name so they can be reused when needed again instead of recreating it
    /// </summary>
    public class InstanceCache
    {
        readonly Dictionary<string, ScriptableObject> instanceByType = new Dictionary<string, ScriptableObject>();
        /// <summary>
        /// Retrieves the instance of the specified ScriptableObject type name. If none exists, a new one is created and stored
        /// </summary>
        /// <param name="typeName">The typename of the ScriptableObject</param>
        /// <returns>The cached instance of the specified ScriptableObject typename</returns>
        public ScriptableObject GetInstance(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }
            
            if (!instanceByType.ContainsKey(typeName))
            {
                var type = System.Type.GetType(typeName);
                if (type == null)
                {
                    // Search all assemblies
                    foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        type = asm.GetType(typeName);
                        if (type != null)
                        {
                            break;
                        }
                    }
                }
                var obj = ScriptableObject.CreateInstance(type);
                instanceByType.Add(typeName, obj);
            }
            return instanceByType[typeName];
        }

        public void Clear()
        {
            foreach (var entry in instanceByType)
            {
                var obj = entry.Value;
                if (obj != null)
                {
                    ObjectUtils.DestroyObject(obj);
                }
            }
            instanceByType.Clear();
        }
    }
}
