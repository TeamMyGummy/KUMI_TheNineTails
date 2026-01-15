using UnityEngine;
using BehaviorTree.Leaf;

namespace BehaviorTree.Leaf
{
    public class PlaySFXNode : LeafNode
    { 
        [Header("Sound Settings")]
        [SerializeField] private SFXName sfxname;
        [SerializeField] private float volume = 1.0f;

        protected override NodeState Start()
        {
            // 사운드 재생
            SoundManager.Instance.PlaySFX(sfxname);
            return NodeState.Success;
        }

        protected override NodeState Update()
        {
            return NodeState.Success;
        }

        protected override void OnAbort()
        {
        }
    }
}