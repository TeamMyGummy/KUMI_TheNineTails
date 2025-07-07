using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Util
{
    public class JsonLoader
    {
        private static readonly string StaticDataWritePath = "StaticJson/";
        private static readonly string DynamicDataPath = Application.persistentDataPath + "/";

        /// <summary>
        /// 정적 데이터를 T 오브젝트로 반환. <br/>
        /// Addressable에 추가해야 함
        /// 그룹 : Default ~~ (만약 특정 씬에서만 쓰이면 그 씬의 그룹)
        /// </summary>
        /// <typeparam name="T">읽어올 오브젝트의 클래스 <br/>
        /// Entity 클래스를 상속받은 클래스만 읽어올 수 있음
        /// </typeparam>
        /// <param name="key"> 데이터 저장명 <br/>
        /// 이 경우 클래스명query(Ex. 클래스명 = ItemTable, query = SinAsan면 ItemTableSinAsan이 파일명인 데이터를 읽어옴</param>
        /// <returns></returns>
        public static T ReadStaticData<T>(string key)
        {
            try
            {
                if (String.IsNullOrEmpty(key)) key = typeof(T).Name;
                TextAsset jsonData = Resources.Load<TextAsset>(StaticDataWritePath + key);
                T data = JsonConvert.DeserializeObject<T>(jsonData.text);
                return data;
            }
            catch (Exception e)
            {
                Debug.Log($"StaticData가 존재하지 않습니다! {StaticDataWritePath + key}");
                return default;
            }
        }

        /// <summary>
        /// 정적 데이터를 Read하기 편하도록 데이터를 생성하는 함수 <br/>
        /// 개발 시 사용하는 것이 아닌 데이터 생성 용도로 사용하는 것
        /// 위치 : Assets/Resources/CreatedJson/
        /// </summary>
        /// <param name="data">데이터</param>
        /// <param name="query"> Read 시 사용할 추가 쿼리 </param>
        /// <typeparam name="T"> 데이터 타입 </typeparam>
        public static void CreateStaticData<T>(T data, string query = "")
        {
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(StaticDataWritePath + typeof(T).Name + query + ".json", jsonData);
        }

        /// <summary>
        /// 동적 데이터를 T 오브젝트로 반환. <br/>
        /// </summary>
        /// <param name="name">읽어올 파일명<br/>
        /// enum DynamicData에 정의 후 사용 가능</param>
        /// <typeparam name="T">읽어올 오브젝트의 클래스</typeparam>
        /// <returns></returns>
        public static T ReadDynamicData<T>(string name)
        {
            try
            {
                string jsonData = File.ReadAllText(DynamicDataPath + name + ".json");
                T data = JsonConvert.DeserializeObject<T>(jsonData);
                return data;
            }
            catch (FileNotFoundException ex)
            {
                Debug.Log($"DynamicData가 존재하지 않습니다! {DynamicDataPath + name + ".json"}");
                return default;
            }
        }

        /// <summary>
        /// 동적 데이터를 저장합니다.
        /// </summary>
        /// <param name="name">저장할 파일명 </param>
        /// <param name="data">저장할 오브젝트 </param>
        /// <typeparam name="T">저장할 오브젝트의 클래스</typeparam>
        /// 
        public static void WriteDynamicData<T>(string name, T data)
        {
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(DynamicDataPath + name + ".json", jsonData);
        }

        public static string GetDynamicDataPath(string key)
        {
            return Path.Combine(DynamicDataPath, key + ".json");
        }
        
        
        public static bool Exists(string key)
        {
            string path = GetDynamicDataPath(key);
            return File.Exists(path);
        }
    }
}
