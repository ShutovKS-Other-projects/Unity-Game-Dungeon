using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Skills.SkillsBook
{
    public class UISkill : MonoBehaviour
    {
        [SerializeField] private Skill skill;

        public Image iconImage;
        public TMP_Text nameText;
        public TMP_Text buffText;
        public TMP_Text levelText;

        private void Start()
        {
            skill = GetComponent<Skill>();
            iconImage = transform.GetChild(0).GetComponent<Image>();
            nameText = transform.GetChild(1).GetComponent<TMP_Text>();
            buffText = transform.GetChild(2).GetComponent<TMP_Text>();
            levelText = transform.GetChild(3).GetComponent<TMP_Text>();
            transform.GetChild(4).GetComponent<Button>().onClick.AddListener(skill.Buy);
            ManagerSkillBook.Instance.SkillUnlocked += UpdateUI;
            UpdateUI();
        }

        private void UpdateUI()
        {
            iconImage.sprite = skill.IconSprite;
            nameText.text = $"{skill.NameSkill}";
            buffText.text = $"+{skill.BuffSkill}";
            levelText.text = $"{skill.Level}/{skill.LevelMax}";

            GetComponent<Image>().color = skill.Level >= skill.LevelMax
                ? Color.yellow
                : ManagerSkillBook.Instance.skillPoints > skill.Price
                    ? Color.green
                    : Color.white;
        }
    }
}