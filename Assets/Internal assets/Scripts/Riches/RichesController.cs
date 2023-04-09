using System;
using Scene;
using UnityEngine;

namespace Riches
{
    public class RichesController : MonoBehaviour
    {
        [NonSerialized] public static RichesObject richesObjectDefault;
        [NonSerialized] public static RichesObject richesObjectTime;

        public static event Action RichesChanged;

        #region Unity Methods

        private void Awake()
        {
            richesObjectDefault = Resources.Load<RichesObject>($"ScriptableObject/Mining/MiningObjectDefault");
            richesObjectTime = Resources.Load<RichesObject>($"ScriptableObject/Mining/MiningObjectTime");

            OnRichesChanged();
            SceneController.OnNewSceneLoaded += LoadScene;
        }

        #endregion

        private static void LoadScene()
        {
            richesObjectDefault += richesObjectTime;
            richesObjectTime.Clear();
        }

        public static void OnRichesChanged()
        {
            RichesChanged?.Invoke();
        }
    }
}