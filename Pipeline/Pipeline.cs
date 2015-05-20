using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pipeline
{
    public partial class Pipeline<TIn, TOut, TStatus>
        where TStatus : new()
	{
        private readonly IList<IStage> _stages = new List<IStage>();

        private Pipeline()
		{
		}

        public static OriginalPipelineBuilder New()
        {
            return new OriginalPipelineBuilder();
        }

        public Task<TStatus> Run(out IEnumerable<TOut> result)
        {
            var cts = new CancellationTokenSource();

            return Run(result, cts.Token);
        }

        public Task<TStatus> Run(out IEnumerable<TOut> result, CancellationToken token)
        {
            var taskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
            
            var tasks = _stages.Select(stage => stage.Start(taskFactory)).ToArray();

            Task.WaitAll(tasks);

            return Task.FromResult(new TStatus());
        }
	}
}