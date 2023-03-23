using System.Collections;
using UnityEngine;

namespace Skills.SkillsBook
{
    public class Skill : MonoBehaviour
    {
        [SerializeField] private SkillObject skillObject;
        private int _level;

        public Sprite IconSprite => skillObject.iconSprite;
        public string NameSkill => skillObject.nameSkill;
        public int BuffSkill => _level == 0 ? 0 : skillObject.Buff(_level - 1);
        public int Price => skillObject.Price(_level);
        public int Level => _level;
        public int LevelMax => skillObject.levelMax;


        public void Buy()
        {
            if (_level >= skillObject.levelMax || SkillTree.Instance.skillPoints < Price) return;

            Debug.Log($"Skill {NameSkill} level up to {_level} price: {Price}");
            SkillTree.Instance.skillPoints -= Price;
            _level++;
            StartCoroutine(OnSkillUnlocked());
        }

        private static IEnumerator OnSkillUnlocked()
        {
            yield return null;
            SkillTree.Instance.OnSkillUnlocked();
        }
    }
}