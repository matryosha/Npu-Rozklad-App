using System;
using System.Threading.Tasks;

namespace NpuRozklad.Core.Infrastructure
{
    public abstract class PeriodicOperationExecutor
    {
        protected int PeriodicCallIntervalInSeconds { get; set; } = 1;

        private Func<Task> _asyncDelegate;
        protected Func<Task> PeriodicAction
        {
            get => _asyncDelegate;
            set
            {
                _asyncDelegate = value;
                // cancellation token?
                UpdateFunction();
            }
        }
        
        private async Task UpdateFunction()
        {
            // exception kills calling
            while (true)
            {
                var delayTime = TimeSpan.FromSeconds(PeriodicCallIntervalInSeconds);
                if (delayTime.TotalMilliseconds <= 0) return;
                
                await Task.Delay(delayTime);
                await _asyncDelegate();
            }
        }
    }
}