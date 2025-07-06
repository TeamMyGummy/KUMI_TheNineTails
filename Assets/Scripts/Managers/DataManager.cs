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
                JsonLoader.WriteDynamicData(domain.Key.ToString(), domain.Value.Save());
            }
        }

        public static T Load<T>(SaveKey key)
        {
            string saveKey = key.ToString();
            var data = JsonLoader.ReadDynamicData<T>(saveKey);
            if (data is null) data = JsonLoader.ReadStaticData<T>(saveKey);
            return data;
        }
    }
}