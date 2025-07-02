using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Util
{
    /// <summary>
    /// 전역용/씬 전용에서 메인 로직 추출해 중복 방지
    /// </summary>
    public abstract class BaseAssetLoader
    {
        protected readonly Dictionary<string, UnityEngine.Object> _cache = new();
        protected readonly Dictionary<string, GameObject> _prefabCache = new();

        /// <summary>
        /// 에셋 로드
        /// </summary>
        /// <param name="key">에셋 key</param>
        /// <returns>로드한 에셋</returns>
        public async UniTask<T> Load<T>(string key) where T : UnityEngine.Object
        {
            if (_cache.TryGetValue(key, out var cached))
                return (T)cached;

            var asset = await Addressables.LoadAssetAsync<T>(key).ToUniTask();
            _cache[key] = asset;
            return asset;
        }

        /// <summary>
        /// 에셋 로드
        /// </summary>
        /// <param name="key">에셋 key</param>
        /// <returns>로드한 에셋</returns>
        public async UniTask<GameObject> Load(string key)
        {
            if (_prefabCache.TryGetValue(key, out var cached))
                return cached;

            var asset = await Addressables.LoadAssetAsync<GameObject>(key).ToUniTask();
            _prefabCache[key] = asset;
            return asset;
        }

        /// <summary>
        /// 로드한 에셋을 실제 씬에 스폰
        /// +a) 오브젝트 풀링으로 최적화 가능
        /// </summary>
        /// <param name="key">에셋 키</param>
        /// <param name="parent">에셋의 부모 오브젝트 Transform</param>
        /// <returns>스폰된 오브젝트</returns>
        public async UniTask<GameObject> Spawn(string key, Transform parent = null)
        {
            await Load(key);
            return Instantiate(key, parent);
        }

        #region private
        /// <summary>
        /// 내부 Instantiate – Load 이후에만 호출해야 함
        /// </summary>
        private GameObject Instantiate(string key, Transform parent = null)
        {
            if (_prefabCache.TryGetValue(key, out var prefab))
                return GameObject.Instantiate(prefab, parent);

            Debug.LogError($"[강승연] 에셋이 캐싱되지 않음 - Spawn으로 호출했음에도 Load 시 에셋이 캐싱되지 않는 문제 발생");
            return null;
        }
        #endregion

        /// <summary>
        /// 캐싱된 에셋을 릴리즈
        /// </summary>
        public void ReleaseAll()
        {
            foreach (var cached in _cache)
            {
                Addressables.Release(cached.Value);
            }
            foreach (var cached in _prefabCache)
            {
                Addressables.Release(cached.Value);
            }

            _cache.Clear();
            _prefabCache.Clear();
        }
    }
}
