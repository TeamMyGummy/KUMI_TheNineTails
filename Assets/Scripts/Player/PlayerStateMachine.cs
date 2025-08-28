using System;
using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;

// Enum: Player State
public enum PlayerStateType
{
    Idle,
    Run,
    Jump,
    Fall,
    Hurt,
    Die,
    WallClimb,
    RopeClimb,
    Attack,
    Dash,
    Parrying,
    FoxFire,
    LiverExtraction,
}

// 추상 상태 클래스
public abstract class PlayerState
{
    protected Player Player;
    protected PlayerStateType StateType;

    /// <summary>
    /// State 생성자
    /// </summary>
    /// <param name="player">Player Class</param>
    /// <param name="type">State Name</param>
    public PlayerState(Player player, PlayerStateType type)
    {
        StateType = type;
        Player = player;
    }

    public virtual bool CanChangeState(PlayerStateType newStateType) { return true; }
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }

    public PlayerStateType GetStateType()
    {
        return StateType;
    }
}

public class PlayerStateMachine
{
    private readonly Dictionary<PlayerStateType, PlayerState> _states;
    private PlayerState _currentState;
    private PlayerState _beforeState;
    private Player _player;

    public PlayerState CurrentState => _currentState;

    public PlayerStateMachine(Player player)
    {
        _player = player;
        _states = new Dictionary<PlayerStateType, PlayerState>();
        InitializeStates();
    }

    private void InitializeStates()
    {
        // 모든 상태 초기화
        _states.Add(PlayerStateType.Idle, new IdleState(_player));
        _states.Add(PlayerStateType.Run, new RunState(_player));
        _states.Add(PlayerStateType.Jump, new JumpState(_player));
        _states.Add(PlayerStateType.Fall, new FallState(_player));
        _states.Add(PlayerStateType.Hurt, new HurtState(_player));
        _states.Add(PlayerStateType.Die, new DieState(_player));
        _states.Add(PlayerStateType.WallClimb, new WallClimbState(_player));
        _states.Add(PlayerStateType.RopeClimb, new RopeClimbState(_player));
        _states.Add(PlayerStateType.Attack, new AttackState(_player));
        _states.Add(PlayerStateType.Dash, new DashState(_player));
        _states.Add(PlayerStateType.Parrying, new ParryingState(_player));
        _states.Add(PlayerStateType.FoxFire, new FoxFireState(_player));
        _states.Add(PlayerStateType.LiverExtraction, new LiverExtractionState(_player));
    }
    
    public void StartStateMachine()
    {
        _currentState = _states[PlayerStateType.Idle];
        _currentState.Enter();
    }
    
    public void Update()
    {
        _currentState?.Update();
    }
    
    /// <summary>
    /// State 강제 전환. 무조건 전환됨
    /// </summary>
    /// <param name="newStateType">새로운 State</param>
    public void ChangeState(PlayerStateType newStateType)
    {
        if (_currentState != null && _currentState.GetStateType() == newStateType)
            return;
        
        _currentState?.Exit();
        _beforeState = _currentState;
        _currentState = _states[newStateType];
        _currentState.Enter();
        Debug.Log(_currentState.GetStateType());
    }

    public PlayerStateType GetCurrentState()
    {
        return _currentState.GetStateType();
    }

    public PlayerStateType GetBeforeState()
    {
        return _beforeState.GetStateType();
    }
    
    public bool IsCurrentState(PlayerStateType stateType)
    {
        return _currentState != null && _currentState.GetStateType() == stateType;
    }
}


//------------------------------------------------------------------------------

public class IdleState : PlayerState
{
    public IdleState(Player player) : base(player, PlayerStateType.Idle){}
    
    public override void Enter()
    {
        base.Enter();
        
        Player.SetAnimatorBool(Player.RunID, false);
        Player.SetAnimatorBool(Player.IsGroundID, true);
    }

    public override void Update()
    {
        if (Player.Controller.MoveInput != Vector2.zero)
        {
            Player.StateMachine.ChangeState(PlayerStateType.Run);
        }
        else if (Player.Controller.IsJumpPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Jump);
        }
        else if (!Player.Movement.CheckIsGround())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Fall);
        }
        else if (Player.Controller.IsDashPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Dash);
        }
        else if (Player.Controller.IsAttackPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Attack);
        }
        else if (Player.Controller.IsParryingPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Parrying);
        }
        else if (Player.Controller.IsFoxFirePressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.FoxFire);
        }
    }
    
}

public class RunState : PlayerState
{
    public RunState(Player player) : base(player, PlayerStateType.Run){}
    
    public override void Enter()
    {
        base.Enter();
        
        Player.SetAnimatorBool(Player.RunID, true);
    }
    
    public override void Update()
    {
        Player.Movement.Move(Player.Controller.MoveInput); 
        
        if (Player.Controller.MoveInput == Vector2.zero)
        {
            Player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
        else if (Player.Controller.IsJumpPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Jump);
        }
        else if (!Player.Movement.CheckIsGround())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Fall);
        }
        else if (Player.Controller.IsDashPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Dash);
        }
        else if (Player.Controller.IsAttackPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Attack);
        }
        else if (Player.Controller.IsParryingPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Parrying);
        }
        else if (Player.Controller.IsFoxFirePressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.FoxFire);
        }
    }
}

public class JumpState : PlayerState
{
    public JumpState(Player player) : base(player, PlayerStateType.Jump){}

    private GameplayAbility _ability;
    
    public override void Enter()
    {
        base.Enter();
        
        Player.SetAnimatorBool(Player.JumpID, true);
        _ability = Player.ASC.TryActivateAbility(AbilityKey.DoubleJump);
    }

    public override void Update()
    {
        if (Player.Movement.CheckIsGround())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
        else if (Player.Controller.IsJumpPressed())
        {
            if (_ability.TryActivate())
            {
                Player.Animator.Play(Player.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0);
            }
        }
        else if (Player.Movement.GetVelocity().y < 0 && Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !Player.Movement.CheckIsGround())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Fall);
        }
        else if (Player.CanWallClimb())
        {
            Player.StateMachine.ChangeState(PlayerStateType.WallClimb);
        }
        else if (Player.CanRopeClimb())
        {
            Player.StateMachine.ChangeState(PlayerStateType.RopeClimb);
        }
        else if (Player.Controller.IsDashPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Dash);
        }
        else if (Player.Controller.IsAttackPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Attack);
        }
        else if (Player.Controller.IsParryingPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Parrying);
        }
        else if (Player.Controller.IsFoxFirePressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.FoxFire);
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        
        Player.SetAnimatorBool(Player.JumpID, false);
    }
}

public class FallState : PlayerState
{
    public FallState(Player player) : base(player, PlayerStateType.Fall){}
    
    public override void Enter()
    {
        base.Enter();
        
        Player.SetAnimatorBool(Player.IsGroundID, false);
        Player.SetAnimatorBool(Player.IsFallingID, true);
    }

    public override void Update()
    {
        if (Player.Movement.CheckIsGround())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
        else if (Player.Controller.IsJumpPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Jump);
        }
        else if (Player.CanWallClimb())
        {
            Player.StateMachine.ChangeState(PlayerStateType.WallClimb);
        }
        else if (Player.CanRopeClimb())
        {
            Player.StateMachine.ChangeState(PlayerStateType.RopeClimb);
        }
        else if (Player.Controller.IsDashPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Dash);
        }
        else if (Player.Controller.IsAttackPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Attack);
        }
        else if (Player.Controller.IsParryingPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Parrying);
        }
        else if (Player.Controller.IsFoxFirePressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.FoxFire);
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        
        Player.SetAnimatorBool(Player.IsFallingID, false);
        
        if (Player.Movement.CheckIsGround())
        {
            Player.SetAnimatorBool(Player.IsGroundID, true);
        }
    }
}

public class HurtState : PlayerState
{
    public HurtState(Player player) : base(player, PlayerStateType.Hurt){}
    
    public override void Enter()
    {
        base.Enter();
        
        Player.SetAnimatorTrigger(Player.HurtID);
    }

    public override void Update()
    {
        if (Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt") &&
            Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            Player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        Player.ResetAnimatorTrigger(Player.HurtID);
    }
}

public class DieState : PlayerState
{
    public DieState(Player player) : base(player, PlayerStateType.Die){}
    
    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class WallClimbState : PlayerState
{
    public WallClimbState(Player player) : base(player, PlayerStateType.WallClimb){}

    private enum WallClimbStates
    {
        Idle,
        Climbing,
        Ledge
    }

    private enum WallType
    {
        Normal,
        PlatformAbove
    }

    private WallClimbStates _state;
    private WallType _type;
    private WallClimbActions _actions;
    
    private Collider2D _currentWall;
    private Collider2D _playerCollider;
    private bool _ledge;    // ledge에 도달했는지 확인
    
    public override void Enter()
    {
        base.Enter();
        
        Player.SetAnimatorBool(Player.WallClimbID, true);
        Player.Movement.ClimbState();
        
        _state = WallClimbStates.Idle;
        _currentWall = Player.MakeOverlapHitBox(LayerMask.GetMask("GraspableWall"));
        _playerCollider = Player.GetComponent<Collider2D>();
        _actions = new WallClimbActions(Player, _currentWall);

        _ledge = false;
        
        // 벽 타입 체크
        _type = _actions.CheckPlatformAboveWall() ? WallType.PlatformAbove : WallType.Normal;
    }

    public override void Update()
    {
        switch (_state)
        {
            case WallClimbStates.Idle:
                Idle();
                break;
            case WallClimbStates.Climbing:
                Climbing();
                break;
            case WallClimbStates.Ledge:
                if(!_ledge)
                    OnLedge();
                break;
            default:
                Debug.Log("WallClimbStates not implemented");
                break;
        }
        
        if (_type == WallType.Normal && _actions.IsCharacterReachedTop())
        {
            _state = WallClimbStates.Ledge;
        }
        else if (Player.Controller.ClimbInput == Vector2.zero)
        {
            _state = WallClimbStates.Idle;
        }
        else if (Player.Controller.ClimbInput != Vector2.zero)
        {
            _state = WallClimbStates.Climbing;
        }
        
        if (Player.Controller.IsJumpPressed())
        {
            _actions.WallJump();
            Player.StateMachine.ChangeState(PlayerStateType.Jump);
        }
        else if (Player.Controller.IsAttackPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Attack);
        }
        else if (Player.Controller.IsParryingPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Parrying);
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        
        Player.SetAnimatorBool(Player.WallClimbID, false);
        Player.Movement.EndClimbState();
    }

    private void Idle()
    {
        _ledge = false;
        Player.SetAnimatorBool(Player.EndClimbID, false);
        Player.SetAnimatorBool(Player.IsClimbingID, false);
    }
    
    private void Climbing()
    {
        if (_type == WallType.PlatformAbove)
        {
            if (!Player.CanWallClimb())
            {
                Player.Movement.Move(Vector2.zero);  
                Player.StateMachine.ChangeState(PlayerStateType.Fall);
                return;
            }
        }
        _ledge = false;
        Player.SetAnimatorBool(Player.EndClimbID, false);
        Player.SetAnimatorBool(Player.IsClimbingID, true);
      
        // 이동
        Player.Movement.Move(Player.Controller.ClimbInput);          
    }
    
    private void OnLedge()
    {
        _ledge = true;
        Player.SetAnimatorBool(Player.EndClimbID, true);
        
        Player.Movement.Move(Vector2.zero);
        Player.StateMachine.ChangeState(PlayerStateType.Fall);
        
        Vector2 startPos = _playerCollider.bounds.min;
        Vector2 targetPos = new Vector3(
            _currentWall.bounds.center.x,
            _currentWall.bounds.max.y + 0.5f  // 벽 위쪽 약간 위
        );
        
        // 벽 위로 위치 이동
        _actions.MoveToWallTop(startPos, targetPos).Forget();
    }
}

public class RopeClimbState : PlayerState
{
    public RopeClimbState(Player player) : base(player, PlayerStateType.RopeClimb){}
    
    public override void Enter()
    {
        base.Enter();
        
        Player.SetAnimatorBool(Player.RopeClimbID, true);
        Player.Movement.ClimbState();
    }
    
    public override void Update()
    {
        if (Player.Movement.CheckIsGround())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
        else if (Player.Controller.IsJumpPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Jump);
        }
        else if (Player.Controller.IsAttackPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Attack);
        }
        else if (Player.Controller.IsParryingPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Parrying);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        Player.SetAnimatorBool(Player.RopeClimbID, false);
        Player.Movement.EndClimbState();
    }
}

public class AttackState : PlayerState
{
    public AttackState(Player player) : base(player, PlayerStateType.Attack){}
    
    public override void Enter()
    {
        base.Enter();
        
        Player.SetAnimatorTrigger(Player.StartAttackID);
        Player.ASC.TryActivateAbility(AbilityKey.PlayerAttack);
    }

    public override void Update()
    {
        if (Player.Controller.IsAttackPressed())
        {
            Player.ASC.TryActivateAbility(AbilityKey.PlayerAttack);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        Player.Animator.SetInteger(Player.AttackCountID, 0);
    }
}

public class DashState : PlayerState
{
    public DashState(Player player) : base(player, PlayerStateType.Dash){}
    
    public override void Enter()
    {
        base.Enter();
        
        Player.SetAnimatorTrigger(Player.DashID);
        Player.ASC.TryActivateAbility(AbilityKey.Dash);
    }
    public override void Update()
    {
        if (Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Dash") && Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        {
            if (Player.Movement.CheckIsGround())
            {
                Player.StateMachine.ChangeState(PlayerStateType.Idle);
            }
            else if (!Player.Movement.CheckIsGround())
            {
                Player.StateMachine.ChangeState(PlayerStateType.Fall);
            }
        }
    }
    public override void Exit()
    {
        base.Exit();
        
        Player.ResetAnimatorTrigger(Player.DashID);
    }
}

public class ParryingState : PlayerState
{
    public ParryingState(Player player) : base(player, PlayerStateType.Parrying){}
    
    public override void Enter()
    {
        base.Enter();
        
        Player.SetAnimatorTrigger(Player.StartParryingID);
        Player.SetAnimatorBool(Player.ParryingID, true);
        Player.ASC.TryActivateAbility(AbilityKey.Parrying);

        Player.Controller.OnParryingCanceled -= ParryingCanceled;
        Player.Controller.OnParryingCanceled += ParryingCanceled;
    }

    public override void Update()
    {
        if (Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Parrying") &&
            Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            ParryingCanceled();
        }
        else if (Player.Controller.IsLiverExtractionPressed())
        {
            Player.StateMachine.ChangeState(PlayerStateType.LiverExtraction);
        }
    }

    public override void Exit()
    {
        base.Exit();
        Player.SetAnimatorBool(Player.ParryingID, false);
    }

    private void ParryingCanceled()
    {
        Player.StateMachine.ChangeState(PlayerStateType.Idle);
    }
}

public class FoxFireState : PlayerState
{
    public FoxFireState(Player player) : base(player, PlayerStateType.FoxFire){}
    
    public override void Enter()
    {
        base.Enter();
        
        Player.ASC.TryActivateAbility(AbilityKey.FoxFire);
    }
    
    public override void Update()
    {
        // 여우불 애니메이션 나오면 조건문 변경
        if (Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            if (Player.Movement.CheckIsGround())
            {
                Player.StateMachine.ChangeState(PlayerStateType.Idle);
            }
            else if (!Player.Movement.CheckIsGround())
            {
                Player.StateMachine.ChangeState(PlayerStateType.Fall);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class LiverExtractionState : PlayerState
{
    public LiverExtractionState(Player player) : base(player, PlayerStateType.LiverExtraction){}
    
    public override void Enter()
    {
        base.Enter();
        
        //Player.SetAnimatorBool(Player.LiverExtractionID, true);
        Player.ASC.TryActivateAbility(AbilityKey.LiverExtraction);
    }

    public override void Update()
    {
        if (Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("LiverExtraction") && Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            if (Player.Movement.CheckIsGround())
            {
                Player.StateMachine.ChangeState(PlayerStateType.Idle);
            }
            else if (!Player.Movement.CheckIsGround())
            {
                Player.StateMachine.ChangeState(PlayerStateType.Fall);
            }
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        //Player.SetAnimatorBool(Player.LiverExtractionID, false);
    }
}
