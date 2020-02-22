using System;

namespace MyThreadPool
{
    /// <summary>
    /// An interface describing an asynchronous operation.
    /// </summary>
    /// <typeparam name="TResult">Type of the task result.</typeparam>
    public interface IMyTask<TResult>
    {
        /// <summary>
        /// Reports whether the task is completed or not.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Returns the result of the task.
        /// </summary>
        TResult Result { get; }

        /// <summary>
        /// Creates a continuation that executes when the target task completes.
        /// </summary>
        /// <typeparam name="TNewResult">Type of the continuation result.</typeparam>
        /// <param name="supplier">A function describing continuation.</param>
        /// <returns>The created continuation.</returns>
        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier);
    }
}
