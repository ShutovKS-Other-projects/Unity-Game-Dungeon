using System;
using Mining;
using UnityEngine;

namespace Manager
{
    public class ManagerRiches : MonoBehaviour
    {
        #region Singleton

        public static ManagerRiches Instance { get; private set; }
        public RichesObject richesObjectDefault;
        public RichesObject richesObjectTime;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

            richesObjectDefault = Resources.Load<RichesObject>($"ScriptableObject/Mining/MiningObjectDefault");
            richesObjectTime = Resources.Load<RichesObject>($"ScriptableObject/Mining/MiningObjectTime");

            OnRichesChanged();
            ManagerScene.Instance.OnNewSceneLoaded += LoadScene;
        }

        #endregion

        private void LoadScene()
        {
            richesObjectDefault += richesObjectTime;
            richesObjectTime.Clear();
        }

        public event Action RichesChanged;

        public void OnRichesChanged()
        {
            RichesChanged?.Invoke();
        }
    }
}