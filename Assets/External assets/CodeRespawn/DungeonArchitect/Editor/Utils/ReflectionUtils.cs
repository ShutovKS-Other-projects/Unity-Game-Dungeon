//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace DungeonArchitect.Editors
{
    public class ReflectionUtils
    {
        public static System.Type[] GetAllSubtypes(System.Type baseType, bool fromAllAssemblies)
        {
            var types = GetAllTypes(fromAllAssemblies);
            return (from System.Type type in types where type.IsSubclassOf(baseType) select type).ToArray();
        }

        public static System.Type[] GetAllTypes(bool fromAllAssemblies)
        {
            var assemblies = new List<Assembly>();
            assemblies.Add(System.Reflection.Assembly.GetExecutingAssembly());
            if (fromAllAssemblies)
            {
                foreach (var assemblyName in System.Reflection.Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                {
                    var assembly = System.Reflection.Assembly.Load(assemblyName.ToString());
                    assemblies.Add(assembly);
                }
            }

            var types = new List<System.Type>();
            foreach (var assembly in assemblies)
            {
                types.AddRange(assembly.GetTypes());
            }

            return types.ToArray();
        }
    }
}
