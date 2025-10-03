using UnityEngine;

    public static class DebugUtil
    {
        public static void AssertLog()
        {
            #if UNITY_EDITOR
            Debug.Log("Assert Fail");
            #endif
        }
        
        public static void Log(string comment)
        {
#if UNITY_EDITOR
            //Debug.Log(comment);
#endif
        }
    }
