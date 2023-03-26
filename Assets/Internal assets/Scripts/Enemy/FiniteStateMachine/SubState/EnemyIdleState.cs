using UnityEngine;

namespace Enemy.FiniteStateMachine.SubState
{
    public class EnemyIdleState : SuperState.EnemyGroundedState
    {
        public EnemyIdleState(EnemyStateController stateController, EnemyStateMachine stateMachine,
            EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic,
            animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Idle");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (EnemyStatistic.isVisiblePlayer)
                StateMachine.ChangeState(StateController.MoveState);
        }
    }
}