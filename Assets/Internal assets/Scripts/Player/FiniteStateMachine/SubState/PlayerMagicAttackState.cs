using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Magic;
using Magic.SubMagic;
using Magic.SuperMagic;
using Magic.Type;
using Player.FiniteStateMachine.SuperState;
using Skill;
using Skill.Enum;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player.FiniteStateMachine.SubState
{
    public class PlayerMagicAttackState : PlayerAbilityState
    {
        public PlayerMagicAttackState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            switch (PlayerStatistic.MagicAttackType)
            {
                case MagicAttackType.Default:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Default>();
                    break;
                case MagicAttackType.AirGust:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<AirGust>();
                    break;
                case MagicAttackType.AirHurricane:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<AirHurricane>();
                    break;
                case MagicAttackType.AirTornado:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<AirTornado>();
                    break;
                case MagicAttackType.EarthRockslide:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<EarthRockslide>();
                    break;
                case MagicAttackType.EarthTremor:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<EarthTremor>();
                    break;
                case MagicAttackType.EarthEarthquake:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<EarthEarthquake>();
                    break;
                case MagicAttackType.FireIgnition:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<FireIgnition>();
                    break;
                case MagicAttackType.FireInferno:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<FireInferno>();
                    break;
                case MagicAttackType.FireMeteor:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<FireMeteor>();
                    break;
                case MagicAttackType.IceFrostbite:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<IceFrostbite>();
                    break;
                case MagicAttackType.IceGlacialChill:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<IceGlacialChill>();
                    break;
                case MagicAttackType.IceAbsoluteZero:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<IceAbsoluteZero>();
                    break;
                case MagicAttackType.LightningStaticShock:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<LightningStaticShock>();
                    break;
                case MagicAttackType.LightningThunderbolt:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<LightningThunderbolt>();
                    break;
                case MagicAttackType.LightningLightningStorm:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<LightningLightningStorm>();
                    break;
                case MagicAttackType.WaterTorrent:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<WaterTorrent>();
                    break;
                case MagicAttackType.WaterTsunami:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<WaterTsunami>();
                    break;
                case MagicAttackType.WaterCyclone:
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<WaterCyclone>();
                    break;
                case MagicAttackType.None:
                    Debug.Log("No magic");
                    break;
                default:
                    Debug.Log("No magic attack type selected");
                    break; 
            }

            if (Random.Range(0, 101) < PlayerStatistic.CriticalChance)
                StateController.RegisterDelegateStrengthAttackFloat(CriticalMagicAttack);
            else
                StateController.RegisterDelegateStrengthAttackFloat(MagicAttack);
        }

        public override void Exit()
        {
            base.Exit();

            StateController.RegisterDelegateStrengthAttackFloat(AttackZero);
        }


        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();
            IsAbilityDone = true;
        }

        private float CriticalMagicAttack() =>
            PlayerStatistic.MagicAttackDamage * (1 + PlayerStatistic.CriticalDamage / 100);

        private float MagicAttack() => PlayerStatistic.MagicAttackDamage;
        private static float AttackZero() => 0;
    }
}