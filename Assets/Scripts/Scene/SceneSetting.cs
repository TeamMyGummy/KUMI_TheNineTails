using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class SceneSetting : SceneSingleton<SceneSetting>
{
    // Start is called before the first frame update
    public BGMName BGMName;
    void Start()
    {
        SoundManager.Instance.PlayBGM(BGMName);
    }
    
}
