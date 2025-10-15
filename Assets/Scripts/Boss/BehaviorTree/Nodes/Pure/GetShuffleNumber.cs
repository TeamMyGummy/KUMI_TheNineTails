using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace BehaviorTree.Pure
{
    public class GetShuffleNumber : Node
    {
        [SerializeField][Input] public int endNumber = 5; // 1부터 endNumber까지 셔플해서 반환
        
        [Output(connectionType = ConnectionType.Multiple)] 
        public int value;

        // 런타임 상태를 저장할 필드들
        // NonSerialized 어트리뷰트로 애셋에 저장되지 않도록 함
        [System.NonSerialized] private List<int> _shuffledList;
        [System.NonSerialized] private int _currentIndex;

        // XNode 그래프가 초기화될 때 호출될 수 있도록 추가 (안전장치)
        protected override void Init()
        {
            base.Init();
            _shuffledList = null; // 런타임 시작 시 상태 초기화
            _currentIndex = 0;
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName != "value") 
                return null;

            // 1. 수열이 없거나, 2. 수열의 끝까지 다 읽었는지 확인
            if (_shuffledList == null || _currentIndex >= _shuffledList.Count)
            {
                // 수열을 새로 생성하고 섞은 뒤, 인덱스를 0으로 리셋
                Reshuffle();
            }
            
            // 현재 인덱스에 해당하는 숫자를 반환하고, 인덱스를 1 증가시킴
            int result = _shuffledList[_currentIndex];
            _currentIndex++;
            
            return result;
        }

        /// <summary>
        /// 1부터 endNumber까지의 수열을 새로 만들어 랜덤하게 섞습니다.
        /// </summary>
        private void Reshuffle()
        {
            int endRange = GetInputValue<int>("endNumber", endNumber);
            Debug.Log(endRange);
            System.Random random = new System.Random();
            _shuffledList = Enumerable.Range(1, endRange)
                .OrderBy(x => random.Next())
                .ToList();
            _currentIndex = 0; // 인덱스를 맨 앞으로 초기화
        }
    }
}