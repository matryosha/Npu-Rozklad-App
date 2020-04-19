using System;
using System.Threading.Tasks;

namespace NpuRozklad.LessonsProvider
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
                await Task.Delay(TimeSpan.FromSeconds(PeriodicCallIntervalInSeconds));
                await _asyncDelegate();
            }
        }
    }
}