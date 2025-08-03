using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class AbilityTask
{
    // GA Activate() 함수에서 실행할 수 있게
    // execute(), canceled()
    private GameObject _actor;
    private readonly AbilitySequenceSO _sequenceSO;
    private Camera _camera;
    private Animator _animator;
    private Sequence _mainSequence;
    
    private readonly int _endSkillID =  Animator.StringToHash("EndSkill");

    /// <summary>
    /// 기본 생성자
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="sequenceSO"></param>
    public AbilityTask(GameObject actor, AbilitySequenceSO sequenceSO)
    {
        _actor = actor;
        _sequenceSO = sequenceSO;
        _animator = _actor.GetComponent<Animator>();
        
        Initialize();
    }
    
    /// <summary>
    /// Camera 움직임이 필요한 경우
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="camera"></param>
    /// <param name="sequenceSO"></param>
    public AbilityTask(GameObject actor, Camera camera, AbilitySequenceSO sequenceSO)
    {
        _actor = actor;
        _camera = camera;
        _sequenceSO = sequenceSO;
        _animator = _actor.GetComponent<Animator>();
        
        Initialize();
    }

    private void Initialize()
    {
        _mainSequence = DOTween.Sequence();
        
        if (_sequenceSO != null)
        {
            // Animation
            for (int i = 0; i < _sequenceSO.animations.Count; i++)
            {
                AbilitySequenceSO.AnimationSq data =  _sequenceSO.animations[i];
                _mainSequence.InsertCallback(data.delay, () => PlayAnimation(data));
            }
            
            // Sound
            for (int i = 0; i < _sequenceSO.sounds.Count; i++)
            {
                AbilitySequenceSO.SoundSq data = _sequenceSO.sounds[i];
                _mainSequence.InsertCallback(data.delay, () => PlaySound(data));
            }
            
            // Effect
            for (int i = 0; i < _sequenceSO.effects.Count; i++)
            {
                AbilitySequenceSO.EffectSq data = _sequenceSO.effects[i];
                _mainSequence.InsertCallback(data.delay, () => PlayEffect(data));
            }
            
            // Slow
            for (int i = 0; i < _sequenceSO.slows.Count; i++)
            {
                _mainSequence.Join(PlaySlow(_sequenceSO.slows[i]));
                /*AbilitySequenceSO.SlowSq data =  _sequenceSO.slows[i];
                _mainSequence.InsertCallback(data.delay, () => PlaySlow(data));*/
            }
            
            // Camera Task
            for (int i = 0; i < _sequenceSO.cameras.Count; i++)
            {
                AbilitySequenceSO.CameraSq data = _sequenceSO.cameras[i];
                _mainSequence.InsertCallback(data.delay, () => PlayCameraTask(data));
            }
        }

        _mainSequence.SetAutoKill(false);
    }
    
    public void Execute()
    {
        _mainSequence.Play();
    }

    public void Canceled()
    {
        _mainSequence.Pause();
        _mainSequence.Rewind();
        
        _animator.SetTrigger(_endSkillID);
    }

    private void PlayAnimation(AbilitySequenceSO.AnimationSq data)
    {
        _animator.Play(data.source.name, 0, 0);
    }

    private void PlaySound(AbilitySequenceSO.SoundSq data)
    {
        
    }

    private void PlayEffect(AbilitySequenceSO.EffectSq data)
    {
        
    }
    
    private Sequence PlaySlow(AbilitySequenceSO.SlowSq data)
    {
        float originalTimeScale = Time.timeScale;
        
        var sequence = DOTween.Sequence();
        sequence.SetUpdate(true); // UnscaledTime 사용
        
        if (data.delay > 0)
            sequence.AppendInterval(data.delay);
        
        // Slow In
        //sequence.AppendCallback(()=> Time.timeScale = data.targetTimeScale);
        sequence.Append(DOTween.To(() => Time.timeScale, x =>
            {
                Time.timeScale = x;
                //Debug.Log(Time.timeScale);
            }
                , data.targetTimeScale, 0.1f)
            .SetEase(Ease.InOutQuad).SetUpdate(true));
        
        // Slow Duration 유지
        sequence.AppendInterval(data.duration);
        
        /*// Slow Out
        sequence.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, originalTimeScale, 0.1f)
            .SetEase(Ease.InOutQuad).SetUpdate(true));*/

        sequence.OnComplete(() => { Time.timeScale = originalTimeScale; });
        
        return sequence;
    }

    private void PlayCameraTask(AbilitySequenceSO.CameraSq data)
    {
        data.cameraTask.Initialize(_camera);
        data.cameraTask.Execute();
    }
}
