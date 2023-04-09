using System;
using UnityEngine;

namespace Riches
{
    [Serializable]
    [CreateAssetMenu(fileName = "newMiningData", menuName = "Data/Mining Data", order = 0)]
    public class RichesObject : ScriptableObject
    {
        public int riches1;
        public int riches2;
        public int riches3;

        public int richesBose1;
        public int richesBose2;
        public int richesBose3;

        public static RichesObject operator +(RichesObject a, RichesObject b)
        {
            var newScriptableObject = CreateInstance<RichesObject>();
            
            newScriptableObject.riches1 = a.riches1 + b.riches1;
            newScriptableObject.riches2 = a.riches2 + b.riches2;
            newScriptableObject.riches3 = a.riches3 + b.riches3;

            newScriptableObject.richesBose1 = a.richesBose1 + b.richesBose1;
            newScriptableObject.richesBose2 = a.richesBose2 + b.richesBose2;
            newScriptableObject.richesBose3 = a.richesBose3 + b.richesBose3;

            return newScriptableObject;
        }

        public void Clear()
        {
            riches1 = 0;
            riches2 = 0;
            riches3 = 0;
            richesBose1 = 0;
            richesBose2 = 0;
            richesBose3 = 0;
        }
    }
}