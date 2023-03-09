using System;
using Magic;
using Player.FiniteStateMachine.SuperState;
using Skill;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player.FiniteStateMachine.SubState
{
    public class PlayerMagicAttackState : PlayerAbilityState
    {
        private GameObject _magicAttack;
        private PlayerSkills _playerSkills;

        public PlayerMagicAttackState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _playerSkills = StateController.GetComponent<PlayerSkills>();

            _magicAttack = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            var main = UnityEngine.Camera.main;
            var magicTransform = _magicAttack.transform;
            var magicPosition = main!.transform.position;
            magicPosition += main!.transform.forward * 1;

            magicTransform.position = magicPosition;
            magicTransform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            magicTransform.rotation = main.transform.rotation;

            _magicAttack.AddComponent<SphereCollider>();
            _magicAttack.GetComponent<SphereCollider>().isTrigger = true;

            _magicAttack.AddComponent<Rigidbody>();
            _magicAttack.GetComponent<Rigidbody>().AddForce(main.transform.forward * 1500);
            _magicAttack.GetComponent<Rigidbody>().useGravity = false;
            _magicAttack.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

            _magicAttack.GetComponent<Renderer>().material.color = PlayerStatistic.MagicAttackType switch
            {
                SkillMagicType.Fire => Color.red,
                SkillMagicType.Air => Color.white,
                SkillMagicType.Water => Color.blue,
                SkillMagicType.Earth => Color.green,
                SkillMagicType.Light => Color.yellow,
                SkillMagicType.Dark => Color.black,
                SkillMagicType.Default => Color.magenta,
                _ => _magicAttack.GetComponent<Renderer>().material.color
            };
            _magicAttack.tag = "ObjectDamaging";

            _magicAttack.AddComponent<MagicAttack>();
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