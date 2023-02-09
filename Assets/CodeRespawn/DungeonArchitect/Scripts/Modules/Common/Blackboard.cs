//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Utils
{
    /// <summary>
    /// A blackboard holds global data that can be shared across multiple scripts
    /// </summary>
    public class Blackboard
    {
        private BlackboardDatabase<int> intEntries = new BlackboardDatabase<int>(0);
        public BlackboardDatabase<int> IntEntries
        {
            get { return intEntries; }
        }
        private BlackboardDatabase<float> floatEntries = new BlackboardDatabase<float>(0.0f);
        public BlackboardDatabase<float> FloatEntries
        {
            get { return floatEntries; }
        }
        private BlackboardDatabase<string> stringEntries = new BlackboardDatabase<string>("");
        public BlackboardDatabase<string> StringEntries
        {
            get { return stringEntries; }
        }
        private BlackboardDatabase<Vector3> vectorEntries = new BlackboardDatabase<Vector3>(Vector3.zero);
        public BlackboardDatabase<Vector3> VectorEntries
        {
            get { return vectorEntries; }
        }
        private BlackboardDatabase<IntVector> intVectorEntries = new BlackboardDatabase<IntVector>(IntVector.Zero);
        public BlackboardDatabase<IntVector> IntVectorEntries
        {
            get { return intVectorEntries; }
        }
    }

    
    public class BlackboardDatabase<T>
    {
        T defaultValue;
        Dictionary<string, T> database = new Dictionary<string, T>();

        public BlackboardDatabase(T defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public void SetValue(string key, T value) {
            database[key] = value;
        }

        public T GetValue(string key)
        {
            if (!database.ContainsKey(key))
            {
                return defaultValue;
            }
            return database[key];
        }
        
    }

}