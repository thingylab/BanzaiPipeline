using System;
using System.Collections.Generic;

namespace Pipeline
{
    public partial class Pipeline<TIn, TOut, TStatus>
    {
        public sealed class OriginalPipelineBuilder
        {
            private Pipeline<TIn, TOut, TStatus> _pipeline;

            internal OriginalPipelineBuilder()
            {
                _pipeline = new Pipeline<TIn, TOut, TStatus>();
            }

            public PipelineBuilder<TStageOut> WithStage<TStageOut>(Func<IEnumerable<TStageOut>> stageFunc)
            {
                var nextBuilder = new PipelineBuilder<TStageOut>(_pipeline);

                return nextBuilder;
            }
        }
        
        public sealed class PipelineBuilder<TStageIn>
        {
            private Pipeline<TIn, TOut, TStatus> _pipeline;

            internal PipelineBuilder(Pipeline<TIn, TOut, TStatus> pipeline)
            {
                _pipeline = pipeline;
            }

            public PipelineBuilder<TStageOut> WithStage<TStageOut>(Func<TStageIn, TStageOut> stageFunc)
            {
                var stage = new Stage<TStageIn, TStageOut>(stageFunc);

                _pipeline._stages.Add(stage);

                var nextBuilder = new PipelineBuilder<TStageOut>
                {
                    _pipeline = _pipeline
                };

                return nextBuilder;
            }

            public Pipeline<TIn, TOut, TStatus> Finally(Func<TStageIn, TOut> finalStage)
            {
                if (_pipeline == null)
                {
                    _pipeline = new Pipeline<TIn, TOut, TStatus>();
                }

                var stage = new Stage<TStageIn, TOut>(finalStage);

                _pipeline._stages.Add(stage);

                return _pipeline;
            }
        }
    }
}