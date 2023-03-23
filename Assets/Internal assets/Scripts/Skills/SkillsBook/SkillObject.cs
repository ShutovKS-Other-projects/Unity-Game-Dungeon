using System;
using UnityEngine;

namespace Skills.SkillsBook
{
    [CreateAssetMenu(fileName = "new SkillBook", menuName = "Skill/SkillBook", order = 0)]
    public class SkillObject : ScriptableObject
    {
        public int levelMax;
        public Sprite iconSprite;
        public string nameSkill;
        public SkillType skillType;
        public int Buff(int level) => skillBuffs[level].buff;
        public int Price(int level) => skillBuffs[level].price;
        public SkillBuff[] skillBuffs;

        [Serializable]
        public struct SkillBuff
        {
            public int buff;
            public int price;
        }
    }
}