using Magic.Type;
using UnityEngine;

namespace Magic.Object
{
    [CreateAssetMenu(fileName = "New MagicAttack", menuName = "Magic/Magic/Attack", order = 0)]
    public class MagicAttackObject : ScriptableObject
    {
        [SerializeField] private MagicAttackType magicAttackType;
        [SerializeField] private Color colorSphere;
        [SerializeField] private float forceFlight;
        [SerializeField] private float radius;

        public float ForceFlight => forceFlight;
        public float Radius => radius;
        public MagicAttackType MagicAttackType => magicAttackType;
        public Color ColorSphere => colorSphere;
    }
}