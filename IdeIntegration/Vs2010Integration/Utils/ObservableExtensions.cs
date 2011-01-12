using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Disposables;
using System.Linq;

namespace TechTalk.SpecFlow.Vs2010Integration.Utils
{
    public static class ObservableExtensions
    {
        public static IObservable<TSource[]> BufferWithTimeout<TSource>(this IObservable<TSource> source, TimeSpan timeout)
        {
            return source.BufferWithTimeout(timeout, Scheduler.TaskPool);
        }

        public static IObservable<TSource[]> BufferWithTimeout<TSource>(
            this IObservable<TSource> source, TimeSpan timeout, IScheduler scheduler, bool flushFirst = false)
        {
            return Observable.CreateWithDisposable<TSource[]>(
                observer =>
                    {
                        object lockObject = new object();
                        List<TSource> buffer = new List<TSource>();
                        bool fistToBuffer = true;

                        MutableDisposable timeoutDisposable = new MutableDisposable();

                        Action flushBuffer = () =>
                                                 {
                                                     TSource[] values;

                                                     lock (lockObject)
                                                     {
                                                         values = buffer.ToArray();
                                                         buffer.Clear();
                                                         fistToBuffer = true;
                                                     }

                                                     observer.OnNext(values);
                                                 };

                        var sourceSubscription = source.Subscribe(
                            value =>
                                {
                                    if (fistToBuffer && flushFirst)
                                    {
                                        bool isFirst = false;
                                        lock (lockObject)
                                        {
                                            if (fistToBuffer)
                                            {
                                                fistToBuffer = false;
                                                isFirst = true;
                                            }
                                        }
                                        if (isFirst)
                                        {
                                            observer.OnNext(new[] {value});
                                            return;
                                        }
                                    }

                                    lock (lockObject)
                                    {
                                        buffer.Add(value);
                                    }

                                    timeoutDisposable.Disposable = scheduler.Schedule(flushBuffer, timeout);
                                },
                            observer.OnError,
                            () =>
                                {
                                    flushBuffer();
                                    observer.OnCompleted();
                                });

                        return new CompositeDisposable(sourceSubscription, timeoutDisposable);
                    });
        }

        public static IObservable<Tuple<TSource, TSource>> WithPrevious<TSource>(this IObservable<TSource> source) 
            where TSource : class
        {
            return source.Scan((Tuple<TSource, TSource>) null,
                               (prev, current) => new Tuple<TSource, TSource>(prev == null ? null : prev.Item2, current));
        }

        public static IObservable<Tuple<DateTimeOffset, Timestamped<TSource>>> WithPreviousTimestamp<TSource>(this IObservable<Timestamped<TSource>> source) 
        {
            return source.Scan((Tuple<DateTimeOffset, Timestamped<TSource>>)null,
                               (prev, current) => new Tuple<DateTimeOffset, Timestamped<TSource>>(prev == null ? new DateTimeOffset() : prev.Item2.Timestamp, current));
        }
    }
}