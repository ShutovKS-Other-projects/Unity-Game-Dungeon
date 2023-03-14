//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.SxEngine.Utils;
using UnityEngine;

namespace DungeonArchitect.SxEngine
{
    public class SxActor : ISxSceneNode
    {
        private List<SxActorComponent> components = new List<SxActorComponent>();
        
        private SxTransform worldTransform = SxTransform.identity;
        public SxTransform WorldTransform
        {
            get => worldTransform;
            set => worldTransform = value;
        }
        
        public SxWorld World;
        private List<ISxSceneNode> children = new List<ISxSceneNode>();
        public ISxSceneNode[] Children
        {
            get => children.ToArray();
        }
        
        public SxActorComponent[] Components
        {
            get => components.ToArray();
        }

        public ISxSceneNode Parent { get; set; }
        
        public T AddComponent<T>() where T : SxActorComponent, new()
        {
            var component = new T();
            components.Add(component);
            return component;
        }

        public virtual void Draw(SxRenderContext context, Matrix4x4 accumWorldTransform, SxRenderCommandList renderCommandList)
        {
            foreach (var component in components)
            {
                component.Draw(context, accumWorldTransform, renderCommandList);
            }
        }
        
        public virtual void Tick(SxRenderContext context, float deltaTime)
        {
            foreach (var component in components)
            {
                component.Tick(context, deltaTime);
            }
        }

        public void AddChild(ISxSceneNode child)
        {
            if (!children.Contains(child))
            {
                children.Add(child);
                child.Parent = this;
            }
        }

        public void RemoveChild(ISxSceneNode child)
        {
            children.Remove(child);
        }

        public void RemoveAllChildren()
        {
            children.Clear();
        }
        
        public virtual void Destroy()
        {
            World.DestroyActor(this);
        }

        public Vector3 Position
        {
            get => worldTransform.Positon;
            set => worldTransform.Positon = value;
        }
        
        public Quaternion Rotation
        {
            get => worldTransform.Rotation;
            set => worldTransform.Rotation = value;
        }

        public Vector3 Scale
        {
            get => worldTransform.Scale;
            set => worldTransform.Scale = value;
        }
    }

    public abstract class SxActorComponent
    {
        public virtual void Draw(SxRenderContext context, Matrix4x4 accumWorldTransform, SxRenderCommandList renderCommandList)
        {
            
        }
        
        public virtual void Tick(SxRenderContext context, float deltaTime)
        {
        }

        public virtual void Destroy()
        {
        }
    }

}