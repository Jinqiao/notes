using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace RxIntro
{
    class Part4 : Part
    {

        // **** Scheduling and threading ****

        // Rx is single-threaded by default
        public override void Example1()
        {
            // Belows shows that each OnNext was called back on the same thread that it was notified on
            Console.WriteLine($"Starting on threadId:{Tid()}");
            var subject = new Subject<object>();
            subject.Subscribe(
                o => Console.WriteLine($"Received {o} on threadId:{Tid()}"));
            ParameterizedThreadStart notify = obj =>
            {
                Console.WriteLine($"OnNext({obj}) on threadId:{Tid()}");
                subject.OnNext(obj);
            };
            notify(1);
            new Thread(notify).Start(2);
            new Thread(notify).Start(3);
        }

        // SubscribeOn and ObserveOn
        // 1. The invocation of the subscription -> SubscribeOn
        // 2. The observing of notifications     -> ObserveOn   
        public override void Example2()
        {
            // without SubscribeOn(Scheduler.ThreadPool), everything is sequential and single threaded
            // Scheduler.ThreadPool is obsolete, new api use 
            Console.WriteLine($"Starting on threadId:{Tid()}");
            var source = Observable.Create<int>(
            o =>
            {
                Console.WriteLine($"Invoked on threadId:{Tid()}");
                o.OnNext(1);
                o.OnNext(2);
                o.OnNext(3);
                o.OnCompleted();
                Console.WriteLine($"Finished on threadId:{Tid()}");
                return Disposable.Empty;
            });
            Console.WriteLine($"Created Observable source");
            source
            .Subscribe(
                o => Console.WriteLine($"Received {o} on threadId:{Tid()}"),
                () => Console.WriteLine($"OnCompleted on threadId:{Tid()}"));

            // below will be called after the sequence complete
            Console.WriteLine($"Subscribed on threadId:{Tid()}");
        }

        public override void Example3()
        {
            // with SubscribeOn(Scheduler.Default)
            Console.WriteLine($"Starting on threadId:{Tid()}");
            var source = Observable.Create<int>(
            o =>
            {
                Console.WriteLine($"Invoked on threadId:{Tid()}");
                o.OnNext(1);
                o.OnNext(2);
                o.OnNext(3);
                o.OnCompleted();
                Console.WriteLine($"Finished on threadId:{Tid()}");
                return Disposable.Empty;
            });
            Console.WriteLine($"Created Observable source");
            source
            .SubscribeOn(Scheduler.Default)
            //.ObserveOn(NewThreadScheduler.Default)
            .Subscribe(
                o => Console.WriteLine($"Received {o} on threadId:{Tid()}"),
                () => Console.WriteLine($"OnCompleted on threadId:{Tid()}"));

            // below will be called before sequence emiting
            Console.WriteLine($"Subscribed on threadId:{Tid()}");
            Thread.Sleep(1000);
        }

        // Schedulers    
        // NewThread (NewThreadScheduler.Default)
        // ThreadPool (ThreadPoolScheduler.Instance) 
        // Immediate, CurrentThread, TaskPool, DispatcherScheduler.Instance

        // Concurrency pitfalls        
        // Lock-ups                
        public override void Example4()
        {
            var sequence = new Subject<int>();
            Console.WriteLine("Next line should lock the system.");
#pragma warning disable 0618
            var value = sequence.First();
#pragma warning restore 0618
            sequence.OnNext(1);
            Console.WriteLine("I can never execute....");
        }

        public async Task Example101()
        {
            // This will also dead lock or await forever
            var sequence = new Subject<int>();
            Console.WriteLine("Next line should lock the system.");
            var value = await sequence.FirstAsync();
            sequence.OnNext(1);
            Console.WriteLine("I can never execute....");
        }

        // Best practices:
        // 1. Only the final subscriber should be setting the scheduling
        // 2. Avoid using blocking calls: e.g. First, Last and Single        
        // For UI app:
        // Subscribe on a Background thread; Observe on the Dispatcher

        // ** Advanced features of schedulers **
        // Passing state
        public override void Example5()
        {
            // use a closure -> make below non-deterministic
            var myName = "Lee";
            NewThreadScheduler.Default.Schedule(
                () => Console.WriteLine("myName = {0}", myName));
            Thread.Sleep(1);
            myName = "John";//What will get written to the console?
        }

        public override void Example6()
        {
            // pass state to scheduler
            var myName = "Lee";
            NewThreadScheduler.Default.Schedule(myName, (_, state) =>
            {
                Console.WriteLine(state);
                return Disposable.Empty;
            });
            myName = "John";
        }

        public override void Example7()
        {
            // below modify shared state again... try avoiding such code
            var list = new List<int>();
            NewThreadScheduler.Default.Schedule(list, (_, state) =>
            {
                Console.WriteLine(state.Count);
                return Disposable.Empty;
            });
            // Thread.Sleep(10);
            list.Add(1);
        }

        // Future scheduling
        public override void Example8()
        {
            var delay = TimeSpan.FromSeconds(3);
            Console.WriteLine($"Before schedule at {DateTime.Now:o}");
            NewThreadScheduler.Default.Schedule(delay,
                () => Console.WriteLine($"Inside schedule at {DateTime.Now:o}"));
            Console.WriteLine($"After schedule at  {DateTime.Now:o}");

            // No need to wait here, the app won't terminated until the
            // scheduled work finished, this means the NewThreadScheduler
            // use foreground thread, see example 201 below
        }

        public void Example201()
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/threading/foreground-and-background-threads
            // Background threads are identical to foreground threads with one exception: 
            // a background thread does not keep the managed execution environment running.
            // Once all foreground threads have been stopped in a managed process (where the 
            // .exe file is a managed assembly), the system stops all background threads and 
            // shuts down. 

            // Threads that belong to the managed thread pool (that is, threads whose IsThreadPoolThread
            // property is true) are background threads. All threads that enter the managed execution 
            // environment from unmanaged code are marked as background threads. All threads generated 
            // by creating and starting a new Thread object are by default foreground threads.
            Console.WriteLine($"App started, TID:{Tid()}");
            Thread thread = new Thread(() =>
            {
                Console.WriteLine($"Thread started, TID: {Tid()}");
                Thread.Sleep(1000);
                Console.WriteLine($"Thread finished, TID: {Tid()}");
            });
            //thread.IsBackground = true;            
            thread.Start();
            Console.WriteLine($"Thread started, TID: {Tid()}");
        }

        // Cancelation
        public override void Example9()
        {
            var delay = TimeSpan.FromSeconds(3);
            Console.WriteLine("Before schedule at {0:o}", DateTime.Now);
            var token = NewThreadScheduler.Default.Schedule(delay,
                () => Console.WriteLine("Inside schedule at {0:o}", DateTime.Now));
            Console.WriteLine("After schedule at  {0:o}", DateTime.Now);

            // cancel the scheduled work
            token.Dispose();
            // Although the scheduled action did not run, the scheduled delay got 
            // run in a foreground thread if using NewThreadScheduler.Default            
        }

        public void Example10()
        {
            IDisposable Work(IScheduler scheduler, List<int> list)
            {
                var cancelTokenSource = new CancellationTokenSource();
                var cancelToken = cancelTokenSource.Token;
                Task.Run(() =>
                {
                    Console.WriteLine();
                    for (int i = 0; i < 1000; i++)
                    {
                        var sw = new SpinWait();
                        for (int j = 0; j < 500; j++) sw.SpinOnce();
                        Console.Write(".");
                        list.Add(i);
                        if (cancelToken.IsCancellationRequested)
                        {
                            Console.WriteLine("Cancelation requested");
                            //cancelToken.ThrowIfCancellationRequested();
                            return;
                        }
                    }
                }, cancelToken);
                return Disposable.Create(() => cancelTokenSource.Cancel());
            }

            Console.WriteLine("Enter to quit:");
            var token = NewThreadScheduler.Default.Schedule(new List<int>(), Work);
            Console.ReadLine();
            Console.WriteLine("Cancelling...");
            token.Dispose();
            Console.WriteLine("Cancelled");

            // below wait the Task.Run() to finish
            Thread.Sleep(1000);

            // here, we have introduced explicit use of Task. We can avoid explicit usage of a 
            // concurrency model if we use the Rx recursive scheduler features instead.
        }

        // Recursion
        public void Example11()
        {
            Action<Action> work = (Action self) =>
            {
                Console.WriteLine("Running");
                Thread.Sleep(100);
                self();
            };
            var token = NewThreadScheduler.Default.Schedule(work);
            Console.ReadLine();
            Console.WriteLine("Cancelling");
            token.Dispose();
            Console.WriteLine("Cancelled");

            // Note that we didn't have to write any cancellation code in our delegate. Rx handled
            // the looping and checked for cancellation on our behalf. Brilliant! Unlike simple 
            // recursive methods in C#, we are also protected from stack overflows, as Rx provides
            // an extra level of abstraction. Indeed, Rx takes our recursive method and transforms
            // it to a loop structure instead.            
        }

        // ** Schedulers in-depth **

        // ImmediateScheduler -> Scheduler.Immediate
        // CurrentThreadScheduler -> CurrentThreadScheduler.Instance or Scheduler.CurrentThread

        // If you schedule an action that itself schedules an action, the CurrentThreadScheduler 
        // will queue the inner action to be performed later; in contrast, the ImmediateScheduler
        // would start working on the inner action straight away.        

        // If you schedule the action to be invoked in the future:
        // the ImmediateScheduler will use Thread.Sleep (synchronous)        
        // the CurrentThreadScheduler will use a timer (asynchronous)        

        public void Example12()
        {
            System.Console.WriteLine("Scheduler.CurrentThread");
            ScheduleTasks(Scheduler.CurrentThread);

            System.Console.WriteLine("Scheduler.Immediate");
            ScheduleTasks(Scheduler.Immediate);
        }
        public void Example13()
        {
            System.Console.WriteLine("Scheduler.CurrentThread");
            ScheduleTasks(Scheduler.CurrentThread, TimeSpan.FromSeconds(1));

            System.Console.WriteLine("Scheduler.Immediate");
            ScheduleTasks(Scheduler.Immediate, TimeSpan.FromSeconds(1));
        }

        // EventLoopScheduler   
        // like the CurrentThreadScheduler, but we can specify a thread to use
        public void Example14()
        {
            var eventLoopScheduler = new EventLoopScheduler();
            eventLoopScheduler.Schedule(
                () =>
                {
                    Console.WriteLine("Hello from EventLoopScheduler");
                    Console.WriteLine($"Thread.CurrentThread.IsBackground: {Thread.CurrentThread.IsBackground}");
                    Console.WriteLine($"Thread.CurrentThread.IsThreadPoolThread: {Thread.CurrentThread.IsThreadPoolThread}");
                });
            Thread.Sleep(1000);

            var foregroundEventLoopScheduler = new EventLoopScheduler(threadStart => new Thread(threadStart));
            foregroundEventLoopScheduler.Schedule(
                () =>
                {
                    Console.WriteLine("Hello from ForegroundEventLoopScheduler");
                    Console.WriteLine($"Thread.CurrentThread.IsBackground: {Thread.CurrentThread.IsBackground}");
                });

            // Wait some time then dispose, otherwise app will keep running
            Thread.Sleep(1000);
            foregroundEventLoopScheduler.Dispose();
        }

        // NewThread, ThreadPool and TaskPool
        public void Example15()
        {
            Console.WriteLine($"Starting on thread :{Tid()}");

            // Actions are run on same threads
            var eventLoopScheduler = new EventLoopScheduler();
            eventLoopScheduler.Schedule("A", OuterAction);
            eventLoopScheduler.Schedule("B", OuterAction);
            Thread.Sleep(1000);

            // Actions are run on 2 different threads 
            // thread affinity is deterministic, but time to run is not
            var newThreadScheduler = NewThreadScheduler.Default;
            newThreadScheduler.Schedule("C", OuterAction);
            newThreadScheduler.Schedule("D", OuterAction);
            Thread.Sleep(1000);

            // Actions are run on many different threads 
            // neither thread affinity nor ime to run is deterministic            
            // For requests that are scheduled as soon as possible, the action is just sent to 
            // ThreadPool.QueueUserWorkItem. For requests that are scheduled in the future, a 
            // System.Threading.Timer is used.
            var poolBasedScheduler = Scheduler.Default;
            poolBasedScheduler.Schedule("E", OuterAction);
            poolBasedScheduler.Schedule("F", OuterAction);
            Thread.Sleep(1000);

            // Same as Scheduler.Default                        
            var taskPoolScheduler = TaskPoolScheduler.Default;
            taskPoolScheduler.Schedule("G", OuterAction);
            taskPoolScheduler.Schedule("H", OuterAction);
            Thread.Sleep(1000);

            // below may be useful, as by default TaskPool will schedule task long running
            // https://stackoverflow.com/a/26165520
            TaskPoolScheduler.Default.DisableOptimizations(typeof(ISchedulerLongRunning));
        }

        // Best Practices:
        // UI Applications
        // * The final subscriber is normally the presentation layer and should control the scheduling.
        // * Observe on the DispatcherScheduler to allow updating of ViewModels
        // * Subscribe on a background thread to prevent the UI from becoming unresponsive
        //   ** If the subscription will not block for more than 50ms then
        //      *** Use the TaskPoolScheduler if available or ThreadPoolScheduler
        //   ** If any part of the subscription could block for longer than 50ms, then you should use the NewThreadScheduler.

        // Service layer
        // * If your service is reading data from a queue of some sort, consider using a 
        //   dedicated EventLoopScheduler. This way, you can preserve order of events
        // * If processing an item is expensive (>50ms or requires I/O), then consider 
        //   using a NewThreadScheduler
        // * If you just need the scheduler for a timer, e.g. for Observable.Interval or 
        //   Observable.Timer, then favor the TaskPool


        // **** Sequences of coincidence ****

        // Window
        // The Window operators are very similar to the Buffer operators; they only really differ 
        // by their return type. Where Buffer would take an IObservable<T> and return an 
        // IObservable<IList<T>>, the Window operators return an IObservable<IObservable<T>>. 
        // It is also worth noting that the Buffer operators will not yield their buffers until 
        // the window closes.
        public void Example16()
        {
            var windowIdx = 0;
            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(10);
            source.Window(3)
            .Subscribe(window =>
                {
                    Console.WriteLine("--Starting new window");
                    window.Dump("Window" + windowIdx++);
                },
                () => Console.WriteLine("Completed")
            );

            Thread.Sleep(11000);
        }

        // Customizing windows
        public void Example17()
        {
            var windowIdx = 0;
            bool completed = false;
            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(10);
            var closer = new Subject<Unit>();
            // use windowClosingSelector 
            // windows close when the sequence from the windowClosingSelector produces a value. 
            // The value is disregarded so it doesn't matter what type the sequence values are    
            source.Window(() => closer)
            .Subscribe(window =>
                {
                    Console.WriteLine("--Starting new window");
                    window.Dump("Window" + windowIdx++);
                },
                () => {
                    Console.WriteLine("Completed");
                    completed = true;
                }
            );
            var input = "";
            while (input != "exit" && !completed)
            {
                input = Console.ReadLine();
                closer.OnNext(Unit.Default);
            }

            // There's another overload allows us to create potentially overlapping windows            
        }

        // Join -> join sequences by intersecting windows
        // Params:
        // this IObservable<TLeft> left
        // IObservable<TRight> right 
        // Func<TLeft, IObservable<TLeftDuration>> leftDurationSelector 
        // Func<TRight, IObservable<TRightDuration>> rightDurationSelector 
        // Func<TLeft, TRight, TResult> resultSelector 

        //public void Example(){}
        //public void Example(){}
        //public void Example(){}
        //public void Example(){}
        //public void Example(){}
        //public void Example(){}
        //public void Example(){}
        //public void Example(){}
        //public void Example(){}

        private static int Tid()
        {
            return Thread.CurrentThread.ManagedThreadId;
        }

        private static void ScheduleTasks(IScheduler scheduler)
        {
            Action leafAction = () => Console.WriteLine("----leafAction.");
            Action innerAction = () =>
            {
                Console.WriteLine("--innerAction start.");
                scheduler.Schedule(leafAction);
                Console.WriteLine("--innerAction end.");
            };
            Action outerAction = () =>
            {
                Console.WriteLine("outer start.");
                scheduler.Schedule(innerAction);
                Console.WriteLine("outer end.");
            };
            scheduler.Schedule(outerAction);
        }

        private static void ScheduleTasks(IScheduler scheduler, TimeSpan delay)
        {
            Action leafAction = () => Console.WriteLine("----leafAction.");
            Action innerAction = () =>
            {
                Console.WriteLine("--innerAction start.");
                scheduler.Schedule(delay, leafAction);
                Console.WriteLine("--innerAction end.");
            };
            Action outerAction = () =>
            {
                Console.WriteLine("outer start.");
                scheduler.Schedule(delay, innerAction);
                Console.WriteLine("outer end.");
            };
            scheduler.Schedule(outerAction);
        }

        private static IDisposable OuterAction(IScheduler scheduler, string state)
        {
            Console.WriteLine("{0} start. ThreadId:{1}", state, Thread.CurrentThread.ManagedThreadId);
            scheduler.Schedule(state + ".inner", InnerAction);
            Console.WriteLine("{0} end. ThreadId:{1}", state, Thread.CurrentThread.ManagedThreadId);
            return Disposable.Empty;
        }
        private static IDisposable InnerAction(IScheduler scheduler, string state)
        {
            Console.WriteLine("{0} start. ThreadId:{1}", state, Thread.CurrentThread.ManagedThreadId);
            scheduler.Schedule(state + ".Leaf", LeafAction);
            Console.WriteLine("{0} end. ThreadId:{1}", state, Thread.CurrentThread.ManagedThreadId);
            return Disposable.Empty;
        }
        private static IDisposable LeafAction(IScheduler scheduler, string state)
        {
            Console.WriteLine("{0}. ThreadId:{1}", state, Thread.CurrentThread.ManagedThreadId);
            return Disposable.Empty;
        }
    }
}