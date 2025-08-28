using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using GameAbilitySystem;
using FixedUpdate = UnityEngine.PlayerLoop.FixedUpdate;

/*
 * 플레이어의 기본 정보를 관리하는 파일입니다.
 * Player 상태, 상수값 등
 */
public class Player : MonoBehaviour
{
    private CharacterMovement _characterMovement;
    private PlayerController _playerController;
    private Animator _animator;
    private PlayerStateMachine _playerStateMachine;
    
    private AbilitySystem _asc;
    
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    
    // -------------- Animation Parameter -------------------
    public static readonly int RunID = Animator.StringToHash("Run");
    public static readonly int JumpID =  Animator.StringToHash("Jump");
    public static readonly int IsGroundID =  Animator.StringToHash("IsGround");
    public static readonly int IsFallingID =  Animator.StringToHash("IsFalling");
    public static readonly int WallClimbID =  Animator.StringToHash("WallClimb");
    public static readonly int RopeClimbID =  Animator.StringToHash("RopeClimb");
    public static readonly int IsClimbingID =  Animator.StringToHash("IsClimbing");
    public static readonly int EndClimbID =  Animator.StringToHash("EndClimb");
    public static readonly int HurtID =  Animator.StringToHash("Hurt");
    
    public static readonly int DashID = Animator.StringToHash("Dash");
    public static readonly int StartAttackID = Animator.StringToHash("StartAttack");
    public static readonly int AttackCountID = Animator.StringToHash("AttackCount");
    public static readonly int StartParryingID = Animator.StringToHash("StartParrying");
    public static readonly int ParryingID = Animator.StringToHash("Parrying");
    public static readonly int LiverExtractionID = Animator.StringToHash("LiverExtraction");
    
    public PlayerController Controller => _playerController;
    public CharacterMovement Movement => _characterMovement;
    public PlayerStateMachine StateMachine => _playerStateMachine;
    public Animator Animator => _animator;
    public AbilitySystem ASC => _asc;

    private bool _canFlip;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _characterMovement = GetComponent<CharacterMovement>();
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
        
        _playerStateMachine = new PlayerStateMachine(this);
        _playerStateMachine.StartStateMachine();
        
        DomainFactory.Instance.GetDomain(DomainKey.Player, out _asc);
        _asc.SetSceneState(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _asc.GrantAllAbilities();
        _canFlip = true;
    }

    private void Update()
    {
        _playerStateMachine.Update();
    }

    private void FixedUpdate()
    {
        _canFlip = !StateMachine.IsCurrentState(PlayerStateType.WallClimb);
        
        if (_playerController.MoveInput.x != 0 && _canFlip)
        {
            _spriteRenderer.flipX = _playerController.MoveInput.x < 0;
        }
        
    }

    public void SetAnimatorBool(int parameterHash, bool value)
    {
        _animator.SetBool(parameterHash, value);
    }

    public void SetAnimatorFloat(int parameterHash, float value)
    {
        _animator.SetFloat(parameterHash, value);
    }
    
    public void SetAnimatorTrigger(int parameterHash)
    {
        _animator.SetTrigger(parameterHash);
    }

    public void ResetAnimatorTrigger(int parameterHash)
    {
        _animator.ResetTrigger(parameterHash);
    }

    public void ChangeState(PlayerStateType newState)
    {
        _playerStateMachine.ChangeState(newState);
    }
    
    /// <summary>
    /// Sprite를 현재 바라보는 방향의 반대로 뒤집는 함수
    /// </summary>
    public void FlipSprite()
    {
        _spriteRenderer.flipX = _characterMovement.GetCharacterSpriteDirection().x > 0 ? true : false;
        //_playerController.SetDirection(_characterMovement.GetCharacterSpriteDirection() * (-1));
    }

    public Collider2D MakeOverlapHitBox(int layerMask)
    {
        Vector2 center = GetComponent<Collider2D>().bounds.center;
        return Physics2D.OverlapCircle(center, 0.55f, layerMask);
    }
    
    public bool CanWallClimb()
    {
        Collider2D hit = MakeOverlapHitBox(LayerMask.GetMask("GraspableWall"));
        
        if (hit != null)
        {
            // 오브젝트가 있는 방향으로 향할 때만 벽타기 가능
            Vector2 objectDir = hit.transform.position.x - transform.position.x > 0 ? Vector2.right : Vector2.left;
            return _characterMovement.GetCharacterSpriteDirection() ==  objectDir;
        }

        return false;
    }
    
    public bool CanRopeClimb()
    {
        return MakeOverlapHitBox(LayerMask.GetMask("Rope"));
        
        // 윗 방향키 누르고 있을 때
    }
    
    private void OnDrawGizmos()
    {
        Vector2 center = GetComponent<Collider2D>().bounds.center;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, 0.55f);
    }
}
