using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Util
{
    /// <summary>
    /// Prefab, 이미지, 등의 에셋(데이터 X)을 로드하는 데 사용
    /// 단, 씬에 종속된 에셋(특정 씬에서만 사용하는 에셋)은 SceneAssetLoader를 사용
    /// </summary>
    public static class AssetLoader
    {
        private class InternalAssetLoader : BaseAssetLoader { }
        private static readonly InternalAssetLoader _impl = new();

        public static UniTask<T> Load<T>(string key) where T : UnityEngine.Object => _impl.Load<T>(key);
        public static UniTask<GameObject> Load(string key) => _impl.Load(key);
        public static UniTask<GameObject> Spawn(string key, Transform parent = null) => _impl.Spawn(key, parent);
        public static void ReleaseAll() => _impl.ReleaseAll();
    }
}