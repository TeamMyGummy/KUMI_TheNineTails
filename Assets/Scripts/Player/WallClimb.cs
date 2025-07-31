using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class WallClimb
{
    private GameObject _actor;
    private Collider2D _actorCollider;
    private Rigidbody2D _actorRigidbody2D;
    private CharacterMovement _characterMovement;
    
    private GameObject _currentWall;
    private Bounds _wallBounds;
    private WallClimbState _currentState;
    
    public static event System.Action WallClimbEndAnimation;
    
    public enum WallClimbState
    {
        Idle,
        Climbing,
        Gripping,
        ReachedTop
    }

    public WallClimb(GameObject actor)
    {
        this._actor = actor;
        this._characterMovement = actor.GetComponent<CharacterMovement>();
        this._actorCollider = actor.GetComponent<Collider2D>();
        this._actorRigidbody2D = actor.GetComponent<Rigidbody2D>();
    }
    
    public void SetWallClimbState(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            // Idle
            ChangeState(WallClimbState.Idle);
        }
        else if (direction.x != 0 && direction.y == 0)
        {
            // Gripping
            ChangeState(WallClimbState.Gripping);
        }
        else
        {
            // Climbing
            ChangeState(WallClimbState.Climbing);
        }
    }
    
    public void OnReachedTop()
    {
        ChangeState(WallClimbState.ReachedTop);
        MovePosition().Forget();
        WallClimbEndAnimation?.Invoke();
    }

    private async UniTask MovePosition()
    {
        // 애니메이션에 맞추어 캐릭터 움직이기
        // x + 캐릭터 사이즈, y + 캐릭터 사이즈
        Bounds actorBounds = _actorCollider.bounds;
        Vector2 spriteDir = _characterMovement.GetCharacterSpriteDirection();
        
        Vector2 offset = new Vector2(actorBounds.size.x * spriteDir.x, _wallBounds.max.y - actorBounds.min.y + 0.02f);
        
        await UniTask.Delay(583);   // 애니메이션 길이
        
        //Debug.Log("위치 이동");
        _actorRigidbody2D.position += offset;
    }
    
    // 벽의 높이를 기준으로 캐릭터 위치 판단
    public bool IsCharacterReachedTop()
    {
        Bounds actorBounds = _actorCollider.bounds;
        
        // 캐릭터 사이즈 만큼 남겨두고 벽을 다 올랐을 때 (발 위치 기준)
        float topY = _wallBounds.max.y - actorBounds.size.y;
        return actorBounds.min.y >= topY;
    }

    // 현재 벽 설정
    public void SetCurrentWall(GameObject wall)
    {
        _currentWall = wall;
        _wallBounds =  _currentWall.GetComponent<Collider2D>().bounds;
    }
    
    private void ChangeState(WallClimbState state)
    {
        _currentState = state;
    }
    
    public WallClimbState GetState()
    {
        return  _currentState;
    }

    public void Reset()
    {
        ChangeState(WallClimbState.Idle);
    }
}
