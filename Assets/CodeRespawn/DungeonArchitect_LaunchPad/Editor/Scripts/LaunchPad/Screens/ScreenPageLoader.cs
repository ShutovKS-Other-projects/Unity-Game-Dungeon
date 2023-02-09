//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

namespace DungeonArchitect.Editors.LaunchPad
{
    public class ScreenPageLoader
    {
        static T LoadAsset<T>(string basePath, string path) where T : Object
        {
            var fullPath = "LaunchPad/" + basePath + path;
            return Resources.Load<T>(fullPath);
        }

        static TextAsset LoadPageAsset(string path)
        {
            return LoadAsset<TextAsset>("pages/", path);
        }

        public static Texture2D LoadImageAsset(string path)
        {
            return LoadAsset<Texture2D>("images/", path);
        }


        public static ScreenPage LoadPage(string path)
        {
            var textAsset = LoadPageAsset(path);
            if (textAsset == null)
            {
                return null;
            }

            var json = textAsset.text;
            var layoutType = GetPageType(json);
            var page = ScreenPageFactory.Create(layoutType);
            if (page != null)
            {
                page.Load(json);
            }

            return page;
        }

        static ScreenPageType GetPageType(string json)
        {
            var data = JsonUtility.FromJson<ScreenPageJsonDataBase>(json);
            if (data == null)
            {
                return ScreenPageType.Empty;
            }

            try
            {
                ScreenPageType result = (ScreenPageType)System.Enum.Parse(typeof(ScreenPageType), data.layout);
                return result;
            }
            catch { }

            return ScreenPageType.Empty;
        }
    }
}
