using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pipeline
{
    public partial class Pipeline<TIn, TOut, TStatus>
	{
        private IList<IStage> _stages = new List<IStage>();

        private Pipeline()
		{
		}

        public IEnumerable<TOut> Run(CancellationToken token)
        {
        }

        public static PipelineBuilder<TIn> New()
        {
            return new PipelineBuilder<TIn>();
        }
	}
}