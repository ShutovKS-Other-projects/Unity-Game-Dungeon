using System.Collections.Generic;
using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Other
{
    public delegate void ModifiedEvent();
    [System.Serializable]
    public class ModifiableInt
    {
        [SerializeField] private int baseValue;
        public int BaseValue
        {
            get { return baseValue; }
            set
            {
                baseValue = value;
                //UpdateModifiedValue();
            }
        }

        [SerializeField] private int modifiedValue;
        public int ModifiedValue
        {
            get { return modifiedValue; }
            private set
            {
                modifiedValue = value;
            }
        }

        public List<IModifiers> modifiers = new List<IModifiers>();

        public event ModifiedEvent ValueModified;
        public ModifiableInt(ModifiedEvent method = null)
        {
            modifiedValue = baseValue;
            if (method != null)
            {
                ValueModified += method;
            }
        }

        public void RegsiterModifier(ModifiedEvent method)
        {
            ValueModified += method;
        }

        public void UnregisterModifier(ModifiedEvent method)
        {
            ValueModified -= method;
        }

        public void UpdateModifiedValue()
        {
            var valueAdd = 0;
            for(var i = 0; i < modifiers.Count; i++)
            {
                modifiers[i].AddValue(ref valueAdd);
            }
            ModifiedValue = baseValue + valueAdd;
            if(ValueModified != null)
            {
                ValueModified.Invoke();
            }
        }

        public void AddModifier(IModifiers modifier)
        {
            modifiers.Add(modifier);
            UpdateModifiedValue();
        }

        public void RemoveModifier(IModifiers modifier)
        {
            modifiers.Remove(modifier);
            UpdateModifiedValue();
        }

    }
}