using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Util
{
    public static class SceneLoader
    {
        /// <summary>
        /// 씬을 불러옴 <br/>
        /// 주의) SceneBuilder에 등록해야 함
        /// </summary>
        public static void LoadScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        /// <summary>
        /// 씬을 비동기로 불러옴 <br/>
        /// 주의) SceneBuilder에 등록해야 함
        /// </summary>
        public static async UniTask LoadSceneAsync(string scene)
        {
            await SceneManager.LoadSceneAsync(scene);
        }
            
    }
}