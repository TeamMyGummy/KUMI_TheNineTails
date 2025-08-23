    using DG.Tweening;
    using UnityEngine;

    public class Moveable : MonoBehaviour
    {
        public float timer;
        public void MoveX(float x)
        {
            transform.DOMoveX(x, timer);
        }
        
        public void MoveY(float y)
        {
            transform.DOMoveY(y, timer);
        }
    }
