using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour
{
    public SpineAnimationHandler animationHandler;
    public void GetParried()
    {
        animationHandler.SetAnimation("parry_tail");
    }
}
