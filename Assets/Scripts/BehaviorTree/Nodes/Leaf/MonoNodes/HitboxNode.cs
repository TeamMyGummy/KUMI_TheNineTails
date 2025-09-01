using UnityEngine;
using BehaviorTree.Leaf;

namespace BehaviorTree.Leaf
{
    public class HitboxNode : MonoNode
    {
        [SerializeField] public Vector2 size;
        [SerializeField] public Vector2 offset;
        [SerializeField] public float timer;

        protected override void OnEnter()
        {
            if (runtimeHandler is HitboxHandler handler) 
                handler.SetHitbox(size, offset, timer);
        }
    }
}