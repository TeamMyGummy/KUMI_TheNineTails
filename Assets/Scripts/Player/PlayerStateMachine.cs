using System;
using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

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
    public bool IsBeforeState(PlayerStateType stateType)
    { 
        bool isBefore = _beforeState != null && _beforeState.GetStateType() == stateType;
        if(isBefore) _beforeState = null; // 한 번만 체크
        return isBefore;
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
            AbilityKey jumpType = Player.ASC.IsGranted(AbilityKey.DoubleJump) ?  AbilityKey.DoubleJump : AbilityKey.Jump;
            
            if(Player.ASC.CanActivateAbility(jumpType))
                Player.StateMachine.ChangeState(PlayerStateType.Jump);
        }
        else if (!Player.Movement.CheckIsGround())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Fall);
        }
        else if (Player.Controller.IsDashPressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.Dash))
                Player.StateMachine.ChangeState(PlayerStateType.Dash);
        }
        else if (Player.Controller.IsAttackPressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.PlayerAttack))
                Player.StateMachine.ChangeState(PlayerStateType.Attack);
        }
        else if (Player.Controller.IsParryingPressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.Parrying))
                Player.StateMachine.ChangeState(PlayerStateType.Parrying);
        }
        else if (Player.Controller.IsFoxFirePressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.FoxFire))
                Player.StateMachine.ChangeState(PlayerStateType.FoxFire);
        }
        else if (Player.Controller.IsLiverExtractionPressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.LiverExtraction))
                Player.StateMachine.ChangeState(PlayerStateType.LiverExtraction);
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
        SoundManager.Instance.PlaySFX(SFXName.달리기);
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
            AbilityKey jumpType = Player.ASC.IsGranted(AbilityKey.DoubleJump) ?  AbilityKey.DoubleJump : AbilityKey.Jump;
            
            if(Player.ASC.CanActivateAbility(jumpType))
                Player.StateMachine.ChangeState(PlayerStateType.Jump);
        }
        else if (!Player.Movement.CheckIsGround())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Fall);
        }
        else if (Player.Controller.IsDashPressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.Dash))
                Player.StateMachine.ChangeState(PlayerStateType.Dash);
        }
        else if (Player.Controller.IsAttackPressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.PlayerAttack))
                Player.StateMachine.ChangeState(PlayerStateType.Attack);
        }
        else if (Player.Controller.IsParryingPressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.Parrying))
                Player.StateMachine.ChangeState(PlayerStateType.Parrying);
        }
        else if (Player.Controller.IsFoxFirePressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.FoxFire))
                Player.StateMachine.ChangeState(PlayerStateType.FoxFire);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        SoundManager.Instance.StopSFX(SFXName.달리기);
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
        //Player.SetAnimatorBool(Player.IsGroundID, true);
        _ability = Player.ASC.IsGranted(AbilityKey.DoubleJump) ? Player.ASC.TryActivateAbility(AbilityKey.DoubleJump) : Player.ASC.TryActivateAbility(AbilityKey.Jump); 
        SoundManager.Instance.PlaySFX(SFXName.점프);
    }

    public override void Update()
    {
        if (Player.Movement.GetVelocity().y <= 0 && Player.Movement.CheckIsGround())
        {
            Player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
        else if (Player.Controller.IsJumpPressed())
        {
            if (_ability.TryActivate())
            {
                Player.Animator.Play(Player.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0);
                SoundManager.Instance.PlaySFX(SFXName.점프);
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
            if(Player.ASC.CanActivateAbility(AbilityKey.Dash))
                Player.StateMachine.ChangeState(PlayerStateType.Dash);
        }
        else if (Player.Controller.IsAttackPressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.PlayerAttack))
                Player.StateMachine.ChangeState(PlayerStateType.Attack);
        }
        else if (Player.Controller.IsParryingPressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.Parrying))
                Player.StateMachine.ChangeState(PlayerStateType.Parrying);
        }
        else if (Player.Controller.IsFoxFirePressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.FoxFire))
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
            AbilityKey jumpType = Player.ASC.IsGranted(AbilityKey.DoubleJump) ?  AbilityKey.DoubleJump : AbilityKey.Jump;
            
            if(Player.ASC.CanActivateAbility(jumpType))
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
            if(Player.ASC.CanActivateAbility(AbilityKey.Dash))
                Player.StateMachine.ChangeState(PlayerStateType.Dash);
        }
        else if (Player.Controller.IsAttackPressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.PlayerAttack))
                Player.StateMachine.ChangeState(PlayerStateType.Attack);
        }
        else if (Player.Controller.IsParryingPressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.Parrying))
                Player.StateMachine.ChangeState(PlayerStateType.Parrying);
        }
        else if (Player.Controller.IsFoxFirePressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.FoxFire))
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
        SoundManager.Instance.PlaySFX(SFXName.피격);
    }

    public override void Update()
    {
        if (Player.ASC.Attribute.Attributes["HP"].CurrentValue.Value <= 0)
        {
            // 사망 처리
            Player.StateMachine.ChangeState(PlayerStateType.Die);
        }
        
        var curAnimStateInfo = Player.Animator.GetCurrentAnimatorStateInfo(0);
        if (curAnimStateInfo.IsName("Hurt"))
        {
            if (curAnimStateInfo.normalizedTime >= 0.9f)
            {
                Player.StateMachine.ChangeState(PlayerStateType.Idle);
            }
        }
        else
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
        
        SoundManager.Instance.PlaySFX(SFXName.죽음);
        
        // 저장 데이터 로드 및 상태 복원
        DomainFactory.Instance.ClearStateAndReload();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class WallClimbState : PlayerState
{
    public WallClimbState(Player player) : base(player, PlayerStateType.WallClimb){}

    private const float MOVE_INPUT_THRESHOLD = 0.1f;

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
        SoundManager.Instance.PlaySFX(SFXName.벽에붙음);
        Player.Movement.ClimbState();
        
        _state = WallClimbStates.Idle;
        _currentWall = Player.DrawClimbRay(LayerMask.GetMask("GraspableWall")).collider;
        _playerCollider = Player.GetComponent<Collider2D>();
        _actions = new WallClimbActions(Player, _currentWall);
        _ledge = false;

        Dash DashAbility = Player.ASC.GetAbility(AbilityKey.Dash) as Dash;
        Debug.Assert(DashAbility != null);
        DashAbility.ResetDash();
        
        Jump JumpAbility = (Player.ASC.IsGranted(AbilityKey.DoubleJump) ? Player.ASC.GetAbility(AbilityKey.DoubleJump) : Player.ASC.GetAbility(AbilityKey.Jump)) as Jump;
        Debug.Assert(JumpAbility != null);
        JumpAbility.SetJumpCount(1);

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
                UpdateClimbing();
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
        // move input 방향이 벽 방향과 다르면 벽에서 떨어짐
        // 플레이어가 벽의 왼쪽에 있으면 벽 방향은 1(오른쪽), 오른쪽에 있으면 -1(왼쪽)
        else if (Math.Abs(Player.Controller.MoveInput.x) > MOVE_INPUT_THRESHOLD && Player.Controller.MoveInput.x != (Player.transform.position.x < _currentWall.transform.position.x ? 1f : -1f))
        {
            Player.Movement.Move(Player.Controller.MoveInput);
            Player.StateMachine.ChangeState(PlayerStateType.Fall);
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        
        Player.SetAnimatorBool(Player.WallClimbID, false);
        Player.Movement.EndClimbState();
        
        if(SoundManager.Instance.IsPlayingSFX(SFXName.벽타기))
            SoundManager.Instance.StopSFX(SFXName.벽타기);
    }

    private void Idle()
    {
        // sound
        if(SoundManager.Instance.IsPlayingSFX(SFXName.벽타기))
            SoundManager.Instance.StopSFX(SFXName.벽타기);
        
        _ledge = false;
        Player.SetAnimatorBool(Player.EndClimbID, false);
        Player.SetAnimatorBool(Player.IsClimbingID, false);
    }
    
    private void UpdateClimbing()
    {
        if (!Player.CanWallClimb())
        {
            if (_actions.IsCharacterReachedTop() && Player.Controller.ClimbInput.y < 0)
            {
                Climbing();
            }
            else
            {
                Player.Movement.Move(Vector2.zero);
                Player.SetAnimatorBool(Player.IsClimbingID, false);
                if (Player.Controller.ClimbInput.y < 0 && !_actions.IsCharacterReachedTop())
                {
                    Player.StateMachine.ChangeState(PlayerStateType.Fall);
                }
                return;
            }
        }

        Climbing();
    }

    private void Climbing()
    {
        // sound
        if(!SoundManager.Instance.IsPlayingSFX(SFXName.벽타기))
            SoundManager.Instance.PlaySFX(SFXName.벽타기);
        
        _ledge = false;
        Player.SetAnimatorBool(Player.EndClimbID, false);
        Player.SetAnimatorBool(Player.IsClimbingID, true);

        // 이동
        Player.Movement.Move(Player.Controller.ClimbInput);
    }
    
    private void OnLedge()
    {
        // sound
        if(SoundManager.Instance.IsPlayingSFX(SFXName.벽타기))
            SoundManager.Instance.StopSFX(SFXName.벽타기);
        
        _ledge = true;
        Player.SetAnimatorBool(Player.EndClimbID, true);
        
        Player.Movement.Move(Vector2.zero);

        Vector2 startPos = _playerCollider.bounds.min;
        Vector2 targetPos = new Vector3(
            _currentWall.bounds.center.x,
            _currentWall.bounds.max.y  // 벽 위쪽 약간 위
        );

        // 벽 위로 위치 이동
        _actions.MoveToWallTop(startPos, targetPos).ContinueWith(() =>
        {
            Player.StateMachine.ChangeState(PlayerStateType.Fall);
        }).Forget();
    }
}

public class RopeClimbState : PlayerState
{
    public RopeClimbState(Player player) : base(player, PlayerStateType.RopeClimb){}
    
    private enum RopeClimbStates
    {
        Idle,
        Climbing,
    }
    
    private RopeClimbStates _state;
    private WallClimbActions _actions;
    
    public override void Enter()
    {
        base.Enter();
        
        Player.SetAnimatorBool(Player.RopeClimbID, true);
        Player.Movement.ClimbState();
        
        _state = RopeClimbStates.Idle;
        _actions = new WallClimbActions(Player);

        Dash DashAbility = Player.ASC.GetAbility(AbilityKey.Dash) as Dash;
        Debug.Assert(DashAbility != null);
        DashAbility.ResetDash();
        
        Jump JumpAbility = (Player.ASC.IsGranted(AbilityKey.DoubleJump) ? Player.ASC.GetAbility(AbilityKey.DoubleJump) : Player.ASC.GetAbility(AbilityKey.Jump)) as Jump;
        Debug.Assert(JumpAbility != null);
        JumpAbility.SetJumpCount(1);

        Player.Movement.Move(Vector2.up);
    }
    
    public override void Update()
    {
        switch (_state)
        {
            case RopeClimbStates.Idle:
                Idle();
                break;
            case RopeClimbStates.Climbing:
                Climbing();
                break;
            default:
                Debug.Log("RopeClimbStates not implemented");
                break;
        }

        if (Player.Controller.ClimbInput == Vector2.zero)
        {
            _state = RopeClimbStates.Idle;
        }
        else if (Player.Controller.ClimbInput != Vector2.zero)
        {
            _state = RopeClimbStates.Climbing;
        }
        
        if (Player.Controller.IsJumpPressed())
        {
            _actions.RopeJump();
            Player.OnRopeClimbAvailable.Invoke(false);
            Player.StateMachine.ChangeState(PlayerStateType.Jump);
        }
        else if (!Player.CanRopeClimb())
        {
            Player.Movement.Move(Vector2.zero);
            Player.StateMachine.ChangeState(PlayerStateType.Fall);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        // sound
        if(SoundManager.Instance.IsPlayingSFX(SFXName.줄타기))
            SoundManager.Instance.StopSFX(SFXName.줄타기);
        
        Player.SetAnimatorBool(Player.RopeClimbID, false);
        Player.Movement.EndClimbState();
    }

    private void Idle()
    {
        // sound
        if(SoundManager.Instance.IsPlayingSFX(SFXName.줄타기))
            SoundManager.Instance.StopSFX(SFXName.줄타기);
        
        // Animation
        Player.SetAnimatorBool(Player.EndClimbID, false);
        Player.SetAnimatorBool(Player.IsClimbingID, false);
    }

    private void Climbing()
    {
        // sound
        if(!SoundManager.Instance.IsPlayingSFX(SFXName.줄타기))
            SoundManager.Instance.PlaySFX(SFXName.줄타기);
        
        // Animation
        Player.SetAnimatorBool(Player.EndClimbID, false);
        Player.SetAnimatorBool(Player.IsClimbingID, true);
        
        // 이동
        Player.Movement.Move(Player.Controller.ClimbInput);  
    }
}

public class AttackState : PlayerState
{
    public AttackState(Player player) : base(player, PlayerStateType.Attack){}
    private int _attackNum; // 공격 횟수
    
    public override void Enter()
    {
        base.Enter();

        _attackNum = 1;
        Player.SetAnimatorTrigger(Player.StartAttackID);
        Player.Animator.SetInteger(Player.AttackCountID, _attackNum);
        SoundManager.Instance.PlaySFX(SFXName.공격1);
        
        Player.ASC.TryActivateAbility(AbilityKey.PlayerAttack);  
    }

    public override void Update()
    {
        var curAnimClipInfo = Player.Animator.GetCurrentAnimatorClipInfo(0);
        string stateName = curAnimClipInfo[0].clip.name;
        
        if (stateName.StartsWith("Attack"))
        {
            var curAnimStateInfo = Player.Animator.GetCurrentAnimatorStateInfo(0);
            if (curAnimStateInfo.normalizedTime >= 0.95f)
            {
                Player.StateMachine.ChangeState(PlayerStateType.Idle);
            }
            else if (Player.Controller.IsAttackPressed() && curAnimStateInfo.normalizedTime >= 0.5f) // 짧은 연타 방지
            {
                // 공격 횟수 초기화
                if (_attackNum >= 3)
                {
                    _attackNum = 0;
                }
                
                _attackNum++;
                Player.Animator.SetInteger(Player.AttackCountID, _attackNum);
                switch (_attackNum)
                {
                    case 1:
                        SoundManager.Instance.PlaySFX(SFXName.공격1);
                        break;
                    case 2:
                        SoundManager.Instance.PlaySFX(SFXName.공격2);
                        break;
                    case 3:
                        SoundManager.Instance.PlaySFX(SFXName.공격3);
                        break;
                    default:
                        break;
                }
                
                Player.ASC.TryActivateAbility(AbilityKey.PlayerAttack);
            }
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
        SoundManager.Instance.PlaySFX(SFXName.대쉬);
        Player.ASC.TryActivateAbility(AbilityKey.Dash);
        Player.ASC.TagContainer.Add(GameplayTags.Invincibility);
        //Player.Movement.SetGravityScale(0);
    }
    public override void Update()
    {
        if (Player.CanWallClimb())
        {
            Player.StateMachine.ChangeState(PlayerStateType.WallClimb);
        }
        
        if (Player.CanRopeClimb())
        {
            Player.StateMachine.ChangeState(PlayerStateType.RopeClimb);
        }

        var curAnimStateInfo = Player.Animator.GetCurrentAnimatorStateInfo(0);
        if (curAnimStateInfo.IsName("Dash"))
        {
            if(curAnimStateInfo.normalizedTime >= 0.9f)
            {
                if (Player.Controller.IsAttackPressed())
                {
                    if (Player.ASC.CanActivateAbility(AbilityKey.PlayerAttack))
                        Player.StateMachine.ChangeState(PlayerStateType.Attack);
                }
                else if (Player.Movement.CheckIsGround())
                {
                    Player.StateMachine.ChangeState(PlayerStateType.Idle);
                }
                else if (!Player.Movement.CheckIsGround())
                {
                    Player.StateMachine.ChangeState(PlayerStateType.Fall);
                }
            }
        }
        else
        {
            Player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }
    public override void Exit()
    {
        base.Exit();
        
        Player.ResetAnimatorTrigger(Player.DashID);
        Player.ASC.TagContainer.Remove(GameplayTags.Invincibility);
        //Player.Movement.ResetGravityScale();
        
        Dash DashAbility = Player.ASC.GetAbility(AbilityKey.Dash) as Dash;
        Debug.Assert(DashAbility != null);
        DashAbility.EndDash();
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
        /*if (Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Parrying") &&
            Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            ParryingCanceled();
        }*/
        if (Player.Controller.IsLiverExtractionPressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.LiverExtraction))
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
        if (Player.Controller.IsLiverExtractionPressed())
        {
            if(Player.ASC.CanActivateAbility(AbilityKey.LiverExtraction))
                Player.StateMachine.ChangeState(PlayerStateType.LiverExtraction);
        }
        else
        {
            Player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }
}

public class FoxFireState : PlayerState
{
    public FoxFireState(Player player) : base(player, PlayerStateType.FoxFire){}
    
    public override void Enter()
    {
        base.Enter();
        
        SoundManager.Instance.PlaySFX(SFXName.여우불);
        Player.SetAnimatorTrigger(Player.FoxFireID);
        Player.ASC.TryActivateAbility(AbilityKey.FoxFire);
    }
    
    public override void Update()
    {
        var curAnimStateInfo = Player.Animator.GetCurrentAnimatorStateInfo(0);
        if (curAnimStateInfo.IsName("FoxFire"))
        {
            if (curAnimStateInfo.normalizedTime >= 0.9f)
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
        else
        {
            Player.StateMachine.ChangeState(PlayerStateType.Idle);
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
        
        Player.SetAnimatorTrigger(Player.LiverExtractionID);
        Player.ASC.TryActivateAbility(AbilityKey.LiverExtraction);
    }

    public override void Update()
    {
        var curAnimStateInfo = Player.Animator.GetCurrentAnimatorStateInfo(0);
        if (curAnimStateInfo.IsName("LiverExtraction"))
        {
            if (curAnimStateInfo.normalizedTime >= 0.9f)
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
        else
        {
            Player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        
        LiverExtraction ability = Player.ASC.GetAbility(AbilityKey.LiverExtraction) as LiverExtraction;
        Debug.Assert(ability != null);
        ability.EndAbility();
    }
}
