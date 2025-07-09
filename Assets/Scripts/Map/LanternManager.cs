using System;
using System.Collections.Generic;
using UnityEngine;

public class LanternManager : MonoBehaviour
{
    public List<LanternObject> lanternObjects = new List<LanternObject>();

    public bool autoFindLanterns = true;  // 자동으로 씬의 랜턴들을 찾을지 여부

    private Dictionary<int, LanternAppearance> lanternStates = new Dictionary<int, LanternAppearance>();
    private List<int> activatedLanterns = new List<int>();
    private int currentBigLanternKey = -1;

    void Start()
    {
        InitializeLanterns();
    }

    private void InitializeLanterns()
    {
        // 자동으로 씬의 랜턴들을 찾기
        if (autoFindLanterns)
        {
            LanternObject[] foundLanterns = FindObjectsOfType<LanternObject>();
            lanternObjects.Clear();
            lanternObjects.AddRange(foundLanterns);
        }

        for (int i = 0; i < lanternObjects.Count; i++)
        {
            if (lanternObjects[i] != null)
            {
                lanternObjects[i].SetLanternKey(i);                
            
                lanternObjects[i].Bind(OnLanternInteraction);

                //초기 상태 설정
                lanternStates[i] = LanternAppearance.Off;
                lanternObjects[i].ChangeLanternState(LanternAppearance.Off);
            }
        }
    }

    private void OnLanternInteraction(int lanternKey)
    {
        if (!IsValidLanternKey(lanternKey))
        {
            Debug.LogWarning($"LanternManager: 잘못된 랜턴 키 - {lanternKey}");
            return;
        }

        if (currentBigLanternKey != -1 && currentBigLanternKey != lanternKey)
        {
            SetLanternState(currentBigLanternKey, LanternAppearance.Small);
        }

        SetLanternState(lanternKey, LanternAppearance.Big);
        currentBigLanternKey = lanternKey;

        if (!activatedLanterns.Contains(lanternKey))
        {
            activatedLanterns.Add(lanternKey);
        }

    }

    public void SetLanternState(int lanternKey, LanternAppearance appearance)
    {
        if (!IsValidLanternKey(lanternKey))
        {
            Debug.LogWarning($"LanternManager: 잘못된 랜턴 키 - {lanternKey}");
            return;
        }

        lanternStates[lanternKey] = appearance;
        lanternObjects[lanternKey].ChangeLanternState(appearance);

        Debug.Log($"호롱불 {lanternKey} {appearance}");
    }

    public LanternAppearance GetLanternState(int lanternKey)
    {
        if (!IsValidLanternKey(lanternKey))
        {
            Debug.LogWarning($"LanternManager: 잘못된 랜턴 키 - {lanternKey}");
            return LanternAppearance.Off;
        }

        return lanternStates.ContainsKey(lanternKey) ? lanternStates[lanternKey] : LanternAppearance.Off;
    }

    private bool IsValidLanternKey(int lanternKey)
    {
        return lanternKey >= 0 && lanternKey < lanternObjects.Count && lanternObjects[lanternKey] != null;
    }
}

