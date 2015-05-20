using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pipeline
{
    public partial class Pipeline<TIn, TOut, TStatus>
    {
        internal interface IStage
        {
            Task Start(TaskFactory taskFactory);
        }

        internal interface IConsumer<T>
        {
        }

        internal interface IProducer<T>
        {
            IConsumer<T> Next { get; set; }

            BlockingCollection<T> OutputCollection { get; }
        }

        internal sealed class ProducerStage<TStageOut> : IStage, IProducer<TStageOut>
        {
            private readonly Func<IEnumerable<TStageOut>> _func;

            public IConsumer<TStageOut> Next { get; set; }

            public BlockingCollection<TStageOut> OutputCollection { get; private set; }

            internal ProducerStage(Func<IEnumerable<TStageOut>> func)
            {
                _func = func;
                OutputCollection = new BlockingCollection<TStageOut>();
            }

            public Task Start(TaskFactory taskFactory)
            {
                return taskFactory.StartNew(StageWork);
            }

            private void StageWork()
            {
                try
                {
                    foreach (var item in _func())
                    {
                        OutputCollection.Add(item);
                    }
                }
                finally
                {
                    OutputCollection.CompleteAdding();
                }
            }
        }

        internal sealed class ProcessorStage<TStageIn, TStageOut> : IStage, IConsumer<TStageIn>, IProducer<TStageOut>
        {
            private readonly Func<TStageIn, TStageOut> _func;

            public IProducer<TStageIn> Previous { get; set; }

            public IConsumer<TStageOut> Next { get; set; }

            public BlockingCollection<TStageOut> OutputCollection { get; private set; }

            internal ProcessorStage(Func<TStageIn, TStageOut> func)
            {
                _func = func;
                OutputCollection = new BlockingCollection<TStageOut>();
            }

            public Task Start(TaskFactory taskFactory)
            {
                return taskFactory.StartNew(StageWork);
            }

            private void StageWork()
            {
                try
                {
                    foreach (var item in Previous.OutputCollection.GetConsumingEnumerable())
                    {
                        OutputCollection.Add(_func(item));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    OutputCollection.CompleteAdding();
                }
            }
        }
    }
}