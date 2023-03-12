using Magic.SuperMagic;
using Magic.Type;
using UnityEngine;

namespace Magic.SubMagic
{
    public class FireInferno: MagicAttack
    {
        public override MagicAttackType MagicAttackType { get; set; }
        protected override Color ColorSphere { get; set; }
        protected override float ForceFlight { get; set; }
        protected override float Radius { get; set; }
        
        protected override void CreateMagicModel()
        {
            MagicAttackType = MagicAttackType.FireInferno;
            ColorSphere = Color.red;
            ForceFlight = 1000;
            Radius = 0.5f;
            base.CreateMagicModel();
        }
    }
}