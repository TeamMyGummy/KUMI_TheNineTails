using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAbilitySystem;
using System.Xml.Serialization;
using Cysharp.Threading.Tasks;

public class Dash : BlockAbility<BlockAbilitySO>, ITickable
{
    private CharacterMovement _characterMovement;
    private Collider2D _playerCol;
    private Vector2 _rayPos;

    private DashSO _dashSO;
    private float _dashDistance;
    private float _dashDuration;
    private bool _canDash = true;
    private bool _endDash = true;

    private bool isDashing = false;

    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);

        IsTickable = true;
        _dashSO = (DashSO) abilitySo;
        _dashDistance = _dashSO.dashDistance;
        _dashDuration = _dashSO.dashDuration;
        
        _characterMovement = Actor.GetComponent<CharacterMovement>();
        _playerCol = Actor.GetComponent<Collider2D>();
    }

    public override bool CanActivate()
    {
        if (_characterMovement.CheckIsGround())
        {
            _canDash = true;
        }
        
        return _canDash && base.CanActivate();
    }

    protected override void Activate()
    {
        base.Activate();
        
        if (!_characterMovement.CheckIsGround())
        {
            // 공중에서 대쉬 한 번만 가능 
            _canDash = false;
        }
        
        StartDash();
    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {
        // 공중에서 점프 후 착지하면 대쉬 초기화
        if (_characterMovement.CheckIsGround())
        {
            if (!_canDash)
            {
                ResetDash();
            }
        }
    }

    private void StartDash()
    {
        _characterMovement.SetGravityScale(0);
        _endDash = false;
        
        DashAsync(_characterMovement.GetCharacterSpriteDirection()).Forget();
    }

    public void EndDash()
    {
        _characterMovement.ResetGravityScale();
        _endDash = true;
    }

    public void ResetDash()
    {
        _canDash = true;

        if(!_endDash)
            EndDash();
    }
    
    /// <summary>
    /// 실제 Player Transform 변경하는 부분(대쉬)
    /// </summary>
    /// <param name="dir">대쉬 하는 방향</param>
    public async UniTaskVoid DashAsync(Vector2 dir)
    {
        if (isDashing) return;
        isDashing = true;

        float elapsed = 0f;
        Vector3 start = Actor.transform.position;
        Vector3 target = start + (Vector3)(dir.normalized * _dashDistance);

        while (elapsed < _dashDuration)
        {
            float t = elapsed / _dashDuration;
            Vector3 nextPos = Vector3.Lerp(start, target, t);

            // Raycast로 벽 체크
            _rayPos = new Vector2(_playerCol.bounds.center.x, _playerCol.bounds.center.y);
            RaycastHit2D hit = Physics2D.Raycast(_rayPos, dir, (nextPos - Actor.transform.position).magnitude, _characterMovement.GroundMask);
            if (hit.collider != null)
            {
                // 벽에 닿으면 그 직전 위치로 이동 후 종료
                Actor.transform.position = hit.point - dir.normalized * 0.01f;
                isDashing = false;
                //EndDash();
                return;
            }

            Actor.transform.position = nextPos;

            elapsed += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update); // 한 프레임 대기
        }

        Actor.transform.position = target;
        isDashing = false;
        //EndDash();
    }
}
