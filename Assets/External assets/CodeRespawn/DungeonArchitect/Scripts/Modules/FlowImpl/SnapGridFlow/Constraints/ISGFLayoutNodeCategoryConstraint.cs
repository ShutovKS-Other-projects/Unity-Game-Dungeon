//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
namespace DungeonArchitect
{
    public interface ISGFLayoutNodeCategoryConstraint
    {
        string[] GetModuleCategoriesAtNode(int currentPathPosition, int pathLength);
    }
}