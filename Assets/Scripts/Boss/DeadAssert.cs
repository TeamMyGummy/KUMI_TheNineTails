using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadAssert : MonoBehaviour
{
    public SpineAnimationHandler SpineAnimationHandler;
    public BTController controller;
    public BTContext context;
    public bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(isDead == false)
        {
            //HP 및 BT 종료 여부 체크 후 만약에 HP 0인데 이무기 안 죽으면 걍제로 코드에서 BT 종료시키고 종료 수행

            if(context.ASC.Attributes["HP"].Value <= 0 && controller.isRunning)
            {
                isDead = true;
                controller.StopAI();
                SpineAnimationHandler.SetAnimation("death");
                SoundManager.Instance.PlaySFX(SFXName.이무기_보스_사망);
            }
        }
    }
}
