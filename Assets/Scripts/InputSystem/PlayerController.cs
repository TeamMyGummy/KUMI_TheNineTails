using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameAbilitySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
/*
 * 플레이어의 Input을 관리하는 파일입니다.
 */

public class PlayerController : MonoBehaviour, IMovement
{
    private PlayerInput _playerInput;
    private Player _player;
    private CharacterMovement _characterMovement;
    private WallClimb _wallClimb;
    
    private LanternObject _lanternObject;
    private HPRefillStation _hpRefillStation;
    private TailBox _tailBox;
    private NpcObject _npcObject;
    private MaxHpItem _maxHpItem;
    private FoxFireItem _foxFireItem;

    private Vector2 _direction;
    
    // ------------- Input Values ---------------
    public Vector2 MoveInput { get; private set; }
    public Vector2 ClimbInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool DashPressed { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool ParryingPressed { get; private set; }
    public bool FoxFirePressed { get; private set; }
    public bool LiverExtractionPressed { get; private set; }
    
    
    public event System.Action OnJumpCanceled;
    public event System.Action OnParryingCanceled;
    
    public Vector2 Direction => _direction;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _player = GetComponent<Player>();
        _characterMovement = GetComponent<CharacterMovement>();
        _wallClimb = new WallClimb(gameObject);
    }
    private void Start()
    {
        OnEnableAllInput();

#if !UNITY_EDITOR
        RespawnPlayer();
#endif
        Debug.Log("[Player] 개발 중에는 E로 저장된 포인트로 돌아가지 않습니다. ");
        //이런 종류의 것들이 더 생기면 에디터 단에서 설정할 수 있는 걸 만들겠음
    }

    //스폰된 플레이어를 영구 저장 위치(마지막으로 저장한 호롱불)로 옮김
    private void RespawnPlayer()
    {
        int checkPoint = DomainFactory.Instance.Data.LanternState.RecentCheckPoint;
        if (checkPoint != -1)
        {
            Debug.Log("[Player] 영구 저장된 위치로 돌아왔습니다." + Lantern.Instance.GetLanternPos(checkPoint));
            transform.position = Lantern.Instance.GetLanternPos(checkPoint);
        }
    }

    /*
     * On_ : Input Key가 눌렸을 때 실행되는 함수
     * OnEnable_ : Input 활성화
     * OnDisable_ : Input 비활성화
     */

    // --------------------------- Move ---------------------------
    public void OnMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
        if (MoveInput != Vector2.zero) _direction = MoveInput;
        
        if (!_player.StateMachine.IsCurrentState(PlayerStateType.WallClimb) &&
            !_player.StateMachine.IsCurrentState(PlayerStateType.RopeClimb))
        {
            _characterMovement.Move(MoveInput);
        }
    }

    public void SetDirection(Vector2 direction)
    {
        MoveInput = direction;
    }

    public void OnEnableMove()
    {
        _playerInput.actions["Move"].Enable();
    }

    public void OnDisableMove()
    {
        _playerInput.actions["Move"].Disable();
    }
    
    // ------------------------- WallClimb ------------------------
    public void OnWallClimb(InputAction.CallbackContext ctx)
    {
        ClimbInput = ctx.ReadValue<Vector2>();
        if(ClimbInput.x != 0) _direction = ClimbInput;
        
        if (!_player.StateMachine.IsCurrentState(PlayerStateType.WallClimb) ||
            !_player.StateMachine.IsCurrentState(PlayerStateType.RopeClimb))
        {
             _characterMovement.Move(ClimbInput);
             _wallClimb.SetWallClimbState(ClimbInput);             
        }
    }

    public bool IsPressedClimbKey()
    {
        return _playerInput.actions["WallClimb"].IsPressed();
    }

    public void StartWallClimb(GameObject wall)
    {
        _wallClimb.SetCurrentWall(wall);
        OnDisableAllInput();
        OnEnableWallClimb();
        OnEnableJump();
        OnEnableMove();
    }

    public void EndWallClimb()
    {
        _wallClimb.Reset();
        OnEnableAllInput();
    }

    public void StartRopeClimb()
    {
        //OnDisableAllInput();
        OnEnableWallClimb();
        OnEnableJump();
        OnEnableMove();
    }

    public void EndRopeClimb()
    {
        OnEnableAllInput();
    }

    public void OnEnableWallClimb()
    {
        _playerInput.actions["WallClimb"].Enable();
    }

    public void OnDisableWallClimb()
    {
        _playerInput.actions["WallClimb"].Disable();
    }

    // --------------------------- Jump ---------------------------
    public void OnJump(InputAction.CallbackContext ctx)
    {
        JumpPressed = ctx.performed;
        
        if (ctx.started)
        {
            if (_characterMovement.CheckIsClimbing() &&
                _characterMovement.GetCharacterDirection() != Vector2.up &&
                _characterMovement.GetCharacterDirection() != Vector2.down)
            {
                int jumpDir = _characterMovement.CheckIsWallClimbing() ? -1 : 1;
                
                if(_playerInput.actions["Move"].IsPressed() && 
                   (jumpDir == 1 || _direction != _characterMovement.GetCharacterSpriteDirection()))
                {
                    // climb 중 방향키를 누르고 있었을 때
                    _characterMovement.Move(_characterMovement.GetCharacterSpriteDirection() * jumpDir);
                }
                else{
                    _characterMovement.Jump(2.0f, _characterMovement.GetCharacterSpriteDirection() * jumpDir);
                    if(jumpDir == -1) _player.FlipSprite();
                }
            }
        }
        else if (ctx.performed)
        {
            if (_characterMovement.GetCharacterDirection() != Vector2.up &&
                _characterMovement.GetCharacterDirection() != Vector2.down)
            {
                if (_characterMovement.CheckIsRopeClimbing())
                {
                    _characterMovement.EndRopeClimbState();
                    EndRopeClimb();
                }                
            }

        }
        else if (ctx.canceled)
        {
            OnJumpCanceled?.Invoke();
            OnJumpCanceled = null;
        } 
    }

    public bool IsJumpPressed()
    {
        bool pressed = JumpPressed;
        JumpPressed = false; // 한 번만 읽도록 리셋
        return pressed;
    }
    
    public void OnEnableJump()
    {
        _playerInput.actions["Jump"].Enable();
    }

    public void OnDisableJump()
    {
        _playerInput.actions["Jump"].Disable();
    }

    // --------------------------- Dash ---------------------------

    public void OnDash(InputAction.CallbackContext ctx)
    {
        DashPressed = ctx.performed;
    }

    public bool IsDashPressed()
    {
        bool pressed = DashPressed;
        DashPressed = false; // 한 번만 읽도록 리셋
        return pressed;
    }
    
    public void OnEnableDash()
    {
        _playerInput.actions["Dash"].Enable();
    }

    public void OnDisableDash()
    {
        _playerInput.actions["Dash"].Disable();
    }

    // --------------------------- Attack ---------------------------
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        AttackPressed = ctx.performed;
    }

    public bool IsAttackPressed()
    {
        bool pressed = AttackPressed;
        AttackPressed = false; // 한 번만 읽도록 리셋
        return pressed;
    }
    
    public void OnEnableAttack()
    {
        _playerInput.actions["Attack"].Enable();
    }

    public void OnDisableAttack()
    {
        _playerInput.actions["Attack"].Disable();
    }

    // --------------------------- Parrying ---------------------------
    public void OnParrying(InputAction.CallbackContext ctx)
    {
        ParryingPressed = ctx.performed;
        
        if (ctx.canceled)
        {
            OnParryingCanceled?.Invoke();
        } 
    }

    public bool IsParryingPressed()
    {
        bool pressed = ParryingPressed;
        ParryingPressed = false; // 한 번만 읽도록 리셋
        return pressed;
    }
    
    public void OnEnableParrying()
    {
        _playerInput.actions["Parrying"].Enable();
    }

    public void OnDisableParrying()
    {
        _playerInput.actions["Parrying"].Disable();
    }
    
    // --------------------------- FoxFire ---------------------------
    public void OnFoxFire(InputAction.CallbackContext ctx)
    {
        FoxFirePressed = ctx.performed;
    }
    
    public bool IsFoxFirePressed()
    {
        bool pressed = FoxFirePressed;
        FoxFirePressed = false; // 한 번만 읽도록 리셋
        return pressed;
    }
    
    public void OnEnableFoxFire()
    {
        _playerInput.actions["FoxFire"].Enable();
    }

    public void OnDisableFoxFire()
    {
        _playerInput.actions["FoxFire"].Disable();
    }
    
    // --------------------------- Liver Extraction ---------------------------
    public void OnLiverExtraction(InputAction.CallbackContext ctx)
    {
        LiverExtractionPressed = ctx.performed;
    }
    
    public bool IsLiverExtractionPressed()
    {
        bool pressed = LiverExtractionPressed;
        LiverExtractionPressed = false; // 한 번만 읽도록 리셋
        return pressed;
    }
    
    public void OnEnableLiverExtraction()
    {
        _playerInput.actions["LiverExtraction"].Enable();
    }

    public void OnDisableLiverExtraction()
    {
        _playerInput.actions["LiverExtraction"].Disable();
    }
    
    // -------------------- Map Interaction -----------------------
    // Lantern
    public void OnLanternInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (_lanternObject == null)
            {
                /*Debug.Log("[LanternInteraction] _lanternObject가 null입니다.");*/
                return;
            }
            _lanternObject.InteractWithLantern();
        }
    }
    public void SetLanternObject(LanternObject lantern)
    {
        _lanternObject = lantern;
    }
    
    // HP Refill Station
    public void OnRefillHP(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (_hpRefillStation != null)
            {
                _hpRefillStation.RefillHp();
            }
            else
            {
                Debug.LogWarning("[PlayerController] _hpRefillStation == null");
            }
        }
    }
    
    public void SetHpRefillStation(HPRefillStation station)
    {
        if (station != null || _hpRefillStation != station)
        {
            _hpRefillStation = station;
        }
    }
    
    // Tail Box
    public void OnTailBoxInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (_tailBox != null)
            {
                _tailBox.TailBoxInteraction();
            }
            else
            {
                Debug.LogWarning("[PlayerController] _tailBox == null");
            }
        }
    }
    
    public void SetTailBox(TailBox tailbox)
    {
        if (tailbox != null || _tailBox != tailbox)
        {
            _tailBox = tailbox;
        }
    }

    // Max HP Item
    public void OnMaxHpItemInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (_maxHpItem != null)
            {
                _maxHpItem.ApplyMaxHpIncrease();
            }
            else
            {
                Debug.LogWarning("[PlayerController] _maxHpItem == null");
            }
        }
    }

    public void SetMaxHpItem(MaxHpItem HPitem)
    {
        if (HPitem != null || _maxHpItem != HPitem)
        {
            _maxHpItem = HPitem;
        }
    }

    public void OnFoxFireItemInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.started && _foxFireItem != null)
        {
            _foxFireItem.ApplyFoxFireIncrease();
        }
    }

    public void SetFoxFireItem(FoxFireItem item)
    {
        if (item != null || _foxFireItem != item)
        {
            _foxFireItem = item;
        }
    }
        
    // NPC Interaction
    public void OnNpcInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (_npcObject != null)
            {
                _npcObject.NpcInteraction();
            }
            else
            {
                Debug.LogWarning("[PlayerController] NPC == null");
            }
        }
    }
    
    public void SetNpc(NpcObject npc)
    {
        if (npc != null || _npcObject != npc)
        {
            _npcObject = npc;
        }
    }
    

    // ----------------------------------------------------------------
    /// <summary>
    /// 모든 Player Input 활성화
    /// </summary>
    public void OnEnableAllInput()
    {
        _playerInput.ActivateInput();
        OnDisableLiverExtraction();
    }
    public void OnDisableAllInput()
    {
        _playerInput.DeactivateInput();
    }
}
