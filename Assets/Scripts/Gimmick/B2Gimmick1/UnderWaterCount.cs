using System;
using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;

public class UnderWaterCount : MonoBehaviour
{
    public Collider2D playerFaceCollider; 

    public List<GameObject> waterTimerUI = new List<GameObject>();
    public GameObject waterTimerUIGroup;
    public float timePerPop = 1f;
    
    public CharacterMovement characterMovement;
    private Jump _jumpAbility;

    private Coroutine drowningCoroutine; 
    private float originalPlayerSpeed;
    public float underwaterSpeedMultiplier = 0.1f; //수중 플레이어 속도 배율
    public float underwaterJumpPowerMultiplier = 0.5f; // 수중 플레이어 점프 파워 배율
    public float underwaterGravityMultiplier = 0.5f; // 수중 플레이어 중력 배율
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == playerFaceCollider)
        {
            OnEnterWater();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == playerFaceCollider)
        {
            OnExitWater();
        }
    }

    /// <summary>
    /// 물에 잠기기 시작했을 때 호출
    /// </summary>
    private void OnEnterWater()
    {
        if (drowningCoroutine != null) return;
        Debug.Log("물에 잠김");
        if (characterMovement != null)
        {
            // 속도 설정
            originalPlayerSpeed = characterMovement.Speed;
            characterMovement.SetSpeed(originalPlayerSpeed * underwaterSpeedMultiplier);
            
            // 점프 높이 설정
            if (_jumpAbility == null)
            {
                AbilitySystem asc = characterMovement.gameObject.GetComponent<Player>().ASC;
                _jumpAbility = (asc.IsGranted(AbilityKey.DoubleJump) ? asc.GetAbility(AbilityKey.DoubleJump) : asc.GetAbility(AbilityKey.Jump)) as Jump;
                Debug.Assert(_jumpAbility != null);
            }
            _jumpAbility.SetJumpPower(_jumpAbility.GetMaxJumpPower() * underwaterJumpPowerMultiplier);
            
            // 중력 설정
            characterMovement.SetGravityScale(characterMovement.GetGravityScale() * underwaterGravityMultiplier);
        }
        
        if (waterTimerUIGroup != null)
        {
            waterTimerUIGroup.SetActive(true);
        }
        foreach (GameObject waterDrop in waterTimerUI)
        {
            if (waterDrop != null)
            {
                waterDrop.SetActive(true);
            }
        }
        drowningCoroutine = StartCoroutine(DrowningProcess());
    }

    /// <summary>
    /// 물에서 나왔을 때 호출
    /// </summary>
    private void OnExitWater()
    {
        Debug.Log("물에서 나옴");
        if (drowningCoroutine != null)
        {
            StopCoroutine(drowningCoroutine);
            drowningCoroutine = null;
        }
        if (characterMovement != null)
        {
            //속도 복구
            characterMovement.SetSpeed(originalPlayerSpeed); 
            
            // 점프 높이 복구
            _jumpAbility.ResetJumpPower();
            
            // 중력 복구
            characterMovement.ResetGravityScale();
        }
        if (waterTimerUIGroup != null)
        {
            waterTimerUIGroup.SetActive(false);
        }
    }

    //수중 타이머 코루틴
    private IEnumerator DrowningProcess()
    {
        for (int i = 0; i < waterTimerUI.Count; i++)
        {
            yield return new WaitForSeconds(timePerPop);
            
            if (waterTimerUI[i] != null)
            {
                waterTimerUI[i].SetActive(false);
                Debug.Log($"{i + 1}번째 물방울이 터졌습니다.");
            }
        }
        yield return new WaitForSeconds(timePerPop);
        PlayerDrown();
    }

    //익사
    private void PlayerDrown()
    {
        drowningCoroutine = null; 
        if (waterTimerUIGroup != null)
        {
            waterTimerUIGroup.SetActive(false);
        }

        Debug.Log("꼬르륵.. 사망");
        
        // TODO:실제 데미지... 사망 로직 넣기
    }
}