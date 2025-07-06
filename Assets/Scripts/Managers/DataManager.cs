using System;
using System.Collections.Generic;
using System.IO;
using AbilitySystem.Base;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using Util;

namespace Managers
{
    /// <summary>
    /// 250705 기준) 저장 프로세스는 PR #10 참고
    /// </summary>
    [DefaultExecutionOrder(-50)]
    public class DataManager : Singleton<DataManager>
    {
        public new void Awake()
        {
            base.Awake();
            Load(String.Empty);
        }
        public void Save(string key)
        {
            SaveData("Player", DomainFactory.Instance.PlayerASC.Save());
        }

        public void Load(string key)
        {
            DomainFactory.Instance.PlayerASC.Load(LoadData<ASCState>("Player"));
        }

        private void SaveData<T>(string key, T data)
        {
            JsonLoader.WriteDynamicData(key, data);
        }

        private void LoadData<T>(string key, out T container)
        {
            var data = JsonLoader.ReadDynamicData<T>(key);
            if (data is null) data = JsonLoader.ReadStaticData<T>(key);
            container = default;
        }
        
        private T LoadData<T>(string key)
        {
            var data = JsonLoader.ReadDynamicData<T>(key);
            if (data is null) data = JsonLoader.ReadStaticData<T>(key);
            return default;
        }
        
        /*
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void EnsureGlobalLifetimeScope()
        {
            if (FindObjectOfType<DataManager>() == null)
            {
                var go = new GameObject("DataManager");
                go.AddComponent<DataManager>();
                DontDestroyOnLoad(go);
            }
        }*/
    }
}