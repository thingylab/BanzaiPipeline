using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Pipeline
{
    public partial class Pipeline<TIn, TOut, TStatus>
    {
        public sealed class OriginalPipelineBuilder
        {
            private readonly Pipeline<TIn, TOut, TStatus> _pipeline;

            internal OriginalPipelineBuilder()
            {
                _pipeline = new Pipeline<TIn, TOut, TStatus>();
            }

            public PipelineBuilder<TStageOut> WithStage<TStageOut>(Func<IEnumerable<TStageOut>> stageFunc)
            {
                _pipeline._stages.Add(new ProducerStage<TStageOut>(stageFunc));

                var nextBuilder = new PipelineBuilder<TStageOut>(_pipeline);

                return nextBuilder;
            }
        }
        
        public sealed class PipelineBuilder<TStageIn>
        {
            private readonly Pipeline<TIn, TOut, TStatus> _pipeline;

            internal PipelineBuilder(Pipeline<TIn, TOut, TStatus> pipeline)
            {
                _pipeline = pipeline;
            }

            public PipelineBuilder<TStageOut> WithStage<TStageOut>(Func<TStageIn, TStageOut> stageFunc)
            {
                var previousStage = _pipeline._stages.Last() as IProducer<TStageIn>;
                var stage = new ProcessorStage<TStageIn, TStageOut>(stageFunc)
                {
                    Previous = previousStage
                };
                previousStage.Next = stage;

                _pipeline._stages.Add(stage);

                var nextBuilder = new PipelineBuilder<TStageOut>(_pipeline);

                return nextBuilder;
            }

            public Pipeline<TIn, TOut, TStatus> Finally(Func<TStageIn, TOut> finalStage)
            {
                var previousStage = _pipeline._stages.Last() as IProducer<TStageIn>;
                var stage = new ProcessorStage<TStageIn, TOut>(finalStage)
                {
                    Previous = previousStage
                };
                previousStage.Next = stage;

                _pipeline._stages.Add(stage);

                return _pipeline;
            }
        }
    }
}