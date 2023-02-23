using Enemy.FiniteStateMachine.SuperState;
using Player.FiniteStateMachine;
using UnityEngine;
namespace Enemy.FiniteStateMachine.SubState
{
    public class EnemyDamageState : EnemyAbilityState
    {
        public EnemyDamageState(EnemyStateController stateController, EnemyStateMachine stateMachine, EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            var playerStateController = GameObject.FindWithTag("Player").GetComponent<PlayerStateController>();
            EnemyStatistic.health -= playerStateController.StrengthAttackFloat!();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsAnimationFinished)
                return;

            if (EnemyStatistic.isDead)
            {
                StateMachine.ChangeState(StateController.DeathState);
            }
            else
            {
                StateMachine.ChangeState(StateController.IdleState);
            }
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            IsAnimationFinished = true;
        }
    }
}
