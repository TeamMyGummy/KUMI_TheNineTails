using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BGMName
{
    이무기,
    B2,
    B2_Gimmick2,
}

public enum SFXName
{
    달리기,
    점프,
    착지,
    머드걷기,
    머드착지,
    벽에붙음,
    벽타기,
    줄타기,
    대쉬,
    공격1,
    공격2,
    공격3,
    패링,
    여우불,
    간빼기,
    피격,
    죽음,
    몬스터피격,
    이무기등장,
    이무기공격1,
    이무기공격2,
    이무기공격3,
}

[System.Serializable]
public class BGMSound
{
    public BGMName name;
    public AudioClip clip;
    public bool loop = false;
    
    [Range(0f, 1f)] 
    public float volume = 1f;
    
    [HideInInspector]
    public AudioSource source;
}
[System.Serializable]
public class SFXSound
{
    public SFXName name;
    public AudioClip clip;
    public bool loop = false;
    
    [Range(0f, 1f)]
    public float volume = 1f;
    
    [HideInInspector]
    public AudioSource source;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    public AudioSource BGMSource => bgmSource;
    [SerializeField] private AudioSource[] sfxSources;
    
    [Header("Sound Lists")]
    [SerializeField] private BGMSound[] bgmSounds;
    [SerializeField] private SFXSound[] sfxSounds;
    private Queue<AudioSource> availableSFXSources;
    
    private Dictionary<BGMName, BGMSound> bgmDictionary;
    private Dictionary<SFXName, SFXSound> sfxDictionary;
    private Dictionary<SFXName, AudioSource> loopingSFXSource;
    
    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // 사운드 초기화
        CreateBGMSources();
        CreateVFXSources();

        InitializeDictionaries();
    }
    
    private void CreateBGMSources()
    {
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }
    }
    
    private void CreateVFXSources()
    {
        int playCount = 5;
        sfxSources = new AudioSource[playCount];
        availableSFXSources = new Queue<AudioSource>();
        
        for (int i = 0; i < playCount; i++)
        {
            sfxSources[i] = gameObject.AddComponent<AudioSource>();
            sfxSources[i].loop = false;
            sfxSources[i].playOnAwake = false;
            availableSFXSources.Enqueue(sfxSources[i]);
        }
    }
    
    private void InitializeDictionaries()
    {
        bgmDictionary = new Dictionary<BGMName, BGMSound>();
        sfxDictionary = new Dictionary<SFXName, SFXSound>();
        loopingSFXSource = new Dictionary<SFXName, AudioSource>();
        
        // BGM 딕셔너리 생성
        foreach (BGMSound s in bgmSounds)
        {
            if (!bgmDictionary.ContainsKey(s.name))
            {
                bgmDictionary[s.name] = s;
            }
        }
        
        // VFX 딕셔너리 생성
        foreach (SFXSound s in sfxSounds)
        {
            if (!sfxDictionary.ContainsKey(s.name))
            {
                sfxDictionary[s.name] = s;
                if (s.loop)
                {
                    loopingSFXSource[s.name] = new AudioSource();
                }
            }
        }
    }
    
    /// <summary>
    /// BGM 실행 함수
    /// </summary>
    /// <param name="name"></param>
    public void PlayBGM(BGMName name)
    {
        if (bgmDictionary.TryGetValue(name, out BGMSound sound))
        {
            bgmSource.clip = sound.clip;
            bgmSource.loop = true;
            bgmSource.volume = sound.volume;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("BGM not found: " + name);
        }
    }
    
    public void StopBGM()
    {
        bgmSource.Stop();
    }
    
    /// <summary>
    /// SFX 실행 함수
    /// </summary>
    /// <param name="name"></param>
    public void PlaySFX(SFXName name)
    {
        if (sfxDictionary.TryGetValue(name, out SFXSound sound))
        {
            AudioSource source = GetAvailableSFXSource();
            if (source != null)
            {
                source.clip = sound.clip;
                source.loop = sound.loop;
                source.volume = sound.volume;
                source.Play();

                if (sound.loop)
                {
                    if (loopingSFXSource.ContainsKey(name))
                    {
                        loopingSFXSource[name] = source;
                    }
                }
                else
                {
                    StartCoroutine(ReturnSourceWhenFinished(source, sound.clip.length / source.pitch));
                }
            }
        }
        else
        {
            Debug.LogWarning("VFX Sound not found: " + name);
        }
    }
    
    public void StopSFX(SFXName name)
    {
        if (loopingSFXSource.ContainsKey(name) && loopingSFXSource[name] != null)
        {
            AudioSource source = loopingSFXSource[name];
            source.Stop();
            availableSFXSources.Enqueue(source);
            loopingSFXSource[name] = null;
        }
        else
        {
            Debug.LogWarning("Cannot stop non-looping VFX or VFX not found: " + name);
        }
    }

    public bool IsPlayingSFX(SFXName name)
    {
        if (loopingSFXSource.ContainsKey(name))
        {
            if (loopingSFXSource[name] == null) return false;
            return loopingSFXSource[name].isPlaying;
        }

        return false;
    }
    
    private AudioSource GetAvailableSFXSource()
    {
        if (availableSFXSources.Count > 0)
        {
            return availableSFXSources.Dequeue();
        }
        
        // 사용 가능한 소스가 없으면 재생이 끝난 소스를 찾아서 반환
        foreach (AudioSource source in sfxSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        
        // 모든 소스가 사용 중이면 첫 번째 소스를 강제로 사용
        return sfxSources[0];
    }
    
    private IEnumerator ReturnSourceWhenFinished(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!source.isPlaying && !availableSFXSources.Contains(source))
        {
            availableSFXSources.Enqueue(source);
        }
    }
}
