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
    private readonly AbilitySequenceSO _sequence;
    private Camera _camera;
    private Animator _animator;
    private Sequence _dTsequence;
    
    private readonly int _endSkillID =  Animator.StringToHash("EndSkill");

    /// <summary>
    /// 기본 생성자
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="sequence"></param>
    public AbilityTask(GameObject actor, AbilitySequenceSO sequence)
    {
        _actor = actor;
        _sequence = sequence;
        _animator = _actor.GetComponent<Animator>();
        
        Initialize();
    }
    
    /// <summary>
    /// Camera 움직임이 필요한 경우
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="camera"></param>
    /// <param name="sequence"></param>
    public AbilityTask(GameObject actor, Camera camera, AbilitySequenceSO sequence)
    {
        _actor = actor;
        _camera = camera;
        _sequence = sequence;
        _animator = _actor.GetComponent<Animator>();
        
        Initialize();
    }

    private void Initialize()
    {
        _dTsequence = DOTween.Sequence();
        
        if (_sequence != null)
        {
            // Animation
            for (int i = 0; i < _sequence.animations.Count; i++)
            {
                var animation = _sequence.animations[i].source;
                var delay = _sequence.animations[i].delay;
                _dTsequence.InsertCallback(delay, () => PlayAnimation(animation));
            }
            
            // Sound
            for (int i = 0; i < _sequence.sounds.Count; i++)
            {
                var sound = _sequence.sounds[i].source;
                var delay = _sequence.sounds[i].delay;
                _dTsequence.InsertCallback(delay, () => PlaySound(sound));
            }
            
            // Slow
            for (int i = 0; i < _sequence.slows.Count; i++)
            {
                var duration = _sequence.slows[i].duration;
                var delay = _sequence.slows[i].delay;
                _dTsequence.InsertCallback(delay, () => PlaySlow(duration).Forget());
            }
            
            // Camera Task
            for (int i = 0; i < _sequence.cameras.Count; i++)
            {
                var camera = _sequence.cameras[i].cameraTask;
                var delay = _sequence.cameras[i].delay;
                _dTsequence.InsertCallback(delay, () => PlayCameraTask(camera));
            }
        }

        _dTsequence.SetAutoKill(false);
    }
    
    public void Execute()
    {
        _dTsequence.Play();
    }

    public void Canceled()
    {
        _dTsequence.Pause();
        _dTsequence.Rewind();
        
        _animator.SetTrigger(_endSkillID);
    }

    private void PlayAnimation(AnimationClip source)
    {
        _animator.Play(source.name, 0, 0);
    }

    private void PlaySound(AudioClip source)
    {
        
    }
    
    private async UniTask PlaySlow(float duration)
    {
        await UniTask.Delay((int)(duration * 1000));
    }

    private void PlayCameraTask(CameraTask cameraTask)
    {
        cameraTask.Initialize(_camera);
        cameraTask.Execute();
    }
}
