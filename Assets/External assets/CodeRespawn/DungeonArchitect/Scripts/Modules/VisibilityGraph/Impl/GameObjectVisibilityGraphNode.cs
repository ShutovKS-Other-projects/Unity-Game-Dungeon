//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Visibility.Impl
{
    public class GameObjectVisibilityGraphNode : VisibilityGraphNode
    {
        private GameObject[] gameObjects;
        
        public GameObjectVisibilityGraphNode(GameObject gameObject)
        {
            gameObjects = new GameObject[] { gameObject };
        }

        public GameObjectVisibilityGraphNode(GameObject[] gameObjects)
        {
            this.gameObjects = gameObjects.Clone() as GameObject[];
        }
        
        public override void SetVisibleImpl(bool visible)
        {   
            if (gameObjects != null)
            {
                foreach (var gameObject in gameObjects)
                {
                    gameObject.SetActive(visible);
                }
            }
        }
        
        public override Bounds CalculateBounds()
        {
            var bounds = new Bounds();
            bool foundBounds = false;
            foreach (var gameObject in gameObjects)
            {
                Bounds itemBounds;
                if (CalculateBounds(gameObject, out itemBounds))
                {
                    if (!foundBounds)
                    {
                        bounds = itemBounds;
                        foundBounds = true;
                    }
                    else
                    {
                        bounds.Encapsulate(itemBounds);
                    }
                }
            }
            
            return bounds;
        }
        
        private bool CalculateBounds(GameObject target, out Bounds bounds)
        {
            if (target == null)
            {
                bounds = new Bounds();
                return false;
            }
            
            if (target.transform.childCount == 0)
            {
                var renderer = target.GetComponent<Renderer>();
                if (renderer == null)
                {
                    bounds = new Bounds();
                    return false;
                }
                else
                {
                    bounds = renderer.bounds;
                    return true;
                }
            }

            bounds = new Bounds();
            bool foundBounds = false;
            var stack = new Stack<GameObject>();
            stack.Push(target);
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                if (top == null) continue;
                
                var renderer = top.GetComponent<Renderer>();
                if (renderer != null)
                {
                    if (!foundBounds)
                    {
                        bounds = renderer.bounds;
                        foundBounds = true;
                    }
                    else
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                }

                // Add the children
                for (int i = 0; i < top.transform.childCount; i++)
                {
                    var child = top.transform.GetChild(i);
                    if (child == null) continue;
                    stack.Push(child.gameObject);
                }
            }

            return foundBounds;
        }
        
    }
}