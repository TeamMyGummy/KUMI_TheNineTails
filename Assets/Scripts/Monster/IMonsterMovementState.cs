
    public interface IMonsterMovementState 
    { 
        void Enter(MonsterMovement monsterMovement); //State 진입 시 시작
        void UpdateState(); //State 중 매 프레임 호출
        void Exit(); //State 나갈 시 시작
    }

   