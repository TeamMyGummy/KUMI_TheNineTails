using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wisp : MonoBehaviour
{
    private PlayerStateMachine _playerStateMachine;
    private IMovement _targetMovement;
    private Transform _target;
    public float followSpeed = 2f;
    public Vector3 offset = new Vector3(-1.0f, 1.5f, -1f);
    public Animator animator;
    private static readonly int IsIdle = Animator.StringToHash("isIdle");

    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        _playerStateMachine = player.GetComponent<Player>().StateMachine;
        _targetMovement = player.GetComponent<IMovement>();
        _target = player.transform;
    }
    
    void Update()
    {
        if (_target == null) return;

        CheckPlayerState();

        Vector3 desiredPos = _target.position + new Vector3(offset.x * _targetMovement.Direction.x, offset.y, 0f);

        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
    }

    public void SaveWisp()
    {
        animator.SetTrigger("save");
        SoundManager.Instance.PlaySFX(SFXName.도깨비불저장);
    }

    private void CheckPlayerState()
    {
        switch (_playerStateMachine.CurrentState.GetStateType())
        {
            case PlayerStateType.Idle:
                animator.SetBool(IsIdle, true);
                break;
            default:
                animator.SetBool(IsIdle, false);
                break;
        }
    }
}