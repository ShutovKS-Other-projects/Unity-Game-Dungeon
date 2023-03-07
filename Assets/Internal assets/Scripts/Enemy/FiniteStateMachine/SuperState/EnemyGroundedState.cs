using UnityEngine;
namespace Enemy.FiniteStateMachine.SuperState
{
    public class EnemyGroundedState : EnemyState
    {
        protected float PlayerDistance;
        protected bool IsAttack;
        protected bool IsVisiblePlayer;

        public EnemyGroundedState(EnemyStateController stateController, EnemyStateMachine stateMachine, EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic, animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            PlayerDistance = StateController.CheckPlayerDistance();
            IsVisiblePlayer = StateController.CheckIfPlayer();

            if (EnemyStatistic.IsDead)
            {
                StateMachine.ChangeState(StateController.DeathState);
            }
            else if (!IsVisiblePlayer)
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
