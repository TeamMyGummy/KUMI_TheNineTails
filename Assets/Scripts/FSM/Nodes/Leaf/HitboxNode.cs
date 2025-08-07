using UnityEngine;

[CreateNodeMenu("FSM/Leaf/Hitbox Node")]
public class HitboxNode : MonoNode
{ 
    [SerializeField] private Vector2 size;
        [SerializeField] private Vector2 offset;
        [SerializeField] private float timer;
        protected override void OnEnterAction()
        {
            if (runtimeHandler is HitboxHandler handler)
            {
                handler.SetHitbox(size, offset, timer);
            }

            base.OnEnterAction();
        }
}
