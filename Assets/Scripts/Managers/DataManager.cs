using System;
using System.Collections.Generic;
using System.IO;
using AbilitySystem.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Util;

namespace Managers
{
    /// <summary>
    /// 250705 기준) 저장 프로세스는 PR #10 참고
    /// </summary>
    public class DataManager : Singleton<DataManager>
    {
        public PlayerState Player;
        
        public void Save(string key)
        {
            SaveData("Player", Player);
        }

        public void Load(string key)
        {
            LoadData("Player", out Player);
        }

        private void SaveData<T>(string key, T data)
        {
            JsonLoader.WriteDynamicData(key, data);
        }

        private void LoadData<T>(string key, out T container)
        {
            var data = JsonLoader.ReadDynamicData<T>(key);
            if (data is null) data = JsonLoader.ReadStaticData<T>(key);
            container = data;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void EnsureGlobalLifetimeScope()
        {
            if (FindObjectOfType<DataManager>() == null)
            {
                var go = new GameObject("DataManager");
                go.AddComponent<DataManager>();
                DataManager.Instance.Load(String.Empty);
            }
        }
    }
}