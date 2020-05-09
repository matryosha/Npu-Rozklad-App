using System;
using System.Threading;
using System.Threading.Tasks;

namespace NpuRozklad.Core.Infrastructure
{
    public abstract class PeriodicOperationExecutor
    {
        protected int PeriodicCallIntervalInSeconds { get; set; } = 1;
        private Task _updateTask;

        private Func<Task> _asyncDelegate;
        private CancellationTokenSource _tokenSource;
        protected Func<Task> PeriodicAction
        {
            get => _asyncDelegate;
            set
            {
                _asyncDelegate = value;
                if (_updateTask != null)
                    throw new NotSupportedException("Periodic operation was already set");
                _tokenSource = new CancellationTokenSource();
                _updateTask = UpdateFunction(_tokenSource.Token);
            }
        }

        protected void StopPeriodicOperation()
        {
            _tokenSource.Cancel();
        }
        
        private async Task UpdateFunction(CancellationToken cancellationToken)
        {
            // exception kills calling
            while (true)
            {
                var delayTime = TimeSpan.FromSeconds(PeriodicCallIntervalInSeconds);
                if (delayTime.TotalMilliseconds <= 0) return;
                
                await Task.Delay(delayTime, cancellationToken);
                if(cancellationToken.IsCancellationRequested) return;
                await _asyncDelegate();
            }
        }
    }
}