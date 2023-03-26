﻿using Old.Magic.SuperMagic;
using Old.Magic.Type;
using UnityEngine;

namespace Old.Magic.SubMagic
{
    public class Ice6 : MagicAttack
    {
        public override MagicType MagicType { get; set; }
        protected override Color ColorSphere { get; set; }
        protected override float ForceFlight { get; set; }
        protected override float Radius { get; set; }

        protected override void CreateMagicModel()
        {
            MagicType = MagicType.Ice6;
            ColorSphere = Color.cyan;
            ForceFlight = 1000;
            Radius = 0.5f;
            base.CreateMagicModel();
        }
    }
}