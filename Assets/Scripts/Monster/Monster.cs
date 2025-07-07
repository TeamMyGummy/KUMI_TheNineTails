using UnityEngine;
using System.Collections.Generic;
using AbilitySystem.Base;

public class Monster : MonoBehaviour
{
    //인스펙터에서 설정 가능
    public string monsterName = "";
    public string monsterType = "";
    public bool isActive = true; //능동이면 true

    private void Awake()//게임 오브젝트 생성 시 실행
    {
        var Attributes = new Dictionary<string, Attribute>
        //디폴트값
        {
            { "HP", new Attribute(30f, 30f) },
            { "Attack", new Attribute(1f, 1f) },
            { "AttackSpeed", new Attribute(1f, 1f) },
            { "MoveSpeed", new Attribute(0.7f, 0.7f) }
        };

        var ga = GetComponent<GameplayAttribute>();
        //Attributes 세팅
        ga.SetAttribute(Attributes);
    }

   
}
