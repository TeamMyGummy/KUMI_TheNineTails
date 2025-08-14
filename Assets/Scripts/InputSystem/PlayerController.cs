using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameAbilitySystem;
using UnityEngine;
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
    private AbilitySystem _asc;
    private LanternObject _lanternObject;
    private HPRefillStation _hpRefillStation;
    private TailBox _tailBox;
    private NpcObject _npcObject;
    private Vector2 _direction;
    private MaxHpItem _maxHpItem;
    private FoxFireItem _foxFireItem;

    public event System.Action OnJumpCanceled;
    public event System.Action OnParryingCanceled;
    
    public Vector2 Direction => _direction;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _player = GetComponent<Player>();
        _characterMovement = GetComponent<CharacterMovement>();
        _wallClimb = new WallClimb(gameObject);
        
        DomainFactory.Instance.GetDomain(DomainKey.Player, out _asc);
        _asc.SetSceneState(gameObject);
    }
    private void Start()
    {
        /*_asc.GrantAbility(AbilityKey.Jump, AbilityName.Jump);
        _asc.GrantAbility(AbilityKey.Dash, AbilityName.Dash);
        _asc.GrantAbility(AbilityKey.DoubleJump, AbilityName.DoubleJump);*/
        _asc.GrantAllAbilities();
        
        OnEnableAllInput();

#if !UNITY_EDITOR
        RespawnPlayer();
#endif
        Debug.Log("[Player] 개발 중에는 E로 저장된 포인트로 돌아가지 않습니다. ");
        //이런 종류의 것들이 더 생기면 에디터 단에서 설정할 수 있는 걸 만들겠음
    }

    private void FixedUpdate()
    {
        // WallClimb
        if (_characterMovement.CheckIsWallClimbing() && _wallClimb.IsCharacterReachedTop() && _wallClimb.GetState() == WallClimb.WallClimbState.Climbing)
        {
            _wallClimb.OnReachedTop();
            OnDisableAllInput();
        }
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
        Vector2 inputDirection = ctx.ReadValue<Vector2>();
        if(inputDirection.x != 0) _direction = inputDirection;
        
        /*if (_characterMovement.CheckIsWallClimbing())
        {
            if (ctx.started && inputDirection != _characterMovement.GetCharacterSpriteDirection())
            {
                // sprite가 바라보는 방향의 반대 방향일 때 성립
                _wallClimb.SetWallClimbState(inputDirection);
                OnEnableJump();
            }
            if (ctx.canceled) OnDisableJump();
        }*/
        if (!_characterMovement.CheckIsWallClimbing())
        {
            _characterMovement.Move(inputDirection);
        }
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
        Vector2 inputDirection = ctx.ReadValue<Vector2>();
        
        _characterMovement.Move(inputDirection);
        _wallClimb.SetWallClimbState(inputDirection);           
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
        if (ctx.started)
        {
            if (_characterMovement.CheckIsWallClimbing())
            {
                if (_characterMovement.CheckIsRopeClimbing())
                if(_playerInput.actions["Move"].IsPressed())
                {
                    // climb 중 방향키를 누르고 있었을 때
                    _characterMovement.Move(_characterMovement.GetCharacterSpriteDirection() * (-1));
                }
                else{
                    _characterMovement.Jump(2.0f, _characterMovement.GetCharacterSpriteDirection() * (-1));
                    _player.FlipSprite();
                }
            }
        }
        else if (ctx.performed)
        {
            _asc.TryActivateAbility(AbilityKey.DoubleJump);
        }
        else if (ctx.canceled)
        {
            OnJumpCanceled?.Invoke();
            OnJumpCanceled = null;
        } 
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
        if (ctx.performed)
        {
            if (_characterMovement.CheckIsGround())
            {
                _asc.TryActivateAbility(AbilityKey.Dash);
                OnDisableJump();
            }
        }
        else if (ctx.canceled)
        {
            OnEnableJump();
        }
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
        if (ctx.performed)
        {
            _asc.TryActivateAbility(AbilityKey.PlayerAttack);
        }
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
        if (ctx.started)
        {
            _asc.TryActivateAbility(AbilityKey.Parrying);
        }
        else if (ctx.canceled)
        {
            OnParryingCanceled?.Invoke();
        } 
    }

    public void OnEnableParrying()
    {
        _playerInput.actions["Parrying"].Enable();
    }

    public void OnDisableParrying()
    {
        _playerInput.actions["Parrying"].Disable();
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
            Debug.Log("[Input] E 키 눌림");
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
    /// 벽타기를 제외한 모든 Player Input 활성화
    /// </summary>
    public void OnEnableAllInput()
    {
        _playerInput.ActivateInput();
        OnDisableWallClimb();
    }
    public void OnDisableAllInput()
    {
        _playerInput.DeactivateInput();
    }
}
