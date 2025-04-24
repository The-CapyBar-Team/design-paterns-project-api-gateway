using System.Collections.Concurrent;

namespace APIGateway.Utils
{
    public class ResponseTracker
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<byte[]>> _pendingRequests;
        private readonly TimeSpan _defaultTimeout;

        public ResponseTracker(TimeSpan defaultTimeout)
        {
            _pendingRequests = new ConcurrentDictionary<string, TaskCompletionSource<byte[]>>();
            _defaultTimeout = defaultTimeout;
        }

        public void RegisterRequest(string correlationId)
        {
            var tcs = new TaskCompletionSource<byte[]>();
            _pendingRequests.TryAdd(correlationId, tcs);
        }

        public async Task<byte[]> AwaitResponseAsync(string correlationId, TimeSpan? timeout = null)
        {
            if (!_pendingRequests.TryGetValue(correlationId, out var tcs))
            {
                throw new ArgumentException("Unknown correlation ID");
            }

            var timeoutTask = Task.Delay(timeout ?? _defaultTimeout);
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
                _pendingRequests.TryRemove(correlationId, out _);
                return null; // or throw TimeoutException
            }

            _pendingRequests.TryRemove(correlationId, out _);
            return await tcs.Task;
        }

        public void CompleteRequest(string correlationId, byte[] response)
        {
            if (_pendingRequests.TryGetValue(correlationId, out var tcs))
            {
                tcs.SetResult(response);
            }
        }
    }
}
