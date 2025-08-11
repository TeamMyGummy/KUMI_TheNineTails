    using UnityEngine;

    //todo: (강승연) 상황 봐서... 동작방식 나눌 거면 나누겠음
    public class MoveHandler : ActionHandler
    {
        [SerializeField] private float speed = 2f;
        private Transform _targetTransform;
        private Vector3 _vector;
        private MoveType _xtype;
        private MoveType _ytype;
        private Vector3 _currentDest;
        
        private const float StopThreshold = 0.1f;

        void Awake()
        {
            _targetTransform = GameObject.FindWithTag("Player").transform;
            Debug.Log(_targetTransform.gameObject.name);
        }

        public void SetMovementPoint(Vector3 vector, MoveType xtype, MoveType ytype, float speed)
        {
            _vector = vector;
            _xtype = xtype;
            _ytype = ytype;
            if (speed != 0f) this.speed = speed;
        }

        public override void OnEnterAction()
        {
            _currentDest = 
                new Vector3(GetTargetDestination(_xtype).x, GetTargetDestination(_ytype).y);
        }        
        
        public override bool OnExecuteAction()
        {
            if (Vector3.Distance(transform.position, _currentDest) <= StopThreshold)
            {
                return true; // 목적지 도달 → 행동 완료
            }

            // 방향 계산
            Vector3 direction = (_currentDest - transform.position).normalized;

            // 프레임 이동
            transform.position += direction * (speed * Time.deltaTime);

            return false; // 아직 도달 안함 → 계속 실행 중
        }

        private Vector3 GetTargetDestination(MoveType type)
        {
            switch (type)
            {
                case MoveType.Offset:
                    return transform.position + _vector;
                case MoveType.TargetOffset:
                    return _targetTransform.position + _vector;
                case MoveType.WorldLocation:
                    return _vector;
                case MoveType.CameraOffset:
                    return Camera.main.transform.position + _vector;
            }
            return Vector3.zero;
        }
    }
