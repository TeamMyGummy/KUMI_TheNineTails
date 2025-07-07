/*using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Util
{
    /// <summary>
    /// Prefab, 이미지, 등의 에셋(데이터 X)을 로드하는 데 사용
    /// 단, 씬에 종속되지 않은 에셋(여러 씬에서 사용하는 에셋 = Addressable에서 씬 단위 그룹에 포함되는 에셋)은 AssetLoader를 사용
    /// </summary>
    [DefaultExecutionOrder(9999)] //Awake에서 호출하면 안 됨(Start에서 호출하는 게 안전함)
    public class SceneAssetLoader : SceneSingleton<SceneAssetLoader>
    {
        private class InternalAssetLoader : BaseAssetLoader { }
        private readonly InternalAssetLoader _impl = new();
        
        public UniTask<T> Load<T>(string key) where T : UnityEngine.Object => _impl.Load<T>(key);
        public UniTask<GameObject> Load(string key) => _impl.Load(key);
        public UniTask<GameObject> Spawn(string key, Transform parent = null) => _impl.Spawn(key, parent);
        public void ReleaseAll() => _impl.ReleaseAll();

        private void OnDestroy()
        {
            ReleaseAll();
        }
    }
}*/