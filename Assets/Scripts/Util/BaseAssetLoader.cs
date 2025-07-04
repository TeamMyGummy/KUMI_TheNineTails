using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Util
{
    /// <summary>
    /// Addressables 에셋 로더 – 핸들 기반 캐싱 구조
    /// </summary>
    public abstract class BaseAssetLoader
    {
        private readonly Dictionary<string, AsyncOperationHandle> _handles = new();

        /// <summary>
        /// 에셋 로드
        /// </summary>
        /// <param name="key">Addressable Key</param>
        /// <typeparam name="T">UnityEngine.Object 타입</typeparam>
        /// <returns>로드된 에셋</returns>
        public async UniTask<T> Load<T>(string key) where T : UnityEngine.Object
        {
            if (_handles.TryGetValue(key, out var handle))
                return (T)handle.Result;

            var newHandle = Addressables.LoadAssetAsync<T>(key);
            await newHandle.ToUniTask();

            _handles[key] = newHandle;
            return newHandle.Result;
        }

        /// <summary>
        /// GameObject 전용 에셋 로드
        /// </summary>
        /// <param name="key">Addressable Key</param>
        /// <returns>로드된 GameObject</returns>
        public async UniTask<GameObject> Load(string key)
        {
            return await Load<GameObject>(key);
        }

        /// <summary>
        /// GameObject를 씬에 인스턴스
        /// (오브젝트 풀링 고려 가능)
        /// </summary>
        /// <param name="key">프리팹 키</param>
        /// <param name="parent">부모 트랜스폼</param>
        /// <returns>인스턴스화된 GameObject</returns>
        public async UniTask<GameObject> Spawn(string key, Transform parent = null)
        {
            var prefab = await Load<GameObject>(key);
            if (prefab != null)
                return Object.Instantiate(prefab, parent);

            Debug.LogError($"[강승연] {key} 로드 실패로 Instantiate 불가");
            return null;
        }

        /// <summary>
        /// 모든 캐싱된 에셋 릴리즈
        /// </summary>
        public void ReleaseAll()
        {
            foreach (var handle in _handles.Values)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }

            _handles.Clear();
        }
    }
}
