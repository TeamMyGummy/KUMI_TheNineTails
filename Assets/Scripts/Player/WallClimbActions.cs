using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 플레이어 벽타기 상태 중 행동 / 타입 체크
/// </summary>
public class WallClimbActions
{
    private Player _player;
    private Collider2D _playerCollider;
    private Collider2D _currentWall;
    
    public WallClimbActions(Player player, Collider2D currentWall)
    {
        _player = player;
        _currentWall = currentWall;
        _playerCollider = _player.GetComponent<Collider2D>();
    }
    
    /// <summary>
    /// 벽 타입 체크 (플랫폼이랑 붙어있는 벽인지, 혼자 있는 벽인지)
    /// </summary>
    /// <returns>True: 벽 위에 플랫폼이 붙어있는 형태</returns>
    public bool CheckPlatformAboveWall()
    {
        Vector2 rayDir = new Vector2(_currentWall.bounds.center.x, _currentWall.bounds.max.y);
        
        Debug.DrawRay( rayDir, Vector2.up * 1f, new Color(1, 0, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rayDir, Vector2.up, 1, LayerMask.GetMask("Platform"));
        
        if (rayHit.collider != null)
        {
            if (rayHit.distance < 0.2f)
            {
                return true;
            }
        } 
        return false;
    }

    /// <summary>
    /// 플레이어가 Ledge에 도달했는지 확인
    /// </summary>
    /// <returns>True: Ledge 도달</returns>
    public bool IsCharacterReachedTop()
    {
        return _playerCollider.bounds.center.y > _currentWall.bounds.max.y;
    }
    
    /// <summary>
    /// Ledge에 도달했을 때 플레이어을 벽 위로 이동시키는 Task
    /// </summary>
    /// <param name="startPos">시작 위치</param>
    /// <param name="targetPos">도착 위치</param>
    public async UniTask MoveToWallTop(Vector3 startPos, Vector3 targetPos)
    {
        float timer = 0f;
        float climbOverDuration = 0.3f;

        while (timer < climbOverDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / climbOverDuration;

            // 커브 기반 보간
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, progress);

            // 포물선 궤적
            float heightBoost = Mathf.Sin(progress * Mathf.PI);
            currentPos.y += heightBoost;

            _playerCollider.transform.position = currentPos;

            // 다음 프레임 대기
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        // 최종 위치 보정
        _playerCollider.transform.position = targetPos;
    }

    /// <summary>
    /// 벽타기 중 점프
    /// </summary>
    public void WallJump()
    {
        Vector2 jumpDir = _player.Movement.GetCharacterSpriteDirection() * (-1);

        if (_player.Controller.MoveInput == Vector2.zero || _player.Controller.ClimbInput != Vector2.zero)
        {
            // (방향키 입력 없이 / 위아래로 움직이면서) 점프 했을 때
            _player.FlipSprite();
            _player.Movement.Move(Vector2.zero);
        }
        else
        {
            // 방향키 입력이 있을 때
            _player.Controller.SetDirection(jumpDir);
        }
        
        _player.Movement.Jump(3.0f, jumpDir);
    }
}
