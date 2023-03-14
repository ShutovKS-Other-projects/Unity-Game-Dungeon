//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Utils
{
    public class SmoothValue
    {
        private float targetValue = 0;
        private float currentValue = 0;
        private float t = 0;

        public float TimeToArrive = 0.1f;
        
        public float Value
        {
            get => currentValue;
            set
            {
                targetValue = value;
                t = 0;
            }
        }

        public float TargetValue
        {
            get => targetValue;
        }

        public SmoothValue(float value)
        {
            Set(value);
        }

        public void Set(float value)
        {
            currentValue = value;
            targetValue = value;
            t = 0;
        }
        
        public void Update(float deltaTime)
        {
            if (t < 1)
            {
                t += deltaTime / TimeToArrive;
                t = Mathf.Clamp01(t);

                currentValue = Mathf.Lerp(currentValue, targetValue, t);
            }
        }
    }
}