using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class CustomLineView : LineView
{
    [System.Serializable]
    public class LineEvent : UnityEvent<LocalizedLine> { }

    public LineEvent onLineStarted = new LineEvent();

    public override void RunLine(LocalizedLine line, System.Action onLineComplete)
    {
        // 여기에 이벤트 호출 추가
        onLineStarted?.Invoke(line);


        // 기존 기능 수행
        base.RunLine(line, onLineComplete);
        StartCoroutine(RunLineCoroutine(line, onLineComplete));
    }

    private IEnumerator RunLineCoroutine(LocalizedLine line, System.Action onLineComplete)
    {
        if (line.Text.Text.StartsWith("->"))
        {
            yield break;
        }
        
        yield return StartCoroutine(WaitForSpace());
        
        onLineComplete?.Invoke();
    }

    private IEnumerator WaitForSpace()
    {
        //Debug.Log("Waiting for space...");
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
    }
   
}
