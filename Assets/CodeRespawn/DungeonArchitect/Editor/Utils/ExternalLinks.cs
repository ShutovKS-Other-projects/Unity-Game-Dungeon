//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Editors
{
    public class ExternalLinks
    {
        public static readonly string DiscordInvite = "https://discord.gg/N3rHt4h";
        public static readonly string Documentation = "https://docs.dungeonarchitect.dev/unity";
        public static readonly string ProductPage = "https://u3d.as/nAL";
        
        public static void LaunchUrl(string url)
        {
            Application.OpenURL(url);
        }
    }
}
