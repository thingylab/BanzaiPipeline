using System;

namespace Pipeline
{
    public partial class Pipeline<TIn, TOut, TStatus>
    {
        internal interface IStage
        {
        }

        internal interface IConsumer<T>
        {
        }

        internal interface IProducer<T>
        {
        }

        internal sealed class Stage<TStageIn, TStageOut> : IStage, IConsumer<TStageIn>, IProducer<TStageOut>
        {
            private Func<TStageIn, TStageOut> _func;

            private IProducer<TStageIn> Previous { get; set; }

            private IConsumer<TStageOut> Next { get; set; }

            public Stage(Func<TStageIn, TStageOut> func)
            {
                _func = func;
            }
        }
    }
}