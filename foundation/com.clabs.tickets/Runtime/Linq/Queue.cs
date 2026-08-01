using System;
using System.Threading;

namespace CLabs.Tickets.Linq
{
    public static partial class TicketAsyncEnumerable
    {
        public static ITicketAsyncEnumerable<TSource> Queue<TSource>(this ITicketAsyncEnumerable<TSource> source)
        {
            return new QueueOperator<TSource>(source);
        }
    }

    internal sealed class QueueOperator<TSource> : ITicketAsyncEnumerable<TSource>
    {
        readonly ITicketAsyncEnumerable<TSource> source;

        public QueueOperator(ITicketAsyncEnumerable<TSource> source)
        {
            this.source = source;
        }

        public ITicketAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Queue(source, cancellationToken);
        }

        sealed class _Queue : ITicketAsyncEnumerator<TSource>
        {
            readonly ITicketAsyncEnumerable<TSource> source;
            CancellationToken cancellationToken;

            Channel<TSource> channel;
            ITicketAsyncEnumerator<TSource> channelEnumerator;
            ITicketAsyncEnumerator<TSource> sourceEnumerator;
            bool channelClosed;

            public _Queue(ITicketAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
            {
                this.source = source;
                this.cancellationToken = cancellationToken;
            }

            public TSource Current => channelEnumerator.Current;

            public Ticket<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (sourceEnumerator == null)
                {
                    sourceEnumerator = source.GetAsyncEnumerator(cancellationToken);
                    channel = Channel.CreateSingleConsumerUnbounded<TSource>();

                    channelEnumerator = channel.Reader.ReadAllAsync().GetAsyncEnumerator(cancellationToken);

                    ConsumeAll(this, sourceEnumerator, channel).Forget();
                }

                return channelEnumerator.MoveNextAsync();
            }

            static async TicketVoid ConsumeAll(_Queue self, ITicketAsyncEnumerator<TSource> enumerator, ChannelWriter<TSource> writer)
            {
                try
                {
                    while (await enumerator.MoveNextAsync())
                    {
                        writer.TryWrite(enumerator.Current);
                    }
                    writer.TryComplete();
                }
                catch (Exception ex)
                {
                    writer.TryComplete(ex);
                }
                finally
                {
                    self.channelClosed = true;
                    await enumerator.DisposeAsync();
                }
            }

            public async Ticket DisposeAsync()
            {
                if (sourceEnumerator != null)
                {
                    await sourceEnumerator.DisposeAsync();
                }
                if (channelEnumerator != null)
                {
                    await channelEnumerator.DisposeAsync();
                }

                if (!channelClosed)
                {
                    channelClosed = true;
                    channel.Writer.TryComplete(new OperationCanceledException());
                }
            }
        }
    }
}