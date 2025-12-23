using JetBrains.Annotations;
using UnityEngine;

public class Player_IdleState : Player_GroundedState
{
    public Player_IdleState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        
        //使水平速度为0
        player.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput.x == player.facingDir && player.wallDetected)
            return; //return 后面的代码不执行，防止在墙边按向墙的方向移动时切换到moveState

        if (player.moveInput.x != 0)
            stateMachine.ChangeState(player.moveState);
    }
}
