//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.Flow.Domains;
using UnityEngine;

namespace DungeonArchitect.Flow.Exec
{
    public enum FlowTaskExecutionResult
    {
        Success,
        FailRetry,
        FailHalt
    }

    public enum FlowTaskExecutionFailureReason
    {
        Unknown,
        Timeout
    }

    public class FlowTaskExecContext
    {
        public System.Random Random;
        public FlowDomainExtensions DomainExtensions { get; set; }
    }
    
    public class FlowTaskExecInput
    {
        public FlowTaskExecOutput[] IncomingTaskOutputs;

        public FlowExecTaskState CloneInputState()
        {
            if (IncomingTaskOutputs.Length == 0) return null;
            if (IncomingTaskOutputs[0].State == null) return null;
            return IncomingTaskOutputs[0].State.Clone();
        }
    }

    public class FlowTaskExecOutput
    {
        public FlowExecTaskState State = new FlowExecTaskState();
        public FlowTaskExecutionResult ExecutionResult = FlowTaskExecutionResult.FailHalt;
        public FlowTaskExecutionFailureReason FailureReason = FlowTaskExecutionFailureReason.Unknown;
        public string ErrorMessage = "";
    }
    
    public abstract class FlowExecTask : ScriptableObject
    {
        public abstract FlowTaskExecOutput Execute(FlowTaskExecContext context, FlowTaskExecInput input);
        public string description = "";
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class FlowExecNodeInfoAttribute : System.Attribute
    {
        public string Title { get; private set; }
        public string MenuPrefix { get; private set; }
        public float Weight { get; private set; }

        public FlowExecNodeInfoAttribute(string title)
            : this(title, "", 0)
        {
        }

        public FlowExecNodeInfoAttribute(string title, string menuPrefix)
            : this(title, menuPrefix, 0)
        {
        }

        public FlowExecNodeInfoAttribute(string title, string menuPrefix, float weight)
        {
            this.Title = title;
            this.MenuPrefix = menuPrefix;
            this.Weight = weight;
        }

        public static FlowExecNodeInfoAttribute GetHandlerAttribute(System.Type type)
        {
            if (type == null) return null;
            return type.GetCustomAttributes(typeof(FlowExecNodeInfoAttribute), true).FirstOrDefault() as FlowExecNodeInfoAttribute;
        }
    }

    public class FlowExecNodeOutputRegistry
    {
        public void Clear()
        {
            stateByNodeId.Clear();
        }

        public void Register(string nodeId, FlowTaskExecOutput state)
        {
            if (state != null)
            {
                stateByNodeId[nodeId] = state;
            }
        }

        public FlowTaskExecOutput Get(string nodeId)
        {
            if (stateByNodeId.ContainsKey(nodeId))
            {
                return stateByNodeId[nodeId];
            }
            return null;
        }

        private Dictionary<string, FlowTaskExecOutput> stateByNodeId = new Dictionary<string, FlowTaskExecOutput>();
    }

    public class FlowExecTaskState
    {
        public ICloneable GetState(System.Type type)
        {
            return states.ContainsKey(type) ? states[type] : null;
        }
        
        public T GetState<T>() where T : ICloneable
        {
            if (states.ContainsKey(typeof(T)))
            {
                return (T) states[typeof(T)];
            }

            // Not found, return default value (null)
            return default;
        }

        public void SetState(System.Type type, ICloneable state)
        {
            if (state != null)
            {
                Debug.Assert(state.GetType() == type || state.GetType().IsSubclassOf(type));
                states[type] = state;
            }
        }

        public FlowExecTaskState Clone()
        {
            var clone = new FlowExecTaskState();
            foreach (var entry in states)
            {
                var type = entry.Key;
                var obj = entry.Value;
                var clonedObj = obj.Clone() as ICloneable;
                clone.SetState(type, clonedObj);
            }

            return clone;
        }

        public System.Type[] GetRegisteredStateTypes()
        {
            return states.Keys.ToArray();
        }
        
        protected Dictionary<System.Type, ICloneable> states = new Dictionary<Type, ICloneable>();
    }
}
