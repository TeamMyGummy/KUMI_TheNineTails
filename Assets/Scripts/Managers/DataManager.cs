using System;
using System.Collections.Generic;
using System.IO;
using AbilitySystem.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Util;

namespace Managers
{
    public class DataManager : Singleton<DataManager>
    {
        public PlayerState Player;
        
        public void Save(string key)
        {
            JsonLoader.WriteDynamicData("Player", Player);
        }

        public void Load(string key)
        {
            var data = JsonLoader.ReadDynamicData<PlayerState>("Player");
            if (data is null) data = JsonLoader.ReadStaticData<PlayerState>("Player");
            Player = data;
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