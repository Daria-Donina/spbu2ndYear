using System;

namespace MyThreadPool
{
    public interface IMyTask<TResult>
    {
        bool IsCompleted { get; }
        bool Result { get; }
        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier);
    }
}
