using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;

public class UI_LogPrint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private int maxLines = 2;
    
    private StringBuilder logBuilder = new StringBuilder();

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // 로그 추가
        logBuilder.AppendLine(logString);
        
        // 줄 수 제한
        string[] lines = logBuilder.ToString().Split('\n');
        if (lines.Length > maxLines)
        {
            logBuilder.Clear();
            int start = lines.Length - maxLines;
            for (int i = start; i < lines.Length; i++)
                logBuilder.AppendLine(lines[i]);
        }

        // UI 업데이트
        logText.text = logBuilder.ToString();
    }
}
