//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;

namespace DungeonArchitect.Flow.Domains
{
    public interface IFlowDomainExtension
    {
    }

    public class FlowDomainExtensions
    {
        private Dictionary<System.Type, IFlowDomainExtension> extensions = new Dictionary<System.Type, IFlowDomainExtension>();
        
        public T GetExtension<T>() where T : IFlowDomainExtension, new()
        {
            if (extensions.ContainsKey(typeof(T)))
            {
                return (T)extensions[typeof(T)];
            }

            T data = new T();
            extensions.Add(typeof(T), data);
            return data;
        }
    }
}