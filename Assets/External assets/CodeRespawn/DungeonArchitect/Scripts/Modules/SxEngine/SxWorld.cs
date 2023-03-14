//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.SxEngine
{
    public class SxWorld
    {
        private SxSceneGraph sceneGraph = new SxSceneGraph();

        public SxRootSceneNode RootNode
        {
            get => sceneGraph.RootNode;
        }
        
        public virtual void Draw(SxRenderContext context, SxRenderCommandList renderCommandList)
        {
            var visited = new HashSet<ISxSceneNode>();
            DrawRecursive(context, sceneGraph.RootNode, Matrix4x4.identity, visited, renderCommandList);
        }
        
        void DrawRecursive(SxRenderContext context, ISxSceneNode node, Matrix4x4 incomingWorldTransform, HashSet<ISxSceneNode> visited,
                SxRenderCommandList renderCommandList)
        {
            if (visited.Contains(node))
            {
                return;
            }

            visited.Add(node);

            var accumulatedWorldTransform = incomingWorldTransform * node.WorldTransform.Matrix;
            node.Draw(context, accumulatedWorldTransform, renderCommandList);
            
            // Iterate the children
            foreach (var childNode in node.Children)
            {
                DrawRecursive(context, childNode, accumulatedWorldTransform, visited, renderCommandList);
            }
        }
        
        public void Tick(SxRenderContext context, float deltaTime)
        {
            sceneGraph.IterateNodes((node) => node.Tick(context, deltaTime));
        }

        public void Clear()
        {
            sceneGraph.IterateNodes((node) =>
            {
                if (node is SxActor)
                {
                    DestroyActorImpl(node as SxActor);
                }
            });
            
            sceneGraph.RootNode.RemoveAllChildren();
        }

        public T SpawnActor<T>(bool addToRoot) where T : SxActor, new()
        {
            var actor = new T();
            actor.World = this;

            if (addToRoot)
            {
                sceneGraph.RootNode.AddChild(actor);
            }

            return actor;
        }

        public T[] GetActorsOfType<T>() where T : SxActor
        {
            var result = new List<T>();
            sceneGraph.IterateNodes(node =>
            {
                if (node is T)
                {
                    result.Add(node as T);
                }
            });
            return result.ToArray();
        }
        
        public void DestroyActor(SxActor actor)
        {
            DestroyActorImpl(actor);
            
            // remove from the scene graph
            sceneGraph.Remove(actor);
        }

        private void DestroyActorImpl(SxActor actor)
        {
            foreach (var component in actor.Components)
            {
                component.Destroy();
            }
        }
        
    }

}