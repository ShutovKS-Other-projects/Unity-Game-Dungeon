using Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState;
namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SubState
{
    public class PlayerDamageState : PlayerAbilityState
    {   
        public PlayerDamageState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(stateController, stateMachine, playerData, animBoolName)
        {
        }
    
        // ReSharper disable Unity.PerformanceAnalysis
        public override void Enter()
        {
            base.Enter();
        
            playerData.health -= 5;
        }
    
        // ReSharper disable Unity.PerformanceAnalysis
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsAnimationFinished)
                return;
        
            if (playerData.health > 0)
            {
                StateMachine.ChangeState(StateController.IdleState);
            }
            else
            {
                StateMachine.ChangeState(StateController.DeathState);
            }
        }
    
        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();
        
            isAbilityDone = true;
        }
    }
}
