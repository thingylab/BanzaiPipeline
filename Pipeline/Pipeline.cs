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

        public IEnumerable<TOut> Run(out TStatus status, CancellationToken ct)
        {
            var taskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
            
            var tasks = _stages.Select(stage => stage.Start(taskFactory)).ToArray();

            status = new TStatus();

            return (_stages.Last() as IProducer<TOut>).OutputCollection.GetConsumingEnumerable();
        }
	}
}