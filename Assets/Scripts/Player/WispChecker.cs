using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispChecker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wisp"))
        {
            Wisp wisp = other.GetComponent<Wisp>();
            if(!wisp.IsTargetEqualPlayer())
                wisp.ChangeTargetToPlayer();
        }
    }
}
