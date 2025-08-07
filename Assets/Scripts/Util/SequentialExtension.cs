using System;
using Cysharp.Threading.Tasks;

namespace Util
{
    public static class SequentialExtensions
    {
        /// <summary>
        /// 현재 태스크 완료 후 다음 태스크를 실행합니다
        /// </summary>
        public static async UniTask Then(this UniTask task, Func<UniTask> next)
        {
            await task;
            await next();
        }

        /// <summary>
        /// 현재 태스크 완료 후 다음 태스크를 실행하고 결과를 반환합니다
        /// </summary>
        public static async UniTask<TResult> Then<TResult>(this UniTask task, Func<UniTask<TResult>> next)
        {
            await task;
            return await next();
        }

        /// <summary>
        /// 현재 태스크의 결과를 받아 다음 태스크를 실행합니다
        /// </summary>
        public static async UniTask Then<T>(this UniTask<T> task, Func<T, UniTask> next)
        {
            var result = await task;
            await next(result);
        }

        /// <summary>
        /// 현재 태스크의 결과를 받아 다음 태스크를 실행하고 결과를 반환합니다
        /// </summary>
        public static async UniTask<TResult> Then<T, TResult>(this UniTask<T> task, Func<T, UniTask<TResult>> next)
        {
            var result = await task;
            return await next(result);
        }

        /// <summary>
        /// 지정된 시간만큼 지연 후 다음 태스크를 실행합니다
        /// </summary>
        public static async UniTask Delay(this UniTask task, float seconds)
        {
            await task;
            await UniTask.Delay(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// 지정된 시간만큼 지연 후 다음 태스크를 실행하고 결과를 유지합니다
        /// </summary>
        public static async UniTask<T> Delay<T>(this UniTask<T> task, float seconds)
        {
            var result = await task;
            await UniTask.Delay(TimeSpan.FromSeconds(seconds));
            return result;
        }
    }

    /// <summary>
    /// 시퀀셜 체인을 지연하는 헬퍼 클래스
    /// </summary>
    public static class Sequential
    {
        /// <summary>
        /// 시퀀셜 체인을 생성합니다
        /// </summary>
        public static UniTask Lazy(Func<UniTask> firstTask)
        {
            return firstTask();
        }

        /// <summary>
        /// 결과를 반환하는 시퀀셜 체인을 생성합니다
        /// </summary>
        public static UniTask<T> Lazy<T>(Func<UniTask<T>> firstTask)
        {
            return firstTask();
        }
    }
}