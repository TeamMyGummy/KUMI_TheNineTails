using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace AbilitySystem.Base
{
    /// <summary>
    /// Enum(GameplayTags)으로 Tag 관리 중 <br/>
    /// 만약 계층화가 필요하다면 string으로 리팩토링 하겠음
    /// </summary>
    public class TagContainer
    {
        private readonly HashSet<GameplayTags> _tags = new();

        /// <summary>
        /// 태그가 존재여부 확인
        /// </summary>
        /// <param name="tag">확인할 태그</param>
        /// <returns>태그 존재 유무</returns>
        public bool Has(GameplayTags tag) => _tags.Contains(tag);
        /// <summary>
        /// 태그 추가
        /// </summary>
        /// <param name="tag">추가할 태그</param>
        /// <returns>태그 추가 여부(이미 있을 시 추가 X false 반환)</returns>
        public bool Add(GameplayTags tag) => _tags.Add(tag);
        /// <summary>
        /// 태그 제거
        /// </summary>
        /// <param name="tag">제거할 태그</param>
        /// <returns>태그 제거 여부(이미 없을 시 false)</returns>
        public bool Remove(GameplayTags tag) => _tags.Remove(tag);
        
        /// <summary>
        /// 태그 부여 후 일정 시간 후 태그 삭제
        /// </summary>
        /// <param name="tag">부여할 태그</param>
        /// <param name="duration">삭제되기 전 유예시간(단위 : sec)</param>
        /// <returns></returns>
        public async UniTask<bool> AddWithDuration(GameplayTags tag, float duration)
        {
            if (!Add(tag)) return false;
            await UniTask.Delay(TimeSpan.FromSeconds(duration), DelayType.DeltaTime);
            return Remove(tag);
        }
    }
}