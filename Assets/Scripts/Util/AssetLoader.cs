using UnityEngine;

namespace Util
{
    /// <summary>
    /// Resources 기반 에셋 로더 – Prefab, 이미지 등. 단순 에셋 로딩에 사용.
    /// </summary>
    public static class AssetLoader
    {
        private class InternalAssetLoader : BaseAssetLoader { }
        private static readonly InternalAssetLoader _impl = new();

        public static T Load<T>(string path) where T : Object => _impl.Load<T>(path);
        public static GameObject Load(string path) => _impl.Load(path);
        public static GameObject Spawn(string path, Transform parent = null) => _impl.Spawn(path, parent);
        public static void ReleaseAll() => _impl.ReleaseAll();
    }
}