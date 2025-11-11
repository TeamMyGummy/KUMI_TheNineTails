// UI_Glow.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Glow : MonoBehaviour
{
    private Material glowMaterial; 
    private Image image;
    
    private static readonly int GlowIntensityID = Shader.PropertyToID("_GlowIntensity"); 

    [Header("반짝하는 시간")]
    public float glowDuration = 0.3f;
    [Header("최대 밝기 조정")]
    public float maxIntensity = 1.0f;
    [Header("반짝 그래프.. 뭐라하지")]
    public AnimationCurve glowCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); 

    void Awake()
    {
        image = GetComponent<Image>();
        if (image.material == null)
        {
            Debug.LogError($"[UI_Glow] {gameObject.name}에 머티리얼이 없습니다!", this);
            return;
        }
        glowMaterial = new Material(image.material); 
        image.material = glowMaterial; 

        glowMaterial.SetFloat(GlowIntensityID, 0f);
    }

    public void PlayGlow()
    {
        if (glowMaterial == null) return;
        StopAllCoroutines(); 
        StartCoroutine(GlowCoroutine());
    }

    private IEnumerator GlowCoroutine()
    {
        float timer = 0f;

        while (timer < glowDuration)
        {
            float progress = timer / glowDuration; 
            float curveValue = glowCurve.Evaluate(progress); 
            float intensity = curveValue * maxIntensity; 
            
            glowMaterial.SetFloat(GlowIntensityID, intensity);
            
            timer += Time.deltaTime;
            yield return null;
        }
        
        glowMaterial.SetFloat(GlowIntensityID, 0f); 
    }
}