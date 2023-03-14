//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
namespace DungeonArchitect.Graphs
{
    public abstract class GraphBuilder 
    {
        protected Graph graph;
        public Graph Graph { get { return graph; } }

        public GraphBuilder(Graph graph)
        {
            this.graph = graph;
        }

        public abstract void DestroyNode(GraphNode node);
        public abstract GraphNode CreateNode(System.Type nodeType);
        public abstract TLink LinkNodes<TLink>(GraphPin outputPin, GraphPin inputPin) where TLink : GraphLink;
        public T CreateNode<T>() where T : GraphNode
        {
            return CreateNode(typeof(T)) as T;
        }

        public void DestroyAllNodes()
        {
            var nodes = graph.Nodes.ToArray();
            foreach (var node in nodes)
            {
                DestroyNode(node);
            }
        }
    }
}
