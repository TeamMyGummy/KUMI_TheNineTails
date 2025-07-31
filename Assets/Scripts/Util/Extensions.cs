using System;
using UnityEngine;

public static class Extensions
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static void Move(this GameObject go, Vector3 position)
    {
        go.transform.position = position;
    }

    public static int StringToInt(this String str)
    {
        int result = 0;
        foreach (char c in str)
        {
            result = result * 256 + c;
        }
        return result;
    }
}
