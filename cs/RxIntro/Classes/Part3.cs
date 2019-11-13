using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    class Part3 : Part
    {
        // **** Side effects ****

        // Issues with side effects
        public override void Example1()
        {
            var letters = Observable.Range(0, 3).Select(i => (char)(i + 65));
            var index = -1; // state to be modified
            var result = letters.Select(
            c =>
            {
                index++;
                return c;
            });
            result.Subscribe(
                c => Console.WriteLine("Received {0} at index {1}", c, index),
                () => Console.WriteLine("completed"));

            // Outputs below are nonsense
            result.Subscribe(
                c => Console.WriteLine("Also received {0} at index {1}", c, index),
                () => Console.WriteLine("2nd completed"));
        }

        // Composing data in a pipeline
        // The preferred way of capturing state is to introduce it to the pipeline.
        public override void Example2()
        {
            // use the overload of Select provide index
            var source = Observable.Range(0, 3);
            var result = source.Select((idx, value) => new
            {
                Index = idx,
                Letter = (char)(value + 65)
            });
            result.Subscribe(
                x => Console.WriteLine("Received {0} at index {1}", x.Letter, x.Index),
                () => Console.WriteLine("completed"));
            result.Subscribe(
                x => Console.WriteLine("Also received {0} at index {1}", x.Letter, x.Index),
                () => Console.WriteLine("2nd completed"));

            // use Scan, to introduce a seed of the pipe
            source.Scan(new
            {
                Index = -1,
                Letter = new char()
            },
            (acc, value) => new
            {
                Index = acc.Index + 1,
                Letter = (char)(value + 65)
            }).Dump("scan");
        }

        // Do
        // do side effects explicitly
        public override void Example3()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Take(3);
            var result = source.Do(
                i => Log(i),
                ex => Log(ex),
                () => Log());
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("completed"));
            Thread.Sleep(3100);
        }

        // Encapsulating with AsObservable
        public override void Example4()
        {
            var repo = new LeakyLetterRepo();
            var good = repo.GetLetters();
            var evil = repo.GetLetters();

            good.Subscribe(Console.WriteLine);

            var asSubject = evil as ISubject<string>;
            if (asSubject != null)
            {
                //So naughty, 1 is not a letter!
                asSubject.OnNext("1");
            }
            else
            {
                Console.WriteLine("could not sabotage");
            }
        }

        // Mutable elements cannot be protected
        public override void Example5()
        {
            var source = new Subject<Account>();
            //Evil code. It modifies the Account object.
            source.Subscribe(account => account.Name = "Garbage");
            //unassuming well behaved code
            source.Subscribe(
            account => Console.WriteLine("{0} {1}", account.Id, account.Name),
            () => Console.WriteLine("completed"));
            source.OnNext(new Account { Id = 1, Name = "Microsoft" });
            source.OnNext(new Account { Id = 2, Name = "Google" });
            source.OnNext(new Account { Id = 3, Name = "IBM" });
            source.OnCompleted();
        }

        // **** Leaving the monad ****           
        // ForEach             
        public override void Example6()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
            // ForEach will block the current thread until the sequence completes
            source.ForEachAsync(i => Console.WriteLine("received {0} @ {1}", i, DateTime.Now)).Wait();
            Console.WriteLine("completed @ {0}", DateTime.Now);
        }

        // ToEnumerable
        public override void Example7()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period).Take(5);
            var result = source.ToEnumerable();
            foreach (var value in result)
            {
                Console.WriteLine(value);
            }
            Console.WriteLine("done");
        }

        // To a single collection
        // ToArray, ToList, ToDictionary, ToLookup
        // ToDictionary method overloads mandate that all keys should be unique. If a duplicate key
        // is found, it terminate the sequence with a DuplicateKeyException. On the other hand, the
        // ILookup<TKey, TElement> is designed to have multiple values grouped by the key.
        public override void Example8()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period).Take(5);
            var result = source.ToArray();
            result.Subscribe(arr =>
                {
                    Console.WriteLine("Received array");
                    foreach (var value in arr)
                    {
                        Console.WriteLine(value);
                    }
                },
                () => Console.WriteLine("Completed")
            );
            Console.WriteLine("Subscribed");
            Thread.Sleep(1100);
        }

        // ToTask
        // will ignore multiple values, only returning the last value
        public override void Example9()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
            var result = source.ToTask(); //Will arrive in 5 seconds. 
            Console.WriteLine(result.Result);
        }

        // ToEvent<T>, ToEventPattern
        public void Example10()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
            var result = source.ToEvent();
            result.OnNext += val => Console.WriteLine(val);

            var source2 = Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(i => new EventPattern<MyEventArgs>(this, new MyEventArgs(i)));

            source2.ToEventPattern()
                .OnNext += (sender, eventArgs) => Console.WriteLine($"ToEventPattern: {eventArgs.Value}");

            Thread.Sleep(5100);
        }

        // **** Advanced error handling ****

        // ** Control flow constructs **
        // Catch
        // Catch allows you to intercept a specific Exception type and then continue with another sequence.
        public void Example11()
        {
            // Swallowing exceptions
            var source = new Subject<int>();
            source.Catch(Observable.Empty<int>())
                .Dump("Catch 1");
            source.OnNext(1);
            source.OnNext(2);
            source.OnError(new Exception("Fail!"));
            source.OnNext(3); // not print

            // handle kind of exceptions
            var source2 = new Subject<int>();
            source2.Catch<int, TimeoutException>(tx => Observable.Return(-1))
                .Dump("Catch 2");
            source2.OnNext(1);
            source2.OnNext(2);
            source2.OnError(new TimeoutException());
            // source2.OnError(new ArgumentException("Fail!")); // this will not be caught
            source2.OnNext(3); // not print
        }

        // Finally
        public void Example12()
        {
            var source = new Subject<int>();
            var result = source.Finally(() => Console.WriteLine("Finally action ran"));
            result.Dump("Finally");
            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
            source.OnCompleted();

            // The OnError handler in Subsribe is required
            /* var source = new Subject<int>();
            var result = source.Finally(() => Console.WriteLine("Finally"));
            result.Subscribe(
                Console.WriteLine,
                //Console.WriteLine, // This line is required for finally to be run
                () => Console.WriteLine("Completed"));
            source.OnNext(1);
            source.OnNext(2);
            source.OnNext(3);
            //Brings the app down. Finally action is not called.
            source.OnError(new Exception("Fail")); */
        }

        // Using
        public void Example13()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1));
            var result = Observable.Using(
                () => new TimeIt("Subscription Timer"),
                timeIt => source);
            result.Take(5).Dump("Using");
            Thread.Sleep(5100);
        }

        // OnErrorResumeNext 
        // It will swallow exceptions quietly and can leave your program in an unknown state. 
        // Generally, this will make your code harder to maintain and debug.
        // public void Example()

        // Retry
        // public void Example()


        // **** Combining sequences ****

        // ** Sequential concatenation **
        // Concat
        public void Example14()
        {
            //Generate values 0,1,2 
            var s1 = Observable.Range(0, 3);
            //Generate values 5,6,7,8,9 
            var s2 = Observable.Range(5, 5);
            s1.Concat(s2)
            .Subscribe(Console.WriteLine);
        }

        public IEnumerable<IObservable<long>> GetSequences()
        {
            Console.WriteLine("GetSequences() called");
            Console.WriteLine("Yield 1st sequence");
            yield return Observable.Create<long>(o =>
            {
                Console.WriteLine("1st subscribed to");
                return Observable.Timer(TimeSpan.FromMilliseconds(500))
                        .Select(i => 1L)
                        .Subscribe(o);
            });
            Console.WriteLine("Yield 2nd sequence");
            yield return Observable.Create<long>(o =>
            {
                Console.WriteLine("2nd subscribed to");
                return Observable.Timer(TimeSpan.FromMilliseconds(300))
                        .Select(i => 2L)
                        .Subscribe(o);
            });
            Thread.Sleep(1000);     //Force a delay
            Console.WriteLine("Yield 3rd sequence");
            yield return Observable.Create<long>(o =>
            {
                Console.WriteLine("3rd subscribed to");
                return Observable.Timer(TimeSpan.FromMilliseconds(100))
                        .Select(i => 3L)
                        .Subscribe(o);
            });
            Console.WriteLine("GetSequences() complete");
        }

        public void Example15()
        {
            GetSequences().Concat().Dump("Concat");
            Thread.Sleep(2000);
        }

        // Repeat
        public void Example16()
        {
            var source = Observable.Range(0, 3);
            var result = source.Repeat(3);
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
        }

        // StartWith
        public void Example17()
        {
            // Generate values 0,1,2 
            var source = Observable.Range(0, 3);
            // prefix the values -3, -2 and -1 to the sequence [0,1,2]
            var result = source.StartWith(-3, -2, -1);
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
        }

        // ** Concurrent sequences **

        // Amb -> Ambiguous
        // return values from the sequence that is first to produce values, 
        // ignore the other sequences
        public void Example18()
        {
            var s1 = new Subject<int>();
            var s2 = new Subject<int>();
            var s3 = new Subject<int>();
            var result = Observable.Amb(s1, s2, s3);
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
            s1.OnNext(1);
            s2.OnNext(2);
            s3.OnNext(3);
            s1.OnNext(1);
            s2.OnNext(2);
            s3.OnNext(3);
            s1.OnCompleted();
            s2.OnCompleted();
            s3.OnCompleted();

            // If we comment out the first s1.OnNext(1); then s2 would produce values first 
            // and the marble diagram would look like this.
            // s1 ---1--|
            // s2 -2--2--|
            // s3 --3--3--|
            // r  -2--2--|
        }

        public void Example19()
        {
            // the evaluation of the outer (IEnumerable) sequence is eager
            // the inner observable sequences are not subscribed to until the outer sequence has yielded them all
            GetSequences().Amb().Dump("Amb");
            Thread.Sleep(1500);

            // s1-----1|
            // s2---2|
            // s3-3|
            // rs-3|
        }

        // Merge
        public void Example20()
        {
            // s1 --1--1--1--|
            // s2 ---2---2---2|
            // r  --12-1-21--2|

            //Generate values 0,1,2 
            var s1 = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(3);
            //Generate values 100,101,102,103,104 
            var s2 = Observable.Interval(TimeSpan.FromMilliseconds(150))
                .Take(5)
                .Select(i => i + 100);
            s1.Merge(s2).Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
            Thread.Sleep(1000);
        }

        public void Example21()
        {
            GetSequences().Merge().Dump("Merge 1");
            Thread.Sleep(1000);

            // make evaluation of the outer (IEnumerable) sequence eager, like Amb
            GetSequences().ToList().Merge().Dump("Merge 2");
            Thread.Sleep(2000);
        }

        // Switch
        public void Example22()
        {
            GetSequences().ToList().ToObservable().Switch().Dump("Switch");
            Thread.Sleep(2000);

            // Switch will subscribe to the outer sequence and as each inner sequence is yielded it 
            // will subscribe to the new inner sequence and dispose of the subscription to the previous
            // inner sequence.

            // SV--1---2---3---|
            // S1  -1--1--1--1|
            // S2      --2-2--2--2|
            // S3          -3--3|
            // RS --1--1-2-23--3|            
        }

        // **** Pairing sequences ****

        // CombineLatest
        public void Example23()
        {
            // How it works
            // N---1---2---3---
            // L--a------bc----
            // R---1---2-223---
            //     a   a bcc   

            // Example Usage
            // Yields true when both systems are up, and only on change of status
            // IObservable<bool> webServerStatus = GetWebStatus();
            // IObservable<bool> databaseStatus = GetDBStatus();
            // var systemStatus = webServerStatus.CombineLatest(databaseStatus,
            //         (webStatus, dbStatus) => webStatus && dbStatus)
            //     .DistinctUntilChanged()
            //     .StartWith(false);
        }

        // Zip
        public void Example24()
        {
            // nums  ----0----1----2| 
            // chars --a--b--c--d--e--f| 
            // result----0----1----2|
            //           a    b    c|            

            //Generate values 0,1,2 
            var nums = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(3);
            //Generate values a,b,c,d,e,f 
            var chars = Observable.Interval(TimeSpan.FromMilliseconds(150)).Take(6)
                .Select(i => Char.ConvertFromUtf32((int)i + 97));
            //Zip values together
            nums.Zip(chars, (lhs, rhs) => new { Left = lhs, Right = rhs }).Dump("Zip");

            Thread.Sleep(1000);
        }

        // And-Then-When
        public void Example25()
        {
            //To Zip three sequences together, you can either use Zip methods chained together like this:
            var one = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
            var two = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(10);
            var three = Observable.Interval(TimeSpan.FromMilliseconds(150)).Take(14);
            //lhs represents 'Left Hand Side'
            //rhs represents 'Right Hand Side'
            var zippedSequence = one
                .Zip(two, (lhs, rhs) => new { One = lhs, Two = rhs })
                .Zip(three, (lhs, rhs) => new { One = lhs.One, Two = lhs.Two, Three = rhs });
            zippedSequence.Dump("zippedSequence");
            Thread.Sleep(5100);

            // Or perhaps use the nicer syntax of the And/Then/When:
            var pattern = one.And(two).And(three);
            var plan = pattern.Then((first, second, third) => new { One = first, Two = second, Three = third });
            var zippedSequence2 = Observable.When(plan);
            zippedSequence2.Dump("zippedSequence 2");
            Thread.Sleep(5100);

            // This can be further reduced, if you prefer, to:
            Observable.When(
                one.And(two)
                    .And(three)
                    .Then((first, second, third) =>
                new
                {
                    One = first,
                    Two = second,
                    Three = third
                })
            ).Dump("zippedSequence 3");
            Thread.Sleep(5100);
        }

        // **** Time-shifted sequences ****

        // Buffer
        public void Example26()
        {
            var idealBatchSize = 15;
            var maxTimeDelay = TimeSpan.FromSeconds(3);
            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(10)
                .Concat(Observable.Interval(TimeSpan.FromSeconds(0.01)).Take(100));
            source.Buffer(maxTimeDelay, idealBatchSize).Subscribe(
                buffer => Console.WriteLine("Buffer of {1} @ {0}", DateTime.Now, buffer.Count),
                () => Console.WriteLine("Completed"));
            Thread.Sleep(12100);
        }

        // Overlapping buffers
        public void Example27()
        {
            // There are three interesting things you can do with overlapping buffers:
            // Standard behavior (skip = count)
            //      Ensure that each new buffer only has new data
            // Overlapping behavior (skip < count)
            //      Ensure that current buffer includes some or all values from previous buffer
            // Skip behavior (skip > count)
            //      Ensure that each new buffer not only contains new data exclusively, but also ignores one or more values since the previous buffer

            var source = Observable.Interval(TimeSpan.FromMilliseconds(80)).Take(10);

            // Overlapping behavior             
            source.Buffer(3, 1).Subscribe(buffer =>
                {
                    Console.WriteLine("--Buffered values");
                    foreach (var value in buffer)
                    {
                        Console.WriteLine(value);
                    }
                },
                () => Console.WriteLine("Completed"));
            Thread.Sleep(1000);

            // Standard behavior             
            source.Buffer(3, 3).Subscribe(buffer =>
                {
                    Console.WriteLine("--Buffered values");
                    foreach (var value in buffer)
                    {
                        Console.WriteLine(value);
                    }
                },
                () => Console.WriteLine("Completed"));
            Thread.Sleep(1000);

            // Skip behavior             
            source.Buffer(3, 5).Subscribe(buffer =>
                {
                    Console.WriteLine("--Buffered values");
                    foreach (var value in buffer)
                    {
                        Console.WriteLine(value);
                    }
                },
                () => Console.WriteLine("Completed"));
            Thread.Sleep(1000);
        }

        // Overlapping buffers by time
        public void Example28()
        {
            // Similar as above but with time shift instead of count shift 
            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(10);
            var overlapped = source.Buffer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1));
            var standard = source.Buffer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
            var skipped = source.Buffer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5));
        }

        // Delay -> time-shift an entire sequence
        // Note: Delay will not time-shift OnError notifications. These will be propagated immediately.
        public void Example29()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5).Timestamp();
            var delay = source.Delay(TimeSpan.FromSeconds(2));

            source.Subscribe(
                value => Console.WriteLine("source : {0}", value),
                () => Console.WriteLine("source Completed"));
            delay.Subscribe(
                value => Console.WriteLine("delay : {0}", value),
                () => Console.WriteLine("delay Completed"));
            Thread.Sleep(7100);
        }

        // Sample -> takes the last value for every specified TimeSpan        
        // ThrottleFirst -> takes the first value for every specified TimeSpan https://github.com/dotnet/reactive/issues/395#issuecomment-296172872        
        public void Example30()
        {
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(150));
            interval.Timestamp().Dump("interval");
            interval.ThrottleFirst(TimeSpan.FromSeconds(1), Scheduler.Default).Dump("ThrottleFirst");
            interval.Sample(TimeSpan.FromSeconds(1)).Dump("Sample");
            Thread.Sleep(3100);

            // ThrottleFirst behaves similar to sample, the difference is 
            // ThrottleFirst emit immediately as the source emit, then reset timer
            // Sample first emit until the specified TimeSpan, then reset timer
            // To be consistent, better to be called SampleFirst, I think...         
        }

        // Throttle -> reset timer when new item arrived, emit last when timer elapsed or source completed
        public void Example31()
        {
            new int[] { 100, 300, 400, 500, 600, 1000 }.ToObservable()
                .SelectMany(v => Observable.Timer(TimeSpan.FromMilliseconds(v)).Select(w => v))
                .Throttle(TimeSpan.FromMilliseconds(300))
                .Dump("Throttle");
            Thread.Sleep(3100);

            // The function naming are messed up...
            // in some other rx frameworks, this is called debounce
            // https://weblog.west-wind.com/posts/2017/jul/02/debouncing-and-throttling-dispatcher-events

            // Debouncing enforces that a function not be called again until a certain amount of 
            // time has passed without it being called. As in "execute this function only if 100 
            // milliseconds have passed without it being called".        
            // This is Throttle in Rx.Net

            // Throttling enforces a maximum number of times a function can be called over time. 
            // As in "execute this function at most once every 100 milliseconds".
            // This looks like Sample in Rx.Net
        }

        // Timeout
        public void Example32()
        {
            // time out by interval
            var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(10)
                .Concat(Observable.Interval(TimeSpan.FromSeconds(2)));
            source.Timeout(TimeSpan.FromSeconds(1)).Dump("Timeout");
            Thread.Sleep(2100);
        }
        public void Example33()
        {
            // time out by an absolute time
            var dueDate = DateTimeOffset.UtcNow.AddSeconds(4);
            var source = Observable.Interval(TimeSpan.FromSeconds(1));
            source.Timeout(dueDate).Dump("Timeout");
            Thread.Sleep(4100);
        }

        // **** Hot and Cold observables ****
        // Hot: Sequences that are active and produce notifications regardless of subscriptions.
        // Cold: Sequences that are passive and start producing notifications on request (when subscribed to)

        // Cold observables 
        public void Example34()
        {
            // Observable.Create -> always return cold observable
            // const string connectionString = "...";
            // var sequence = Observable.Create<string>(
            // o =>
            // {
            //     using (var conn = new SqlConnection(connectionString))
            //     using (var cmd = new SqlCommand("Select Name FROM SalesLT.ProductModel", conn))
            //     {
            //         conn.Open();
            //         SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            //         while (reader.Read())
            //         {
            //             o.OnNext(reader.GetString(0));
            //         }
            //         o.OnCompleted();
            //         return Disposable.Create(() => Console.WriteLine("--Disposed--"));
            //     }
            // });

            // Subscribing to the returned IObservable<T> will however invoke the create delegate 
            // which connects to the database and publish all the values, no matter what the 
            // subscriber asks

            // Interval is cold
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period);
            observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            Thread.Sleep(period);
            observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));
            Thread.Sleep(4100);
        }

        // Hot observables
        public void Example35()
        {
            // Examples are UI Events and Subjects
        }

        // Publish and Connect
        public void Example36()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period).Publish();

            // calling Connect() it will subscribe to the underlying
            // make the observable hot
            observable.Connect();

            // subscribe before the first item is published
            observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            Thread.Sleep(1200);

            // subscribe after the first item is published
            observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));
            Thread.Sleep(4100);
        }

        public void Example37()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period).Publish();
            observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            Thread.Sleep(period);
            observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));

            // At this point, the underlying is not subscribed
            System.Console.WriteLine("About to connect");
            observable.Connect();
            Thread.Sleep(4100);
        }

        public void Example38()
        {
            // Disposal of connections and subscriptions
            // By disposing of the 'connection', you can turn the sequence on and off 
            // Connect() to toggle it on, disposing toggles it off
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period).Publish();
            observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));
            var exit = false;
            while (!exit)
            {
                Console.WriteLine("Press enter to connect, esc to exit.");
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    var connection = observable.Connect(); //--Connects here--
                    Console.WriteLine("Press any key to dispose of connection.");
                    Console.ReadKey();
                    connection.Dispose(); //--Disconnects here--
                }
                if (key.Key == ConsoleKey.Escape)
                {
                    exit = true;
                }
            }
        }

        public void Example39()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period)
                .Do(l => Console.WriteLine("Publishing {0}", l)) //Side effect to show it is running
                .Publish();
            var connection = observable.Connect();

            Console.WriteLine("Press any key to subscribe");
            Console.ReadKey();
            var subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));

            Console.WriteLine("Press any key to unsubscribe.");
            Console.ReadKey();
            subscription.Dispose();
            
            Console.WriteLine("Press any key to subscribe again");
            Console.ReadKey();
            subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));

            Console.WriteLine("Press any key to unsubscribe again");
            Console.ReadKey();
            subscription.Dispose();

            Console.WriteLine("Press any key to dispose connection");
            Console.ReadKey();
            connection.Dispose();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        public void Example40()
        {
            // RefCount             
            // -> auto connect when # of subscriptions > 0
            // -> auto dispose when # of subscriptions == 0
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period)
                .Do(l => Console.WriteLine("Publishing {0}", l)) //side effect to show it is running
                .Publish()
                .RefCount();
            //observable.Connect(); Use RefCount instead now 
            Console.WriteLine("Press any key to subscribe");
            Console.ReadKey();
            var subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));

            Console.WriteLine("Press any key to unsubscribe.");
            Console.ReadKey();
            subscription.Dispose();

            Console.WriteLine("Press any key to subscribe again");
            Console.ReadKey();
            subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        // ** Other connectable observables **

        // PublishLast
        public void Example41()
        {
            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period)
                .Take(5)
                .Do(l => Console.WriteLine("Publishing {0}", l)) //side effect to show it is running
                .PublishLast();
            observable.Connect();
            Console.WriteLine("Press any key to subscribe");
            Console.ReadKey();
            var subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));
            Console.WriteLine("Press any key to unsubscribe.");
            Console.ReadKey();
            subscription.Dispose();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        // Replay
        public void Example42()
        {
            var period = TimeSpan.FromSeconds(1);
            var hot = Observable.Interval(period)
                .Take(3)
                .Publish();
            hot.Connect();
            var observable = hot.Replay();
            observable.Connect();
            observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            Thread.Sleep(3100);
            observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));
            Console.ReadKey();
        }

        // Multicast
        public void Example43()
        {
            // Implement Publish/Connect mannually
            var period = TimeSpan.FromSeconds(1);
            //var observable = Observable.Interval(period).Publish();
            var observable = Observable.Interval(period);
            var shared = new Subject<long>();
            shared.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
            observable.Subscribe(shared);   //'Connect' the observable.
            Thread.Sleep(2100);
            shared.Subscribe(i => Console.WriteLine("second subscription : {0}", i));
            Thread.Sleep(2100);

            // You can apply subject behavior via the Multicast extension method. This allows you
            // to share or "multicast" an observable sequence with the behavior of a specific subject
            
            // .Publish() = .Multicast(new Subject<T>)
            // .PublishLast() = .Multicast(new AsyncSubject<T>)
            // .Replay() = .Multicast(new ReplaySubject<T>)            
        }
        // public void Example()
        // public void Example()
        // public void Example()
        // public void Example()

        private static void Log(object onNextValue)
        {
            Console.WriteLine("Logging OnNext({0}) @ {1}", onNextValue, DateTime.Now);
        }
        private static void Log(Exception onErrorValue)
        {
            Console.WriteLine("Logging OnError({0}) @ {1}", onErrorValue, DateTime.Now);
        }
        private static void Log()
        {
            Console.WriteLine("Logging OnCompleted()@ {0}", DateTime.Now);
        }
    }

    public class LeakyLetterRepo
    {
        private readonly ReplaySubject<string> _letters;
        public LeakyLetterRepo()
        {
            _letters = new ReplaySubject<string>();
            _letters.OnNext("A");
            _letters.OnNext("B");
            _letters.OnNext("C");
        }

        // below is not good, make is private or use AsObservable()
        public ReplaySubject<string> Letters
        {
            get { return _letters; }
        }

        // Below is the best practice
        // Types become more discoverable if we reduce their surface area to expose only the 
        // features we intend our consumers to use
        public IObservable<string> GetLetters()
        {
            return _letters.AsObservable();
        }
    }

    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MyEventArgs : EventArgs
    {
        private readonly long _value;
        public MyEventArgs(long value)
        {
            _value = value;
        }
        public long Value
        {
            get { return _value; }
        }
    }

    public class TimeIt : IDisposable
    {
        private readonly string _name;
        private readonly Stopwatch _watch;
        public TimeIt(string name)
        {
            _name = name;
            _watch = Stopwatch.StartNew();
        }
        public void Dispose()
        {
            _watch.Stop();
            Console.WriteLine("{0} took {1}", _name, _watch.Elapsed);
        }
    }
}