using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterMovement _characterMovement;

    private AbilitySystem _asc;

    //private AbilitySystem.Base.AbilitySystem _asc;
    private LanternObject _lanternObject;


    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _characterMovement = GetComponent<CharacterMovement>();
        DomainFactory.Instance.GetDomain(DomainKey.Player, out _asc);
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

    // --------------------------- Jump ---------------------------
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            OnDisableJump();
            _characterMovement.Jump();
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

    // --------------------------- Double Jump ---------------------------
    public void OnDoubleJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _characterMovement.Jump();
        }
        if (ctx.performed)
        {
            OnDisableDoubleJump();
            _characterMovement.Jump();
        }
    }

    public void OnEnableDoubleJump()
    {
        _playerInput.actions["DoubleJump"].Enable();
    }

    public void OnDisableDoubleJump()
    {
        _playerInput.actions["DoubleJump"].Disable();
    }

    // --------------------------- Dash ---------------------------
    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
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
            _asc.TryActivateAbility(AbilityName.Attack);
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
            // 게이지 시작
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

    // -------------------- Lantern Interaction -----------------------
    public void OnLanternInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            _lanternObject.InteractWithLantern();
        }
    }
    public void SetLanternObject(LanternObject lantern)
    {
        _lanternObject = lantern;
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
