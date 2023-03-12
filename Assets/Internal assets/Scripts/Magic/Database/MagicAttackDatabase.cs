using System.Collections.Generic;
using Magic.SubMagic;
using Magic.SuperMagic;
using Magic.Type;

namespace Magic.Database
{
    public class MagicAttackDatabase
    {
        private static readonly Dictionary<MagicAttackType, MagicAttack> magicAttackTypeDictionary =
            new Dictionary<MagicAttackType, MagicAttack>()
            {
                { MagicAttackType.Default, new Default() },
                
                { MagicAttackType.AirGust, new AirGust() },
                { MagicAttackType.AirHurricane, new AirHurricane() },
                { MagicAttackType.AirTornado, new AirTornado() },
                
                { MagicAttackType.EarthRockslide, new EarthRockslide() },
                { MagicAttackType.EarthTremor, new EarthTremor() },
                { MagicAttackType.EarthEarthquake, new EarthEarthquake() },
                
                { MagicAttackType.FireIgnition, new FireIgnition() },
                { MagicAttackType.FireInferno, new FireInferno() },
                { MagicAttackType.FireMeteor, new FireMeteor() },
                
                { MagicAttackType.IceFrostbite, new IceFrostbite() },
                { MagicAttackType.IceGlacialChill, new IceGlacialChill() },
                { MagicAttackType.IceAbsoluteZero, new IceAbsoluteZero() },
                
                { MagicAttackType.LightningStaticShock, new LightningStaticShock() },
                { MagicAttackType.LightningThunderbolt, new LightningThunderbolt() },
                { MagicAttackType.LightningLightningStorm, new LightningLightningStorm() },
                
                { MagicAttackType.WaterTorrent, new WaterTorrent() },
                { MagicAttackType.WaterTsunami, new WaterTsunami() },
                { MagicAttackType.WaterCyclone, new WaterCyclone() },
            };

        public static MagicAttack GetMagicAttack(MagicAttackType magicAttackType)
        {
            return magicAttackTypeDictionary[magicAttackType];
        }
    }
}