using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class BreakableWall : MonoBehaviour
{
    private int _attackCount = 0;
    private const int Maxhit = 2;
    private int _breakableWallKey;

    private void Awake()
    {
        _breakableWallKey = SceneLoader.GetCurrentSceneName().StringToInt() + transform.GetSiblingIndex();
        
        //파괴된 벽 --> 시작할 때 삭제 
        if (DomainFactory.Instance.Data.BreakableWallState.BrokenWalls.Contains(_breakableWallKey))
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 일반공격 2번(Maxhit) --> 벽 파괴, 파괴된 벽 정보 저장 
    /// </summary>
    public void AttackCount()
    {
        _attackCount += 1;
        
        if(_attackCount == Maxhit)
            Broken();
    }
    
    private void Broken()
    {
        DomainFactory.Instance.Data.BreakableWallState.BrokenWalls.Add(_breakableWallKey);
        DomainFactory.Instance.SaveGameData();
        Destroy(this.gameObject);
    }
}