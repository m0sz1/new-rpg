using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator Anim { get; private set; }

    public Rigidbody2D rb { get; private set; }

    public PlayerInputSet input { get; private set; }

    private StateMachine stateMachine;
    
    //各类状态类的引用
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public Player_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_BasicAttackState basicAttackState { get; private set; }
    public Player_JumpAttackState jumpAttackState { get; private set; }

    [Header("Attack details")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity;
    public float attackVelocityDuration = .1f;
    public float comboResetTime = 1;
    private Coroutine queuedAttackCo;



    [Header("Movement details")]
    public float moveSpeed;
    public float jumpForce = 5;
    public Vector2 wallJumpForce;

    [Range(0,1)]
    public float inAirMoveMultiplier = .7f;
    [Range(0,1)]
    public float wallSlideSlowMultiplier = .7f;
    [Space]
    public float dashDuration = .25f;
    public float dashSpeed = 20;

    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;
    public Vector2 moveInput { get; private set; }

    [Header("Collision detection")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }
    private void Awake()
    {
        Anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine = new StateMachine();
        input = new PlayerInputSet();

        //要确保"idle"、"move"等与unity中Animator中Parameters相同
        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new Player_WallJumpState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new Player_JumpAttackState(this, stateMachine, "jumpAttack");
    }

    private void OnEnable()
    {
        input.Enable(); //启用所有 InputAction，输入系统开始监听键盘 / 手柄 / 鼠标

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
        //input.Player.Movement.started - 按下时触发
        //input.Player.Movement.performed - 刚开始按下按钮时触发，无论是完全按下还是长按
        //input.Player.Movement.canceled - 松开时触发，松开按钮
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }

    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }

    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }
    //xVelocity与rb.linearVelocity.x之间的区别
    //xVelocity是我传进来的值，如摁的是D键，就是向右，那么就是右
    //rb.linearVelocity是此时此刻真实的x的速度
    private void HandleFlip(float xVelocity)
    {
        if (facingRight == true && xVelocity < 0)
            Flip();
        else if(facingRight == false && xVelocity >0)
            Flip();
    }

    public void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir *= -1;
    }

    private void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(primaryWallCheck.position,Vector2.right * facingDir, wallCheckDistance, whatIsGround)
                    && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0,-groundCheckDistance));
        Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
        Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
    }
}
