using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonTest : MonoBehaviour
{
    public string nextScene;
    public void ToB2BossScene()
    {
        SceneManager.LoadScene(nextScene);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ToB2BossScene();
    }
}
