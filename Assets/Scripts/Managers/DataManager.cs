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
    public static class DataManager
    {
        public static void Save(string key)
        {
            foreach (var domain in DomainFactory.Instance.GetAllDomains())
            {
                SaveData(domain.Key, domain.Value.Save());
            }
        }

        public static T Load<T>(SaveKey key)
        {
            return LoadData<T>(key.ToString());
        }

        private static void SaveData<T>(SaveKey key, T data)
        {
            JsonLoader.WriteDynamicData(key.ToString(), data);
        }

        private static void LoadData<T>(string key, out T container)
        {
            var data = JsonLoader.ReadDynamicData<T>(key);
            if (data is null) data = JsonLoader.ReadStaticData<T>(key);
            container = data;
        }
        
        private static T LoadData<T>(string key)
        {
            var data = JsonLoader.ReadDynamicData<T>(key);
            if (data is null) data = JsonLoader.ReadStaticData<T>(key);
            return data;
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