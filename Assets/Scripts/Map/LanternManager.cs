using System;
using System.Collections.Generic;
using UnityEngine;

public class LanternManager : MonoBehaviour
{
    public List<LanternObject> lanternObjects = new List<LanternObject>();

    public bool autoFindLanterns = true;  // �ڵ����� ���� ���ϵ��� ã���� ����

    private Dictionary<int, LanternAppearance> lanternStates = new Dictionary<int, LanternAppearance>();
    private List<int> activatedLanterns = new List<int>();
    private int currentBigLanternKey = -1;

    void Start()
    {
        InitializeLanterns();
    }

    private void InitializeLanterns()
    {
        // �ڵ����� ���� ���ϵ��� ã��
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

                //�ʱ� ���� ����
                lanternStates[i] = LanternAppearance.Off;
                lanternObjects[i].ChangeLanternState(LanternAppearance.Off);
            }
        }
    }

    private void OnLanternInteraction(int lanternKey)
    {
        if (!IsValidLanternKey(lanternKey))
        {
            Debug.LogWarning($"LanternManager: �߸��� ���� Ű - {lanternKey}");
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
            Debug.LogWarning($"LanternManager: �߸��� ���� Ű - {lanternKey}");
            return;
        }

        lanternStates[lanternKey] = appearance;
        lanternObjects[lanternKey].ChangeLanternState(appearance);

        Debug.Log($"ȣ�պ� {lanternKey} {appearance}");
    }

    public LanternAppearance GetLanternState(int lanternKey)
    {
        if (!IsValidLanternKey(lanternKey))
        {
            Debug.LogWarning($"LanternManager: �߸��� ���� Ű - {lanternKey}");
            return LanternAppearance.Off;
        }

        return lanternStates.ContainsKey(lanternKey) ? lanternStates[lanternKey] : LanternAppearance.Off;
    }

    private bool IsValidLanternKey(int lanternKey)
    {
        return lanternKey >= 0 && lanternKey < lanternObjects.Count && lanternObjects[lanternKey] != null;
    }
}

