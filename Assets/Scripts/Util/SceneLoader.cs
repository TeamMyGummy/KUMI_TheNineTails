using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Util
{
    public static class SceneLoader
    {
        private static AsyncOperationHandle<SceneInstance>? _currentSceneHandle = null;

        /// <summary>
        /// 씬을 불러옴
        /// 주의) Addressable에 등록되지 않은 씬은 불러오지 못함
        /// </summary>
        /// <param name="scene">Addressables에 저장된 씬 이름</param>
        public static async UniTask LoadSceneAsync(string scene)
        {
            if (_currentSceneHandle.HasValue && _currentSceneHandle.Value.IsValid())
            {
                await Addressables.UnloadSceneAsync(_currentSceneHandle.Value, true);
                _currentSceneHandle = null;
            }

            var handle = Addressables.LoadSceneAsync(scene);
            _currentSceneHandle = handle;
            await handle.ToUniTask();
        }
    }
}