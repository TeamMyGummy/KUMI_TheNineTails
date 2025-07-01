using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private CharacterMovement characterMovement;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        characterMovement = GetComponent<CharacterMovement>();
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
        characterMovement.Move(inputDirection);
    }

    public void OnEnableMove()
    {
        playerInput.actions["Move"].Enable();
    }

    public void OnDisableMove()
    {
        playerInput.actions["Move"].Disable();
    }

    // --------------------------- Jump ---------------------------
    public void OnJump()
    {
        characterMovement.Jump();
        OnDisableJump();
    }

    public void OnEnableJump()
    {
        playerInput.actions["Jump"].Enable();
    }

    public void OnDisableJump()
    {
        playerInput.actions["Jump"].Disable();
    }

    // --------------------------- Dash ---------------------------
    public void OnDash(InputAction.CallbackContext ctx)
    {

    }

    public void OnEnableDash()
    {
        playerInput.actions["Dash"].Enable();
    }

    public void OnDisableDash()
    {
        playerInput.actions["Dash"].Disable();
    }

    // --------------------------- Attack ---------------------------
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        // tryActivate(Attack());
    }

    public void OnEnableAttack()
    {
        playerInput.actions["Attack"].Enable();
    }

    public void OnDisableAttack()
    {
        playerInput.actions["Attack"].Disable();
    }

    // --------------------------- Parrying ---------------------------
    public void OnParrying(InputAction.CallbackContext ctx)
    {
        //tryActivate(Parrying());
    }

    public void OnEnableParrying()
    {
        playerInput.actions["Parrying"].Enable();
    }

    public void OnDisableParrying()
    {
        playerInput.actions["Parrying"].Disable();
    }

    // ----------------------------------------------------------------
    public void OnEnableAllInput()
    {
        playerInput.ActivateInput();
    }
    public void OnDisableAllInput()
    {
        playerInput.DeactivateInput();
    }
}
