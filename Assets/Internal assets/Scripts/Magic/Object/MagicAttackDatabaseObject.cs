using UnityEngine;

namespace Magic.Object
{
    [CreateAssetMenu(fileName = "New MagicAttackDatabase", menuName = "Magic/Database/Attack", order = 0)]
    public class MagicAttackDatabaseObject : ScriptableObject
    {
        public MagicAttackObject[] magicAttackObjects;

        [ContextMenu("Sort")]
        public void Sort()
        {
            for (var i = 0; i < magicAttackObjects.Length; i++)
            for (var j = 0; j < magicAttackObjects.Length - 1; j++)
                if (magicAttackObjects[i].MagicAttackType < magicAttackObjects[j].MagicAttackType)
                    (magicAttackObjects[i], magicAttackObjects[j]) = (magicAttackObjects[j], magicAttackObjects[i]);
        }
    }
}