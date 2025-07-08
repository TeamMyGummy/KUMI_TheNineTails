using UnityEngine;
using Util;

public class TestMapScene : SceneSingleton<TestMapScene>
{
    public TestMapState State;
    new void Awake()
    {
        base.Awake();
        DomainFactory.Instance.GetState(StateKey.TestMap, out State);
    }
}
