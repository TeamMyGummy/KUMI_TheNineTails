using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        YarnManager.Instance.RunDialogue("Start");
    }

    
}
