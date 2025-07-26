using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameAbilitySystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterMovement _characterMovement;
    private AbilitySystem _asc;
    private LanternObject _lanternObject;
    private HPRefillStation _hpRefillStation;
    private TailBox _tailBox;
    private NpcObject _npcObject;

    public static event System.Action OnJumpCanceled;
    public event System.Action OnParryingCanceled;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _characterMovement = GetComponent<CharacterMovement>();
        DomainFactory.Instance.GetDomain(DomainKey.Player, out _asc);
        _asc.SetSceneState(gameObject);
    }
    private void Start()
    {
        _asc.GrantAllAbilities();
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
        _characterMovement.Move(inputDirection);
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
        Debug.Log(inputDirection);
        _characterMovement.Move(inputDirection);
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
            _asc.TryActivateAbility(AbilityKey.DoubleJump);
        }
        else if (ctx.canceled)
        {
            OnJumpCanceled?.Invoke();
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
            if (_characterMovement.GetCharacterDirection() != Vector2.zero && _characterMovement.CheckIsGround())
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
    public void OnEnableAllInput()
    {
        _playerInput.ActivateInput();
    }
    public void OnDisableAllInput()
    {
        _playerInput.DeactivateInput();
    }

}
