using System;
using System.Threading;
using System.Threading.Tasks;

namespace TalkiPlay.Functional.Extensions
{
    public static class SemaphoreSlimExtensions
    {
        public static async Task<IDisposable> UseWaitAsync(
            this SemaphoreSlim semaphore, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            await semaphore.WaitAsync(cancelToken)
                .ConfigureAwait(false);
            return new ReleaseWrapper(semaphore);
        }
 
        public static async Task<IDisposable> UseWaitAsync(
            this SemaphoreSlim semaphore, 
            int millisecondsTimeout)
        {
            await semaphore.WaitAsync(millisecondsTimeout)
                .ConfigureAwait(false);
            return new ReleaseWrapper(semaphore);
        }
        
        private class ReleaseWrapper : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;
 
            private bool _isDisposed;
 
            public ReleaseWrapper(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }
 
            public void Dispose()
            {
                if (_isDisposed)
                    return;
 
                _semaphore.Release();
                _isDisposed = true;
            }
        }
    }
}