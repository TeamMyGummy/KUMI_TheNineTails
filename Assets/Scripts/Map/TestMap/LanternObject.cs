using System;
using UnityEngine;

public enum LanternAppearance
{
    Off,
    Small,
    Big,
}

//⚠️LanternObject가 존재하는 씬에는 반드시 Lantern이 씬에 존재해야 함
public class LanternObject : MonoBehaviour
{
    private Action<int> Interacted;
    public int LanternKey { get; private set; }

    public void Awake()
    {
    }

    public void Bind(Action<int> interacted)
    {
        Interacted = interacted;
    }
    
    //보여지는 랜턴 이미지 변경
    public void ChangeLanternState(LanternAppearance appearance)
    {
        
    }
    
    //OnTriggerEnter어쩌구... 기타등등올 해서 상호작용 했는지 확인 & Lantern에 알리기(Interacted?.Invoke(LanternKey))
}
