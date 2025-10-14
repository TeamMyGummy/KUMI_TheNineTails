using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class SceneChangeObject : MonoBehaviour
{
    [Header("이동할 씬 이름")]
    [SerializeField] private string nextScene;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 닿았을 때만 이동
        if (!other.CompareTag("Player")) return;

        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogWarning("이동할 씬 이름이 비어 있습니다.");
            return;
        }

        SceneManager.LoadScene(nextScene);
    }
}