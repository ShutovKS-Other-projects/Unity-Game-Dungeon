using System.Collections;
using Manager;
using Mining;
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
        public SkillType SkillType => skillObject.skillType;

        public void Buy()
        {
            if (_level >= skillObject.levelMax || ManagerRiches.Instance.richesObjectDefault.riches1 < Price) return;

            Debug.Log($"Skill {NameSkill} level up to {_level} price: {Price}");
            ManagerRiches.Instance.richesObjectDefault.riches1 -= Price;
            _level++;
            StartCoroutine(OnSkillUnlocked());
        }

        public (SkillType, int) GetSkillTypeAndBuff()
        {
            return (SkillType, BuffSkill);
        }

        private static IEnumerator OnSkillUnlocked()
        {
            yield return null;
            SkillBookController.Instance.OnSkillUnlocked();
            ManagerRiches.Instance.OnRichesChanged();
        }
    }
}