using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace RxIntro
{
    public static class OX
    {        
        public static IObservable<T> ThrottleFirst<T>(this IObservable<T> source, 
                TimeSpan timespan, IScheduler timeSource)
        {
            return new ThrottleFirstObservable<T>(source, timeSource, timespan);
        }
    }

    sealed class ThrottleFirstObservable<T> : IObservable<T>
    {
        readonly IObservable<T> source;

        readonly IScheduler timeSource;

        readonly TimeSpan timespan;

        internal ThrottleFirstObservable(IObservable<T> source, 
                  IScheduler timeSource, TimeSpan timespan)
        {
            this.source = source;
            this.timeSource = timeSource;
            this.timespan = timespan;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var parent = new ThrottleFirstObserver(observer, timeSource, timespan);
            var d = source.Subscribe(parent);
            parent.OnSubscribe(d);
            return d;
        }

        sealed class ThrottleFirstObserver : IDisposable, IObserver<T>
        {
            readonly IObserver<T> downstream;

            readonly IScheduler timeSource;

            readonly TimeSpan timespan;

            IDisposable upstream;

            bool once;

            double due;

            internal ThrottleFirstObserver(IObserver<T> downstream, 
                    IScheduler timeSource, TimeSpan timespan)
            {
                this.downstream = downstream;
                this.timeSource = timeSource;
                this.timespan = timespan;
            }

            public void OnSubscribe(IDisposable d)
            {
                if (Interlocked.CompareExchange(ref upstream, d, null) != null)
                {
                    d.Dispose();
                }
            }

            public void Dispose()
            {
                var d = Interlocked.Exchange(ref upstream, this);
                if (d != null && d != this)
                {
                    d.Dispose();
                }
            }

            public void OnCompleted()
            {
                downstream.OnCompleted();
            }

            public void OnError(Exception error)
            {
                downstream.OnError(error);
            }

            public void OnNext(T value)
            {
                var now = timeSource.Now.ToUnixTimeMilliseconds();
                if (!once)
                {
                    once = true;
                    due = now + timespan.TotalMilliseconds;
                    downstream.OnNext(value);
                } else if (now >= due)
                {
                    due = now + timespan.TotalMilliseconds;
                    downstream.OnNext(value);
                }

            }
        }
    }
}