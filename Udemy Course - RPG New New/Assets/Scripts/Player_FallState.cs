using UnityEngine;

public class Player_FallState : Player_AiredState
{
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    //public override void Enter()
    //{
    //    base.Enter();
    
    //}

    public override void Update()
    {
        base.Update();

        //检测玩家下面是否有地面，如果有，切换到idle状态
        if (player.groundDetected)
            stateMachine.ChangeState(player.idleState);

        if (player.wallDetected)
            stateMachine.ChangeState(player.wallSlideState);
    }
}
