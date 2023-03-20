using Old.Magic.SubMagic;
using Old.Magic.Type;
using Old.Player.FiniteStateMachine;
using Old.Skill.Magic;
using Old.Skill.SkillTree;
using UnityEngine;

namespace Old.Player
{
    public class PlayerSkillMagicAttack : MonoBehaviour
    {
        private SkillMagic _skillMagic;
        private UISkillTreeMagic _uiSkillTreeMagic;
        private PlayerStateController _playerStateController;

        private void Awake()
        {
            Debug.Log("PlayerSkillMagicAttack Awake");

            _skillMagic = new SkillMagic();
            _skillMagic.OnSkillSwitched += PlayerOnSwitched!;

            _uiSkillTreeMagic = FindObjectOfType<UISkillTreeMagic>();
            _uiSkillTreeMagic.SetMagicSkills(_skillMagic);

            _playerStateController = GetComponent<PlayerStateController>();
        }

        public void AddSkillPont() => _skillMagic.AddSkillPoint();

        private void PlayerOnSwitched(object sender, MagicType magicType)
        {
            switch (magicType)
            {
                case MagicType.Default:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Default>());
                    break;

                case MagicType.Buff1:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Buff1>());
                    break;
                case MagicType.Buff2:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Buff2>());
                    break;
                case MagicType.Buff3:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Buff3>());
                    break;
                case MagicType.Buff4:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Buff4>());
                    break;
                case MagicType.Buff5:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Buff5>());
                    break;
                case MagicType.Buff6:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Buff6>());
                    break;

                case MagicType.Debuff1:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Debuff1>());
                    break;
                case MagicType.Debuff2:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Debuff2>());
                    break;
                case MagicType.Debuff3:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Debuff3>());
                    break;
                case MagicType.Debuff4:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Debuff4>());
                    break;
                case MagicType.Debuff5:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Debuff5>());
                    break;
                case MagicType.Debuff6:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Debuff6>());
                    break;

                case MagicType.Fire1:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Fire1>());
                    break;
                case MagicType.Fire2:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Fire2>());
                    break;
                case MagicType.Fire3:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Fire3>());
                    break;
                case MagicType.Fire4:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Fire4>());
                    break;
                case MagicType.Fire5:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Fire5>());
                    break;
                case MagicType.Fire6:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Fire6>());
                    break;

                case MagicType.Ice1:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Ice1>());
                    break;
                case MagicType.Ice2:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Ice2>());
                    break;
                case MagicType.Ice3:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Ice3>());
                    break;
                case MagicType.Ice4:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Ice4>());
                    break;
                case MagicType.Ice5:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Ice5>());
                    break;
                case MagicType.Ice6:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Ice6>());
                    break;

                case MagicType.Lightning1:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Lightning1>());
                    break;
                case MagicType.Lightning2:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Lightning2>());
                    break;
                case MagicType.Lightning3:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Lightning3>());
                    break;
                case MagicType.Lightning4:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Lightning4>());
                    break;
                case MagicType.Lightning5:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Lightning5>());
                    break;
                case MagicType.Lightning6:
                    _playerStateController.RegisterDelegateMagicAttackDelegate(() =>
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Lightning6>());
                    break;

                case MagicType.None:
                    Debug.Log("No magic");
                    break;

                default:
                    Debug.Log("No magic attack type selected");
                    break;
            }
        }
    }
}