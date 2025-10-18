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

    private string _prevCharacterName = "";
    
    private MotionHandle _motionHandle;
    
    private PlayerController _player;

    private event Action dialogEnd;

    

    void Init()
    {
        runner = GameObject.FindAnyObjectByType<DialogueRunner>();
        // 새로운 대사가 시작될 때마다 HandleLineStarted 실행
        lineView.onLineStarted.AddListener(HandleLineStarted);
        
        runner.AddCommandHandler("end", EndDialogue);
        runner.AddCommandHandler<string>("show", ShowCharacter);
        runner.AddCommandHandler<string>("hide", HideCharacter);
        runner.AddCommandHandler<string>("change", ChangeCharacter);
        
        _player = FindAnyObjectByType<PlayerController>();
        
        runner.onDialogueStart.AddListener(() =>
        {
            dialogueCanvas.SetActive(true);
        });
        
        runner.onDialogueComplete.AddListener(() =>
        {
            dialogueCanvas.SetActive(false);
        });
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
        
        dialogue.ApplyDialogueOverlay();
        
        _player.OnDisableMove();

        dialogueScreen.FadeScreen(0.5f, 0f, 0.5f);
        StartCoroutine(WaitSeconds(0.8f));
        runner.StartDialogue(nodeName);
        dialogEnd = callback;
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
        dialogue.RemoveDialogueOverlay();
        _player.OnEnableMove();
        dialogueScreen.FadeScreen(0.5f, 0.5f, 0f);
        dialogEnd?.Invoke();
        dialogEnd = null;
    }

    void ShowCharacter(string spriteName)
    {
        Sprite sprite = ResourcesManager.Instance.Load<Sprite>("Sprites/Characters/" + spriteName);
        characterImage.sprite = sprite;
        characterImage.SetNativeSize();
        
        if (spriteName == "쿠미")
        {
            ShowSide(-710f, -510f);
        }
        else
        {
            ShowSide(710f, 510f);
        }
    }

    void HideCharacter(string spriteName)
    {
        if (spriteName == "쿠미")
        {
            HideSide(-510f, -710f);
        }
        else
        {
            HideSide(510f, 710f);
        }
    }

    void ChangeCharacter(string spriteName)
    {
        Sprite sprite = ResourcesManager.Instance.Load<Sprite>("Sprites/Characters/" + spriteName);
        characterImage.sprite = sprite;
        characterImage.SetNativeSize();
    }

    private void ShowSide(float start, float end)
    {
        RectTransform rect = characterImage.GetComponent<RectTransform>();
        Vector3 startPos = new Vector3(start, rect.anchoredPosition.y, 0f);
        Vector3 endPos = new Vector3(end, rect.anchoredPosition.y, 0f);
        
        rect.anchoredPosition = startPos;
        characterScreen.FadeScreen(0.5f, 0f, 1f);

        rect.DOAnchorPos(endPos, 0.5f).SetEase(Ease.OutCubic);
    }
    
    private void HideSide(float start, float end)
    {
        RectTransform rect = characterImage.GetComponent<RectTransform>();
        Vector3 startPos = new Vector3(start, rect.anchoredPosition.y, 0f);
        Vector3 endPos = new Vector3(end, rect.anchoredPosition.y, 0f);
        
        rect.anchoredPosition = startPos;
        characterScreen.FadeScreen(0.5f, 1f, 0f);

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
