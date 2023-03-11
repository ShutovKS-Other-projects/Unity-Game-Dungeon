//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
namespace DungeonArchitect.Extensions
{
    public static class DungeonExtensions
    {
        public static bool IsValid(this System.Guid guid)
        {
            return guid != System.Guid.Empty;
        }
    }
}