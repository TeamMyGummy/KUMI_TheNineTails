using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Yarn;
using Yarn.Unity;
using Ease = DG.Tweening.Ease;

public class YarnManager : SceneSingleton<YarnManager>
{

    [SerializeField] private DialogueRunner runner;

    [SerializeField] private Screen dialogueScreen;

    [SerializeField] private CustomLineView lineView;
    
    [SerializeField] private Image characterImage;
    [SerializeField] private Screen characterScreen;
    [SerializeField] private UI_Dialogue dialogue;
    [SerializeField] private GameObject dialogueCanvas;

    [SerializeField] private float kumiPosY;
    [SerializeField] private float kkebiPosY;

    private string _prevCharacterName = "";
    
    private MotionHandle _motionHandle;
    
    private PlayerController _player;

    private event Action dialogEnd;

    

    void Init()
    {
        runner = GameObject.FindAnyObjectByType<DialogueRunner>();
        // 새로운 대사가 시작될 때마다 HandleLineStarted 실행
        lineView.onLineStarted.AddListener(HandleLineStarted);
        
        runner.AddCommandHandler("end", StopDialogue);
        runner.AddCommandHandler<string>("show", ShowCharacter);
        runner.AddCommandHandler<string>("hide", HideCharacter);
        runner.AddCommandHandler<string>("change", ChangeCharacter);
        
        _player = FindAnyObjectByType<PlayerController>();
        
        runner.onDialogueComplete.AddListener(HandleDialogueCompletion);
    }

    /// <summary>
    /// 타이틀이 <see cref="nodeName"/>인 다이얼로그를 찾아 실행<br/>
    /// 해당 대화 완전 종료시 <see cref="callback"/> 실행
    /// </summary>
    public void RunDialogue(string nodeName, Action callback = null)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            Debug.Log("good");
        }
        
        if (runner == null)
        {
            Init();
        }

        runner.Stop();
        
        dialogue.ApplyDialogueOverlay();
        
        _player.OnDisableMove();
        
        dialogueCanvas.SetActive(true);
        dialogueScreen.gameObject.SetActive(true);

        dialogueScreen.FadeScreen(0.5f, 0.5f);
        StartCoroutine(WaitSeconds(0.8f));
        runner.StartDialogue(nodeName);
        dialogEnd = callback;
    }
    
    /// <summary>
    /// Yarn 스크립트의 <<end>> 커맨드에 의해 호출됩니다.
    /// 단순히 DialogueRunner를 중지시킵니다.
    /// </summary>
    void StopDialogue()
    {
        if (runner.IsDialogueRunning)
        {
            runner.Stop();
        }
    }
    
    /// <summary>
    /// 스킵 버튼(UI)에서 호출할 공용 메소드입니다.
    /// </summary>
    public void SkipDialogue()
    {
        // 타이핑 중이 아니라면 대화 즉시 중지
        if (runner.IsDialogueRunning)
        {
            runner.Stop();
        }
    }
    
    /// <summary>
    /// 대화가 완료되거나 Stop()으로 중지될 때 공통으로 호출되는 클린업 함수입니다.
    /// (기존 EndDialogue와 onDialogueComplete 리스너의 로직을 합쳤습니다)
    /// </summary>
    private void HandleDialogueCompletion()
    {
        dialogue.RemoveDialogueOverlay();

        if (_player != null) // 안전을 위해 null 체크
        {
            _player.OnEnableMove();
        }

        dialogueScreen.FadeScreen(0.5f, 0f);
        
        dialogEnd?.Invoke();
        dialogEnd = null;
        
        dialogueCanvas.SetActive(false);
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
        StopDialogue();
    }

    void ShowCharacter(string spriteName)
    {
        Sprite sprite = ResourcesManager.Instance.Load<Sprite>("Sprites/Characters/" + spriteName);
        characterImage.sprite = sprite;
        characterImage.SetNativeSize();
        
        if (spriteName == "쿠미")
        {
            ShowSide(-710f, -510f, kumiPosY);
        }
        else
        {
            ShowSide(710f, 510f, kkebiPosY);
        }
    }

    void HideCharacter(string spriteName)
    {
        if (spriteName == "쿠미")
        {
            HideSide(-510f, -710f, kumiPosY);
        }
        else
        {
            HideSide(510f, 710f, kkebiPosY);
        }
    }

    void ChangeCharacter(string spriteName)
    {
        Sprite sprite = ResourcesManager.Instance.Load<Sprite>("Sprites/Characters/" + spriteName);
        characterImage.sprite = sprite;
        characterImage.SetNativeSize();
    }

    private void ShowSide(float start, float end, float posY)
    {
        RectTransform rect = characterImage.GetComponent<RectTransform>();
        Vector3 startPos = new Vector3(start, posY, 0f);
        Vector3 endPos = new Vector3(end, posY, 0f);
        
        rect.anchoredPosition = startPos;
        characterScreen.FadeScreen(0.5f, 1f);

        rect.DOAnchorPos(endPos, 0.5f).SetEase(Ease.OutCubic);
    }
    
    private void HideSide(float start, float end, float posY)
    {
        RectTransform rect = characterImage.GetComponent<RectTransform>();
        Vector3 startPos = new Vector3(start, posY, 0f);
        Vector3 endPos = new Vector3(end, posY, 0f);
        
        rect.anchoredPosition = startPos;
        characterScreen.FadeScreen(0.5f, 0f);

        rect.DOAnchorPos(endPos, 0.5f).SetEase(Ease.OutCubic);
    }

    private IEnumerator WaitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    private void HandleLineStarted(LocalizedLine line)
    {
        _prevCharacterName = line.CharacterName;
    }
    
}
