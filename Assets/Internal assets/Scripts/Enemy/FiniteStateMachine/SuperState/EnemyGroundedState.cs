using System;
using Player.Game;
using UnityEngine;

namespace Enemy.FiniteStateMachine.SuperState
{
    public class EnemyGroundedState : EnemyState
    {
        public EnemyGroundedState(EnemyStateController stateController, EnemyStateMachine stateMachine,
            EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic,
            animBoolName)
        {
        }

        public override void DoChecks()
        {
            if (!EnemyStatistic.isVisiblePlayer)
                CheckIfPlayer(ref EnemyStatistic.isVisiblePlayer);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (EnemyStatistic.IsDead)
            {
                // StateMachine.ChangeState(StateController.DeathState);
            }
        }

        private void CheckIfPlayer(ref bool isVisible)
        {
            var position = StateController.transform.position;
            isVisible = Physics.Raycast(position + new Vector3(0, 0.5f, 0),
                ManagerPlayer.Instance.PlayerPosition - position, out var hit,
                EnemyStatistic.PlayerCheckDistance) && hit.collider.CompareTag("Player");
        }

        protected float CheckPlayerDistance()
        {
            return Vector3.Distance(StateController.transform.position,
                ManagerPlayer.Instance.playerTransform.position);
        }
    }
}