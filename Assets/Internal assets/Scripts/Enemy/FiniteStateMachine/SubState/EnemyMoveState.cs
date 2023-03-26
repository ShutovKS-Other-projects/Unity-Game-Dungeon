using Player.Game;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.FiniteStateMachine.SubState
{
    public class EnemyMoveState : SuperState.EnemyGroundedState
    {
        public EnemyMoveState(EnemyStateController stateController, EnemyStateMachine stateMachine,
            EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic,
            animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Move");

            StateController.NavMeshAgent.isStopped = false;
            StateController.NavMeshAgent.SetDestination(ManagerPlayer.Instance.PlayerPosition);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!(CheckPlayerDistance() <= EnemyStatistic.AttackDistance)) return;
            StateMachine.ChangeState(StateController.IdleState);
            StateController.NavMeshAgent.isStopped = true;
        }
    }
}