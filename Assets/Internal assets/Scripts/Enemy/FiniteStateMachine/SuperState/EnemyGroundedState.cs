using UnityEngine;
namespace Enemy.FiniteStateMachine.SuperState
{
    public class EnemyGroundedState : EnemyState
    {
        protected float playerDistance;
        protected bool isAttack;
        protected bool isVisiblePlayer;

        public EnemyGroundedState(EnemyStateController stateController, EnemyStateMachine stateMachine, EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic, animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            playerDistance = StateController.CheckPlayerDistance();
            isVisiblePlayer = StateController.CheckIfPlayer();

            if (enemyStatistic.isDead)
            {
                StateMachine.ChangeState(StateController.DeathState);
            }
            else if (!isVisiblePlayer)
            {
                StateMachine.ChangeState(StateController.IdleState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            StateController.LookAtPlayer();
        }

        public override void TriggerEnter(Collider other)
        {   
            base.TriggerEnter(other);
        
            if (other.CompareTag($"ObjectDamaging"))
            {
                StateMachine.ChangeState(StateController.DamageState);
            }
        }

    }
}
