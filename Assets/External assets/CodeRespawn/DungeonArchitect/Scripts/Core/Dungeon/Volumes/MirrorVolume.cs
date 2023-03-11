//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
namespace DungeonArchitect
{
    [System.Serializable]
    public enum MirrorVolumeDirection
    {
        AxisX,
        AxisZ,
        AxisXZ
    }
    
    public class MirrorVolume : Volume
    {
        public MirrorVolumeDirection direction = MirrorVolumeDirection.AxisX;
    }
}