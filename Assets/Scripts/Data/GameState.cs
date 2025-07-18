using System;
using Data;
using Newtonsoft.Json;
using UnityEngine;

//초기화 시 에셋 로드가 필요한가 -> 도메인
//GameState의 값을 직접 변경하지 않음(저장이 필요할 때 객체의 데이터를 긁어옴)
public enum DomainKey{
    Player,
}

//이곳에 저장할 객체에 Action, 등 바인딩이 가능한 정보는 넣으면 안 됨
//당연하지만 객체가 바뀌면 씬 전체를 다시 불러와야 함(=그래야 씬 각각에서 참조하는 객체가 바뀜)
public class GameState
{
    [JsonProperty] private ASCState _ascState;
    [JsonProperty] public SingletonData SingletonData = new();

    public object Get(DomainKey key) => key switch
    {
        DomainKey.Player => _ascState,
        _ => null
    };
    
    public void Set(DomainKey key, object data) {
        try
        {
            switch(key) {
                case DomainKey.Player: _ascState = (ASCState)data; break;
            }
        }
        catch (InvalidCastException ex)
        {
            Debug.LogError("[GameState] 저장하려는 데이터가 Null이거나 캐스팅 타입과 맞지 않습니다. ");
        }
    }
}
