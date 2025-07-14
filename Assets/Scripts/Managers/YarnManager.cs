using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using Yarn.Unity;

public class YarnManager : SceneSingleton<YarnManager>
{
    
    [SerializeField] private DialogueRunner runner;
    
    [SerializeField] private Screen dialogueScreen;

    private event Action dialogEnd;

    void Init()
    {
        runner = GameObject.FindAnyObjectByType<DialogueRunner>();
    }
    
    /// <summary>
    /// 타이틀이 <see cref="nodeName"/>인 다이얼로그를 찾아 실행<br/>
    /// 해당 대화 완전 종료시 <see cref="callback"/> 실행
    /// </summary>
    public void RunDialogue(string nodeName, Action callback = null)
    {
        if (runner == null)
        {
            Init();
        }
        runner.Stop();
        runner.StartDialogue(nodeName);
        dialogueScreen.FadeInScreen();
        dialogEnd =  callback;
    }

    /// <summary>
    /// 얀 스크립트에서 대화 종료시 호출해야함. <br/>
    /// 기능: <br/>
    /// 캐릭터 이미지 비활성화 <br/>
    /// 배경 이미지 비활성화 <br/>
    /// notice, like/dislike 텍스트 비활성화 <br/>
    /// 다이얼로그 씬 비활성화 <br/>
    /// 대화 완전 종료 시 실행되는 callback 호출 <br/>
    /// </summary>
    void EndDialogue()
    {
        dialogueScreen.FadeOutScreen();
        dialogEnd?.Invoke();
        dialogEnd = null;
    }
    
}
