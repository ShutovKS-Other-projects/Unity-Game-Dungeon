using System;
using System.Collections.Generic;
using UnityEngine;
namespace Other
{
    [System.Serializable]
    public class ModifiableFloat
    {
        [SerializeField] private float modifiedValue = 0;
        public float ModifiedValue
        {
            get => modifiedValue;
            private set => modifiedValue = value;
        }

        public ModifiableFloat() => modifiedValue = 0;

        private event Action ValueModified;
        
        public List<IModifiers> Modifiers = new List<IModifiers>();

        public ModifiableFloat(Action method = null)
        {
            ModifiedValue = 0;
            if (method != null)
                ValueModified += method;
        }

        public void RegisterModifier(Action  method)
        {
            ValueModified += method;
        }

        public void UnregisterModifier(Action  method)
        {
            ValueModified -= method;
        }

        public void UpdateModifiedValue()
        {
            float valueAdd = 0;
            foreach (var modifier in Modifiers)
            {
                modifier.AddValue(ref valueAdd);
            }
            ModifiedValue = valueAdd;
            ValueModified?.Invoke();
        }

        public void AddModifier(IModifiers modifier)
        {
            Modifiers.Add(modifier);
            UpdateModifiedValue();
        }

        public void RemoveModifier(IModifiers modifier)
        {
            Modifiers.Remove(modifier);
            UpdateModifiedValue();
        }

    }
}