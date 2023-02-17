using System.Collections.Generic;
using UnityEngine;
namespace Internal_assets.Scripts.Menu
{
    public class Menu : MonoBehaviour
    {
        System.Random random = new System.Random();
        [SerializeField] private GameObject _citationText;
        private float _scaleCitationText = 1f;
        private bool _isScaleCitationText = false;
        void Start()
        {
            _citationText.GetComponent<UnityEngine.UI.Text>().text = ReturnCitationText();
        }

        void FixedUpdate()
        {
            ChangeScaleCitationText();
        }

        private void ChangeScaleCitationText()
        {
            if (_scaleCitationText >= 1.2f || _scaleCitationText <= 0.95f)
            {
                _isScaleCitationText = !_isScaleCitationText;
            }
            if (_isScaleCitationText)
            {
                _scaleCitationText += 0.15f * Time.deltaTime;
            }
            else if (!_isScaleCitationText)
            {
                _scaleCitationText -= 0.15f * Time.deltaTime;
            }

            _citationText.GetComponent<UnityEngine.UI.Text>().rectTransform.localScale = new Vector3(_scaleCitationText, _scaleCitationText, 1f);
        }
        private string ReturnCitationText()
        {
            List<string> citations = new List<string>()
            {
                "“Нет лабиринтов, из которых нельзя было бы выбраться, но поиск выхода требует решимости и настойчивости”",
                "“Нет лабиринтов, из которых невозможно выбраться”",
                "“Лабиринт — это не место страха, а путь к самопознанию!”",
                "“Жизнь — это лабиринт, и каждый выбор, который мы делаем, ведет нас по другому пути”",
                "“Лабиринт — это головоломка, которую нужно решить, а не барьер, который нужно преодолеть”",
                "“Лабиринт — это зеркало разума, раскрывающее наши самые сокровенные страхи и желания”",
                "“Выход из лабиринта не всегда самый легкий, но всегда самый полезный”",
                "“Настоящая проверка характера заключается не в том, как человек входит в лабиринт, а в том, как он выходит из него”",
                "“Путешествие по лабиринту может быть трудным, но награда в конце стоит усилий”",
                "“Лабиринт — это не проклятие, а вызов, который нужно преодолеть”",
                "“Единственный способ по-настоящему проиграть лабиринту — это сдаться”",
                "“Лабиринт — это не тюрьма, а возможность для роста и самопознания”",
                "“Лабиринт — это не место, где нужно бежать, а место, где нужно размышлять”",
                "“Покорение лабиринта заключается не в том, чтобы никогда не заблудиться, а в том, чтобы иметь смелость продолжать двигаться вперед”",
            };

            return citations[random.Next(0, citations.Count)];
        }
    }
}