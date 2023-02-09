//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Utils
{
    [System.Serializable]
    public class KeyValueDataEntryBase {
        [SerializeField] public string key;
        public virtual object GetValue() { return null; }
        public virtual void SetValue(object value) { }
    }

    [System.Serializable]
    public class KeyValueDataEntryTyped<T> : KeyValueDataEntryBase
    {
        [SerializeField] public T value;
        public override object GetValue() { return value; }
        public override void SetValue(object value) { this.value = (T)value; }
    }

    [System.Serializable] public class KeyValueDataEntryFloat : KeyValueDataEntryTyped<float> { }
    [System.Serializable] public class KeyValueDataEntryInt : KeyValueDataEntryTyped<int> { }
    [System.Serializable] public class KeyValueDataEntryString : KeyValueDataEntryTyped<string> { }
    [System.Serializable] public class KeyValueDataEntryVector3 : KeyValueDataEntryTyped<Vector3> { }
    [System.Serializable] public class KeyValueDataEntryVector2 : KeyValueDataEntryTyped<Vector2> { }

    [System.Serializable]
    public class KeyValueData
    {
        [SerializeField] List<KeyValueDataEntryFloat> dataFloat = new List<KeyValueDataEntryFloat>();
        [SerializeField] List<KeyValueDataEntryInt> dataInt = new List<KeyValueDataEntryInt>();
        [SerializeField] List<KeyValueDataEntryString> dataString = new List<KeyValueDataEntryString>();
        [SerializeField] List<KeyValueDataEntryVector3> dataVector3 = new List<KeyValueDataEntryVector3>();
        [SerializeField] List<KeyValueDataEntryVector2> dataVector2 = new List<KeyValueDataEntryVector2>();

        // Getters
        public bool GetFloat(string key, ref float value) { return GetValue(dataFloat, key, ref value); }
        public bool GetInt(string key, ref int value) { return GetValue(dataInt, key, ref value); }
        public bool GetString(string key, ref string value) { return GetValue(dataString, key, ref value); }
        public bool GetVector3(string key, ref Vector3 value) { return GetValue(dataVector3, key, ref value); }
        public bool GetVector2(string key, ref Vector2 value) { return GetValue(dataVector2, key, ref value); }
        

        // Setters
        public void Set(string key, float value) { SetValue(dataFloat, key, value); }
        public void Set(string key, int value) { SetValue(dataInt, key, value); }
        public void Set(string key, string value) { SetValue(dataString, key, value); }
        public void Set(string key, Vector3 value) { SetValue(dataVector3, key, value); }
        public void Set(string key, Vector2 value) { SetValue(dataVector2, key, value); }


        private void SetValue<T>(List<T> data, string key, object value) where T : KeyValueDataEntryBase, new()
        {
            foreach (var entry in data)
            {
                if (entry is T)
                {
                    if (entry.key == key)
                    {
                        entry.SetValue(value);
                        return;
                    }
                }
            }

            var item = new T();
            item.key = key;
            item.SetValue(value);
            data.Add(item);
        }

        private bool GetValue<TEntry, TValue>(List<TEntry> data, string key, ref TValue value) where TEntry : KeyValueDataEntryBase
        {
            foreach (var entry in data)
            {
                if (entry.key == key)
                {
                    object objValue = entry.GetValue();
                    if (objValue is TValue)
                    {
                        value = (TValue)entry.GetValue();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }
    }
}