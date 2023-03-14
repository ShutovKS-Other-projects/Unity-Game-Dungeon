//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Graphs.SpatialConstraints
{
    public class SCBaseDomainNode : GraphNode
    {
        public static readonly float TileSize = 200;

        [SerializeField]
        protected SCRuleNodeDomain ruleDomain = SCRuleNodeDomain.Invalid;
        public SCRuleNodeDomain RuleDomain
        {
            get { return ruleDomain; }
        }

        [SerializeField]
        protected bool isSnapped = false;
        public bool IsSnapped
        {
            get { return isSnapped; }
            set { isSnapped = value; }
        }

        public virtual Color GetColor()
        {
            return Color.black;
        }

        public override void Initialize(string id, Graph graph)
        {
            base.Initialize(id, graph);

            float nodeSize = TileSize / 10.0f;
            bounds.size = new Vector2(nodeSize, nodeSize);
        }

        public static Vector2 GetSnapPosition(Vector2 position)
        {
            var tileSize = SCBaseDomainNode.TileSize / 2.0f;
            position.x = Mathf.RoundToInt(position.x / tileSize) * tileSize;
            position.y = Mathf.RoundToInt(position.y / tileSize) * tileSize;
            return position;
        }

        public bool ContainsOtherNodeAt(Vector2 snappedPosition)
        {
            foreach (var node in graph.Nodes)
            {
                if (node.Bounds.center == snappedPosition && node != this)
                {
                    return true;
                }
            }
            return false;
        }

        public IntVector2 GetHalfGridLogicalCoords()
        {
            var halfTileSize = SCBaseDomainNode.TileSize / 2.0f;
            var center = bounds.center;
            var coords = new IntVector2();
            coords.x = Mathf.Abs(Mathf.RoundToInt(center.x / halfTileSize) % 2);
            coords.y = Mathf.Abs(Mathf.RoundToInt(center.y / halfTileSize) % 2);
            return coords;
        }

        void UpdateRuleDomain()
        {
            if (isSnapped)
            {
                var coords = GetHalfGridLogicalCoords();

                if (coords.x == 0 && coords.y == 0)
                {
                    ruleDomain = SCRuleNodeDomain.Corner;
                }
                else if (coords.x == 1 && coords.y == 1)
                {
                    ruleDomain = SCRuleNodeDomain.Tile;
                }
                else
                {
                    ruleDomain = SCRuleNodeDomain.Edge;
                }
            }
            else
            {
                ruleDomain = SCRuleNodeDomain.Invalid;
            }
        }

        public virtual bool SnapNode()
        {
            var snappedCenter = GetSnapPosition(bounds.center);
            if (ContainsOtherNodeAt(snappedCenter))
            {
                isSnapped = false;
            }
            else
            {
                bounds.center = snappedCenter;
                isSnapped = true;
            }

            UpdateRuleDomain();

            return isSnapped;
        }
    }
}
