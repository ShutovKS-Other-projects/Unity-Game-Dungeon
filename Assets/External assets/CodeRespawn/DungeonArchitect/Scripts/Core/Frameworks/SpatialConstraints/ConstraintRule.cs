//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Linq;
using DungeonArchitect.Graphs.SpatialConstraints;

namespace DungeonArchitect.SpatialConstraints
{
    public class SpatialConstraintRuleDomain
    {
        public SCReferenceNode referenceNode;
        public Vector3 gridSize = Vector3.one;
    }
    
    public class RuleMetaAttribute : System.Attribute
    {
        public string name;
    }

    public class ConstraintRuleContext
    {
        public SpatialConstraintProcessorContext processorContext;
        public SpatialConstraintRuleDomain domain;
        public SCRuleNode ruleNode;
        public Vector3 ruleNodeWorldPosition;
        public Matrix4x4 rotationFrame;
    }

    public abstract class ConstraintRule : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        public bool enabled = true;

        [SerializeField]
        [HideInInspector]
        public string ruleName;

        [SerializeField]
        public bool inverseRule = false;

        public virtual void OnEnable()
        {
            hideFlags = HideFlags.HideInHierarchy;
            ruleName = GetScriptName();
        }

        public abstract bool Process(ConstraintRuleContext context);

        public static string GetFullMenuPath(System.Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(RuleMetaAttribute), true).FirstOrDefault() as RuleMetaAttribute;
            string path = (attribute != null) ? attribute.name : type.Name;
            return path;
        }

        public static string GetScriptName(System.Type type)
        {
            var path = GetFullMenuPath(type);
            var fileInfo = new System.IO.FileInfo(path);
            return fileInfo.Name;
        }

        public string GetFullMenuPath()
        {
            return GetFullMenuPath(GetType());
        }

        public string GetScriptName()
        {
            return GetScriptName(GetType());
        }

        public override string ToString()
        {
            if (inverseRule)
            {
                return ruleName + " Inverse";
            }
            else
            {
                return ruleName;
            }
        }
    }
}
