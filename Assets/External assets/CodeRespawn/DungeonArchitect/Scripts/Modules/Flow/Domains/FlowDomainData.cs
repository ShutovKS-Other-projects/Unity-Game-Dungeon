//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;

namespace DungeonArchitect.Flow.Domains
{
    public interface IFlowDomainData
    {
        IFlowDomainData Clone();
    }
    
    [System.Serializable]
    public class FlowDomainDataRegistry
    {
        private Dictionary<System.Type, IFlowDomainData> crossDomainData = new Dictionary<System.Type, IFlowDomainData>();
        
        public T Get<T>() where T : IFlowDomainData, new()
        {
            if (crossDomainData.ContainsKey(typeof(T)))
            {
                return (T)crossDomainData[typeof(T)];
            }

            T data = new T();
            crossDomainData.Add(typeof(T), data);
            return data;
        }

        public void Set<T>(T data) where T : IFlowDomainData, new()
        {
            crossDomainData[typeof(T)] = data;
        }
        
        public FlowDomainDataRegistry Clone()
        {
            var clone = new FlowDomainDataRegistry();
            foreach (var entry in crossDomainData)
            {
                var type = entry.Key;
                var clonedData = entry.Value.Clone();
                clone._Internal_AddDomainData(type, clonedData as IFlowDomainData);
            }

            return clone;
        }
        
        private void _Internal_AddDomainData(System.Type type, IFlowDomainData data)
        {
            if (crossDomainData.ContainsKey(type))
            {
                crossDomainData.Remove(type);
            }
            
            crossDomainData.Add(type, data);
        }
    }
}