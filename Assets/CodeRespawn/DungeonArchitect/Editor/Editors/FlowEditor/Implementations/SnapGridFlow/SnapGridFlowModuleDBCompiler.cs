//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Flow.Impl.SnapGridFlow;
using DungeonArchitect.Frameworks.Snap;
using DungeonArchitect.Utils;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.Flow.Impl
{
    public class SnapGridFlowModuleDBCompiler
    {
        public enum CompileErrorType
        {
            Success,
            Warning,
            Error
        }
        
        public struct CompileResultEntry
        {
            public CompileErrorType errorType;
            public string message;
            public int moduleIndex;

            public CompileResultEntry(CompileErrorType errorType, string message) : this(errorType, -1, message) {}
            public CompileResultEntry(CompileErrorType errorType, int moduleIndex, string message)
            {
                this.errorType = errorType;
                this.message = message;
                this.moduleIndex = moduleIndex;
            }
        }

        private static SgfModuleDatabaseConnectionInfo[] GenerateConnectionInfo(GameObject module)
        {
            var baseModuleInverse = module.transform.localToWorldMatrix.inverse;
            baseModuleInverse = Matrix4x4.TRS(Matrix.GetTranslation(ref baseModuleInverse), Matrix.GetRotation(ref baseModuleInverse), Vector3.one);
            
            var result = new List<SgfModuleDatabaseConnectionInfo>();
            var connectionComponents = module.GetComponentsInChildren<SnapConnection>();
            for (var i = 0; i < connectionComponents.Length; i++)
            {
                var connectionComp = connectionComponents[i];
                var transform = connectionComp.transform;
                var worldTransform = baseModuleInverse * transform.localToWorldMatrix; // Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);

                var info = new SgfModuleDatabaseConnectionInfo();
                info.ConnectionIndex = i;
                info.Transform = worldTransform;
                info.Category = connectionComp.category;
                result.Add(info);
            }

            return result.ToArray();
        }
        
        public static bool Build(SnapGridFlowModuleDatabase moduleDatabase, out CompileResultEntry[] errors)
        {
            if (moduleDatabase == null)
            {
                errors = new CompileResultEntry[] { new CompileResultEntry(CompileErrorType.Error, "Missing module database reference") };
                return false;
            }

            if (moduleDatabase.ModuleBoundsAsset == null)
            {
                errors = new CompileResultEntry[] { new CompileResultEntry(CompileErrorType.Error, "Missing module bounds reference") };
                return false;
            }

            var bounds = moduleDatabase.ModuleBoundsAsset;
            var errorList = new List<CompileResultEntry>();
            for (var m = 0; m < moduleDatabase.Modules.Length; m++)
            {
                var item = moduleDatabase.Modules[m];
                if (item.ModulePrefab == null)
                {
                    errorList.Add(new CompileResultEntry(CompileErrorType.Error, m, "Missing module prefab reference"));
                    continue;
                }

                if (item.ModulePrefab.moduleBounds != bounds)
                {
                    errorList.Add(new CompileResultEntry(CompileErrorType.Error, m, "Different bounds asset specified in module prefab"));
                    continue;
                }

                // Build this entry
                var moduleSize = Vector3.Scale(bounds.chunkSize, DungeonArchitect.Utils.MathUtils.ToVector3(item.ModulePrefab.numChunks));
                var moduleBounds = new Bounds(moduleSize * 0.5f, moduleSize);
                item.ModuleBounds = moduleBounds;
                item.NumChunks = item.ModulePrefab.numChunks;
                item.Connections = GenerateConnectionInfo(item.ModulePrefab.gameObject);

                // Build the assemblies
                if (item.allowRotation)
                {
                    item.RotatedAssemblies = new SgfModuleAssembly[4];
                    SGFModuleAssemblyBuilder.Build(bounds, item, out item.RotatedAssemblies[0]);
                    for (int r = 1; r < 4; r++)
                    {
                        SGFModuleAssemblyBuilder.Rotate90Cw(item.RotatedAssemblies[r - 1], out item.RotatedAssemblies[r]);
                    }
                }
                else
                {
                    item.RotatedAssemblies = new SgfModuleAssembly[1];
                    SGFModuleAssemblyBuilder.Build(bounds, item, out item.RotatedAssemblies[0]);
                }
                
                // build the placeable item list
                {
                    var placeableItems = new Dictionary<PlaceableMarker, int>();
                    var placeableMarkers = item.ModulePrefab.gameObject.GetComponentsInChildren<PlaceableMarker>();
                    foreach (var placeableMarker in placeableMarkers)
                    {
                        var prefabTemplate = PrefabUtility.GetCorrespondingObjectFromOriginalSource(placeableMarker);
                        if (prefabTemplate == null) continue;
                        
                        if (!placeableItems.ContainsKey(prefabTemplate))
                        {
                            placeableItems.Add(prefabTemplate, 0);
                        }

                        placeableItems[prefabTemplate]++;
                    }

                    var result = new List<SgfModuleDatabasePlaceableMarkerInfo>();
                    foreach (var entry in placeableItems)
                    {
                        var info = new SgfModuleDatabasePlaceableMarkerInfo();
                        info.placeableMarkerTemplate = entry.Key;
                        info.count = entry.Value;
                        result.Add(info);
                    }

                    item.AvailableMarkers = result.ToArray();
                }
            }

            EditorUtility.SetDirty(moduleDatabase);

            errors = errorList.ToArray();
            var success = (errors.Length == 0);
            return success;
        }
    }
}