using System;
using System.Linq;
using System.Reactive;
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
    class Part2 : Part
    {
        // **** Creating a sequence ****

        // ** Simple factory methods **

        // Observable.Return, Observable.Empty, Observable.Never, Observable.Throw        
        public override void Example1()
        {
            // It unfolds a value of T into an observable sequence
            var singleValue = Observable.Return("Value");
            // it just publishes an OnCompleted notification
            var empty = Observable.Empty<string>();
            // return infinite sequence without any notifications
            var never = Observable.Never<string>();
            // This method creates a sequence with just a single OnError notification containing the exception passed to the factory.
            var throws = Observable.Throw<string>(new Exception());
        }

        // Observable.Create
        // The Create factory method is the preferred way to implement custom observable sequences.
        // Essentially this method allows you to specify a delegate that will be executed anytime 
        // a subscription is made. The IObserver<T> that made the subscription will be passed to 
        // your delegate so that you can call the OnNext/OnError/OnCompleted methods as you need. 
        // Your delegate is a Func that returns an IDisposable. This IDisposable will have its 
        // Dispose() method called when the subscriber disposes from their subscription.

        // The Create method is also preferred over creating custom types that implement the 
        // IObservable interface. There really is no need to implement the observer/observable 
        // interfaces yourself. Rx tackles the intricacies that you may not think of such as thread
        //  safety of notifications and subscriptions.
        public override void Example2()
        {
            var pub = Observable.Create<string>(
                (IObserver<string> observer) =>
                {
                    observer.OnNext("a");
                    observer.OnNext("b");
                    observer.OnCompleted();
                    Thread.Sleep(1000);
                    return Disposable.Create(() => Console.WriteLine("Observer has unsubscribed"));
                    //or can return an Action like 
                    //return () => Console.WriteLine("Observer has unsubscribed"); 
                });

            pub.Subscribe(Console.WriteLine);
        }

        public override void Example3()
        {
            var ob = Observable.Create<string>(
            observer =>
            {
                var timer = new System.Timers.Timer();
                timer.Interval = 1000;
                timer.Elapsed += (s, e) => observer.OnNext("tick");
                timer.Elapsed += OnTimerElapsed;
                timer.Start();
                return Disposable.Empty; // Should dispose timer here!! Or return timer here
            });
            var subscription = ob.Subscribe(Console.WriteLine);
            Thread.Sleep(3100);
            subscription.Dispose();
            Thread.Sleep(3100);
        }

        public override void Example4()
        {
            var ob = Observable.Create<string>(
            observer =>
            {
                var timer = new System.Timers.Timer();
                timer.Interval = 1000;
                timer.Elapsed += (s, e) => observer.OnNext("tick");
                timer.Elapsed += OnTimerElapsed;
                timer.Start();
                return () =>
                {
                    timer.Elapsed -= OnTimerElapsed;
                    timer.Dispose();
                }; // Clean up resources
            });
            var subscription = ob.Subscribe(Console.WriteLine);
            Thread.Sleep(3100);
            subscription.Dispose();
            Thread.Sleep(3100);
        }


        // ** Functional unfolds **
        // Observable.Range
        public override void Example5()
        {
            var range = Observable.Range(10, 15);
            range.Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
        }

        // Observable.Generate
        public override void Example6()
        {
            Observable.Generate(
                1, // initialState
                value => value < 10, // condition
                value => value + 1,  // iterate
                value => value) // resultSelector
            .Subscribe(Console.WriteLine);
        }

        // Observable.Interval
        public override void Example7()
        {
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(250));
            interval.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("completed"));
            Thread.Sleep(3100);
        }

        // Observable.Timer & Observable.Generate
        public override void Example8()
        {
            // Only one value published
            Observable.Timer(TimeSpan.FromSeconds(1))
            .Subscribe(
                x => Console.WriteLine($"Timer 1: {x}"),
                () => Console.WriteLine("Timer 1: completed"));

            // below starts immediately
            Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            .Subscribe(
                x => Console.WriteLine($"Timer 2: {x}"),
                () => Console.WriteLine("Timer 2: completed")); // This will never be called            

            // Generate with the Func<TState, TimeSpan> overload
            var dueTime = TimeSpan.FromSeconds(1);
            var period = TimeSpan.FromMilliseconds(100);
            Observable.Generate(
                0L,
                i => i < 10,
                i => i + 1,
                i => i,
                i => i == 0 ? dueTime : period)
            .Subscribe(x => Console.WriteLine($"Generate: {x}"));
            Thread.Sleep(3100);
        }

        // ** Transit things to IObservable<T> **
        // From delegate, Observable.Start, similar to Task
        public override void Example9()
        {
            // Action overload
            var start = Observable.Start(() =>
            {
                Console.Write("Working away");
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    Console.Write(".");
                }
            });
            start.Subscribe(
                unit => Console.WriteLine("Unit published"),
                () => Console.WriteLine("Action completed"));
            Thread.Sleep(1100);

            var start2 = Observable.Start(() =>
            {
                Console.Write("Working away");
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    Console.Write(".");
                }
                return "Published value";
            });
            start2.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Func completed"));
            Thread.Sleep(1100);
        }

        // From events, Observable.FromEventPattern
        public void Example10()
        {
            //FirstChanceException is EventHandler<FirstChanceExceptionEventArgs>
            var firstChanceException = Observable.FromEventPattern<FirstChanceExceptionEventArgs>(
                h => AppDomain.CurrentDomain.FirstChanceException += h,
                h => AppDomain.CurrentDomain.FirstChanceException -= h);
        }

        // From Task
        public void Example11()
        {
            // Task<T>
            var t2 = Task.Run(() => { return 1; });
            t2.ToObservable()
              .Subscribe(Console.WriteLine);
            Thread.Sleep(1100);

            // There is also an overload that converts a Task (non generic) to an IObservable<Unit>
        }

        // From IEnumerable<T>
        public void Example12()
        {
            Enumerable.Range(0, 5).ToObservable().Subscribe(Console.WriteLine);

            // Above runs syncronously
            Console.WriteLine("Finished");
        }


        // **** Reducing a sequence ****
        // Where
        public void Example13()
        {
            var oddNumbers = Observable.Range(0, 10)
                .Where(i => i % 2 == 0)
                .Subscribe(
                    Console.WriteLine,
                    () => Console.WriteLine("Completed"));
        }

        // Distinct
        public void Example14()
        {
            var subject = new Subject<int>();
            var distinct = subject.Distinct();
            subject.Subscribe(
                i => Console.WriteLine("{0}", i),
                () => Console.WriteLine("subject.OnCompleted()"));
            distinct.Subscribe(
                i => Console.WriteLine("distinct.OnNext({0})", i),
                () => Console.WriteLine("distinct.OnCompleted()"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(4);
            subject.OnCompleted();
        }

        // DistinctUntilChanged
        public void Example15()
        {
            var subject = new Subject<int>();
            var distinct = subject.DistinctUntilChanged();
            subject.Subscribe(
                i => Console.WriteLine("{0}", i),
                () => Console.WriteLine("subject.OnCompleted()"));
            distinct.Subscribe(
                i => Console.WriteLine("distinct.OnNext({0})", i),
                () => Console.WriteLine("distinct.OnCompleted()"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(4);
            subject.OnCompleted();
        }

        // IgnoreElements
        public void Example16()
        {
            var subject = new Subject<int>();
            //Could use subject.Where(_=>false);
            var noElements = subject.IgnoreElements();
            subject.Subscribe(
                i => Console.WriteLine("subject.OnNext({0})", i),
                () => Console.WriteLine("subject.OnCompleted()"));
            noElements.Subscribe(
                i => Console.WriteLine("noElements.OnNext({0})", i),
                () => Console.WriteLine("noElements.OnCompleted()"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }

        // Skip and Take
        public void Example17()
        {
            Observable.Range(0, 10)
                .Skip(3)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            Console.WriteLine();

            // the Take operator that completes once it has received its count
            Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Take(3)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            Thread.Sleep(500);
        }

        // SkipWhile and TakeWhile
        public void Example18()
        {
            var subject = new Subject<int>();
            subject
                .SkipWhile(i => i < 4)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4); // Print start here
            subject.OnNext(3); // Print
            subject.OnNext(2); // Print
            subject.OnNext(1); // Print
            subject.OnNext(0); // Print
            subject.OnCompleted();
        }

        public void Example19()
        {
            var subject = new Subject<int>();
            subject
                .TakeWhile(i => i < 4)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3); // Print stop here
            subject.OnNext(4);
            subject.OnNext(3);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnNext(0);
            subject.OnCompleted();
        }

        // SkipLast and TakeLast
        public void Example20()
        {
            // Only cache last 2 values
            var subject = new Subject<int>();
            subject
                .SkipLast(2)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            Console.WriteLine("Pushing 1");
            subject.OnNext(1);
            Console.WriteLine("Pushing 2");
            subject.OnNext(2);
            Console.WriteLine("Pushing 3");
            subject.OnNext(3);
            Console.WriteLine("Pushing 4");
            subject.OnNext(4);
            subject.OnCompleted();
        }

        public void Example21()
        {
            // have to cache all values
            var subject = new Subject<int>();
            subject
                .TakeLast(2)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            Console.WriteLine("Pushing 1");
            subject.OnNext(1);
            Console.WriteLine("Pushing 2");
            subject.OnNext(2);
            Console.WriteLine("Pushing 3");
            subject.OnNext(3);
            Console.WriteLine("Pushing 4");
            subject.OnNext(4);
            Console.WriteLine("Completing");
            subject.OnCompleted();
        }

        // SkipUntil and TakeUntil
        public void Example22()
        {
            // SkipUntil will skip all values until any value is produced by a secondary observable sequence
            var subject = new Subject<int>();
            var otherSubject = new Subject<Unit>();
            subject
                .SkipUntil(otherSubject)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            otherSubject.OnNext(Unit.Default);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnNext(6);
            subject.OnNext(7);
            subject.OnNext(8);
            subject.OnCompleted();
        }

        public void Example23()
        {
            // When the secondary sequence produces a value, the TakeWhile operator will complete the output sequence
            var subject = new Subject<int>();
            var otherSubject = new Subject<Unit>();
            subject
                .TakeUntil(otherSubject)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            otherSubject.OnNext(Unit.Default);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnNext(6);
            subject.OnNext(7);
            subject.OnNext(8);
            subject.OnCompleted();
        }

        // **** Inspecting a sequence ****

        // Any
        public void Example24()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var any = subject.Any();
            any.Subscribe(b => Console.WriteLine("The subject has any values? {0}", b));
            subject.OnNext(1);
            subject.OnCompleted();
        }

        // All
        public void Example25()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var all = subject.All(i => i < 5);
            all.Subscribe(b => Console.WriteLine("All values less than 5? {0}", b));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(6);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnCompleted();
        }

        // Contains
        public void Example26()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
            Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var contains = subject.Contains(2);
            contains.Subscribe(
                b => Console.WriteLine("Contains the value 2? {0}", b),
                () => Console.WriteLine("contains completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }

        // DefaultIfEmpty
        public void Example27()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var defaultIfEmpty = subject.DefaultIfEmpty();
            defaultIfEmpty.Subscribe(
                b => Console.WriteLine("defaultIfEmpty value: {0}", b),
                () => Console.WriteLine("defaultIfEmpty completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }

        public void Example28()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var defaultIfEmpty = subject.DefaultIfEmpty();
            defaultIfEmpty.Subscribe(
                b => Console.WriteLine("defaultIfEmpty value: {0}", b),
                () => Console.WriteLine("defaultIfEmpty completed"));
            var default42IfEmpty = subject.DefaultIfEmpty(42);
            default42IfEmpty.Subscribe(
                b => Console.WriteLine("default42IfEmpty value: {0}", b),
                () => Console.WriteLine("default42IfEmpty completed"));
            subject.OnCompleted();
        }

        // ElementAt
        public void Example29()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var elementAt1 = subject.ElementAt(1);
            elementAt1.Subscribe(
                b => Console.WriteLine("elementAt1 value: {0}", b),
                () => Console.WriteLine("elementAt1 completed"));
            // If use ElementAt(5) at below will throw exception 
            var elementAt5OrDefault = subject.ElementAtOrDefault(5).Subscribe(
                b => Console.WriteLine("elementAt5OrDefault value: {0}", b),
                () => Console.WriteLine("elementAt5OrDefault completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }

        // SequenceEqual
        public void Example30()
        {
            var subject1 = new Subject<int>();
            subject1.Subscribe(
                i => Console.WriteLine("subject1.OnNext({0})", i),
                () => Console.WriteLine("subject1 completed"));
            var subject2 = new Subject<int>();
            subject2.Subscribe(
                i => Console.WriteLine("subject2.OnNext({0})", i),
                () => Console.WriteLine("subject2 completed"));
            var areEqual = subject1.SequenceEqual(subject2);
            areEqual.Subscribe(
                i => Console.WriteLine("areEqual.OnNext({0})", i),
                () => Console.WriteLine("areEqual completed"));
            subject1.OnNext(1);
            subject1.OnNext(2);
            subject2.OnNext(1);
            subject2.OnNext(2);
            subject2.OnNext(3);
            subject1.OnNext(3);
            subject1.OnCompleted();
            subject2.OnCompleted();
        }


        // **** Aggregation ****

        // Count
        public void Example31()
        {
            var numbers = Observable.Range(0, 3);
            numbers.Dump("numbers");
            numbers.Count().Dump("count");
        }

        // Min, Max, Sum and Average
        public void Example32()
        {
            var numbers = new Subject<int>();
            numbers.Dump("numbers");
            numbers.Min().Dump("Min");
            numbers.Average().Dump("Average");
            numbers.OnNext(1);
            numbers.OnNext(2);
            numbers.OnNext(3);
            numbers.OnCompleted();
        }

        // ** Functional folds **
        // methods will take an IObservable<T> and produce a T
        // Those (sync) methods are obsolete, async versions are prefered now

        // First, Last, Single
        public async Task Example33()
        {
            var interval = Observable.Interval(TimeSpan.FromSeconds(3));

            // Will block for 3s before returning
            // Console.WriteLine(interval.First());

            // Async Version
            Console.WriteLine($"Async: {await interval.FirstAsync()}");
            Console.WriteLine("3 seconds later");
        }

        // Aggregate
        public async Task Example34()
        {
            var source = Observable.Range(0, 7);
            var sum = await source.Aggregate(0, (acc, currentValue) => acc + currentValue);
            var count = await source.Aggregate(0, (acc, _) => acc + 1);

            System.Console.WriteLine($"sum: {sum}, count: {count}");

            // await returns the last value in Observable            
            System.Console.WriteLine($"Source: {await source}");
        }

        // Scan
        public void Example35()
        {
            var numbers = new Subject<int>();
            var scan = numbers.Scan(0, (acc, current) => acc + current);
            numbers.Dump("numbers");
            scan.Dump("scan");
            numbers.OnNext(1);
            numbers.OnNext(2);
            numbers.OnNext(3);
            numbers.OnCompleted();

            //source.Aggregate(0, (acc, current) => acc + current);
            //is equivalent to
            // source.Scan(0, (acc, current) => acc + current).TakeLast();
        }

        // MinBy & MaxBy
        public async Task Example36()
        {
            var source = Observable.Range(0, 10);

            // 0, key: 0
            // 1, key: 1
            // 2, key: 2
            // 3, key: 0
            // 4, key: 1
            // 5, key: 2
            // 6, key: 0
            // 7, key: 1
            // 8, key: 2
            // 9, key: 0
            var valuesWithMinKey = await source.MinBy(i => i % 3);
            var valuesWithMaxKey = await source.MaxBy(i => i % 3);

            string s1 = string.Join(',', valuesWithMinKey);
            System.Console.WriteLine($"valuesWithMinKey: {s1}");

            string s2 = string.Join(',', valuesWithMaxKey);
            System.Console.WriteLine($"valuesWithMinKey: {s2}");
        }

        // GroupBy
        public void Example37()
        {
            var source = Observable.Range(0, 10);

            // GroupBy returns -> nested observables
            var group = source.GroupBy(i => i % 3); 
            
            // select returns an IObservable<IObservable<T>>
            group.Select(
                grp => grp.Max().Select(value => new { grp.Key, value }))
            .Dump("Select");

            // selectMany return an IObservable<T>
            // selectMany seems more useful!
            group.SelectMany(
                grp => grp.Max().Select(value => new { grp.Key, value }))
            .Dump("SelectMany");
        }
        //public void Example(){}

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine(e.SignalTime);
        }
    }
}