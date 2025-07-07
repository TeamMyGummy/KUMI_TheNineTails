using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    /// <summary>
    /// Resources 에셋 로더 – 핸들 기반 캐싱 구조
    /// </summary>
    public abstract class BaseAssetLoader
    {
        private readonly Dictionary<string, Object> _cache = new();

        /// <summary>
        /// 에셋 동기 로드
        /// </summary>
        public T Load<T>(string path) where T : Object
        {
            if (_cache.TryGetValue(path, out var cached))
                return cached as T;

            var asset = Resources.Load<T>(path);
            if (asset != null)
            {
                _cache[path] = asset;
                return asset;
            }

            Debug.LogError($"[AssetLoader] {path} 로드 실패 (Resources 경로 확인)");
            return null;
        }

        /// <summary>
        /// GameObject 전용 로드
        /// </summary>
        public GameObject Load(string path) => Load<GameObject>(path);

        /// <summary>
        /// GameObject 인스턴스화
        /// </summary>
        public GameObject Spawn(string path, Transform parent = null)
        {
            var prefab = Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"[AssetLoader] {path} 로드 실패로 Instantiate 불가");
                return null;
            }

            return Object.Instantiate(prefab, parent);
        }

        /// <summary>
        /// 캐시 초기화 (필요 시 수동으로 호출)
        /// </summary>
        public void ReleaseAll()
        {
            _cache.Clear(); // Resources는 명시적으로 해제하지 않아도 됨
        }
    }
}