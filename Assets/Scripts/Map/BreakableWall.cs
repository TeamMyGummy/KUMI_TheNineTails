using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    private int _attackCount = 0; 

    public void AttackCount()
    {
        _attackCount += 1;
        
        if(_attackCount == 2)
            Broken();
    }
    
    private void Broken()
    {
        /*Debug.Log("[BreakableWall] is broken");*/
        Destroy(this.gameObject);
    }
}
