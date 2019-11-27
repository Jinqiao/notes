using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;

namespace RxIntro
{
    class Part1 : Part
    {
        // **** Key types ****

        // IObservable<T>
        // Only one method -> IDisposable Subscribe(IObserver<T> observer);
        // think of anything that implements IObservable<T> as a streaming sequence of T objects

        // IObserver<T>
        // Has 3 methods 
        // -> void OnNext(T value);
        // -> void OnError(Exception error);
        // -> void OnCompleted();

        public class MyConsoleObserver<T> : IObserver<T>
        {
            public void OnNext(T value)
            {
                Console.WriteLine($"Received value {value}");
            }
            public void OnError(Exception error)
            {
                Console.WriteLine("Sequence faulted with {error}");
            }
            public void OnCompleted()
            {
                Console.WriteLine("Sequence terminated");
            }
        }

        public class MySequenceOfNumbers : IObservable<int>
        {
            public IDisposable Subscribe(IObserver<int> observer)
            {
                observer.OnNext(1);
                observer.OnNext(2);
                observer.OnNext(3);
                observer.OnCompleted();
                return Disposable.Empty;
            }
        }

        public override void Example1()
        {
            var numbers = new MySequenceOfNumbers();
            var observer = new MyConsoleObserver<int>();
            numbers.Subscribe(observer);

            // The problem we have here is that this is not really reactive at all. 
            // This implementation is blocking, so we may as well use an IEnumerable<T> implementation like a List<T> or an array.

            // This problem of implementing the interfaces should not concern us too much. 
            // You will find that when you use Rx, you do not have the need to actually implement these interfaces, 
            // Rx provides all of the implementations you need out of the box. 
        }


        // ** Subject<T> **
        // Subject implements IObservable
        // also OnNext, OnError and OnCompleted
        // it helps us to create an IObservable and control its behavior
        public override void Example2()
        {

            var subject = new Subject<string>();
            subject.Subscribe(Console.WriteLine); // Overloaded extention method, takes an Action<T>, there are many such extention methods
            subject.OnNext("a");
            subject.OnNext("b");
            subject.OnNext("c");
            subject.OnCompleted();
        }

        // ** ReplaySubject<T> **
        // ReplaySubject<T> provides the feature of caching values and then replaying them for any late subscriptions.
        public override void Example3()
        {
            var simpleSubject = new Subject<string>();
            simpleSubject.OnNext("a");
            simpleSubject.Subscribe(Console.WriteLine);
            simpleSubject.OnNext("b");
            simpleSubject.OnNext("c");
            Console.WriteLine();

            var replaySubject = new ReplaySubject<string>();
            replaySubject.OnNext("a");
            replaySubject.Subscribe(Console.WriteLine);
            replaySubject.OnNext("b");
            replaySubject.OnNext("c");
            System.Console.WriteLine();

            // Specify cache size
            var bufferSize = 2;
            var subjectWithBufferSize = new ReplaySubject<string>(bufferSize);
            subjectWithBufferSize.OnNext("a");
            subjectWithBufferSize.OnNext("b");
            subjectWithBufferSize.OnNext("c");
            subjectWithBufferSize.Subscribe(Console.WriteLine);
            subjectWithBufferSize.OnNext("d");
            System.Console.WriteLine();

            // cache expire by time
            var subjectWithExpireTime = new ReplaySubject<string>(TimeSpan.FromMilliseconds(150));
            subjectWithExpireTime.OnNext("w");
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            subjectWithExpireTime.OnNext("x");
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            subjectWithExpireTime.OnNext("y");
            subjectWithExpireTime.Subscribe(Console.WriteLine);
            subjectWithExpireTime.OnNext("z");
        }

        // ** BehaviorSubject<T> **    
        // BehaviorSubject<T> is similar to ReplaySubject<T> except it only remembers the last publication. 
        // BehaviorSubject<T> also requires you to provide it a default value of T. 
        // This means that all subscribers will receive a value immediately (unless it is already completed).   

        // BehaviorSubject<T>s are often associated with class properties. As they always have a value and 
        // can provide change notifications, they could be candidates for backing fields to properties.
        public override void Example4()
        {
            //Need to provide a default value.
            var subject = new BehaviorSubject<string>("a");
            subject.Subscribe(Console.WriteLine);
            System.Console.WriteLine();

            // Only remember last
            var subject2 = new BehaviorSubject<string>("a");
            subject2.OnNext("b");
            subject2.Subscribe(Console.WriteLine);
            subject2.OnNext("c");
            subject2.OnNext("d");
            System.Console.WriteLine();

            // Nothing will write
            var subject3 = new BehaviorSubject<string>("a");
            subject3.OnNext("b");
            subject3.OnNext("c");
            subject3.OnCompleted();
            subject3.Subscribe(Console.WriteLine);
        }

        // ** AsyncSubject<T> **
        // AsyncSubject<T> is similar to the Replay and Behavior subjects in the way that it caches values, however it will only store the last value, 
        // and only publish it when the sequence is completed. The general usage of the AsyncSubject<T> is to only ever publish one value then immediately 
        // complete. This means that is becomes quite comparable to Task<T>
        public override void Example5()
        {
            var subject = new AsyncSubject<string>();
            subject.OnNext("a");
            subject.Subscribe(Console.WriteLine);
            subject.OnNext("b");
            subject.OnNext("c");
            System.Console.WriteLine("No value will write");

            subject.OnCompleted();
            System.Console.WriteLine("The last value is published after completion");
        }


        // **** Lifetime management ****
        // Unsubscribing -> Dispose()
        public override void Example6()
        {
            var values = new Subject<int>();
            var firstSubscription = values.Subscribe(value =>
                Console.WriteLine($"1st subscription received {value}"));
            var secondSubscription = values.Subscribe(value =>
                Console.WriteLine($"2nd subscription received {value}"));
            values.OnNext(0);
            values.OnNext(1);
            values.OnNext(2);
            values.OnNext(3);
            firstSubscription.Dispose();
            Console.WriteLine("Disposed of 1st subscription");
            values.OnNext(4);
            values.OnNext(5);
        }

        // OnError and OnCompleted
        // Both the OnError and OnCompleted signify the completion of a sequence.
        // After any of these, no OnNext should be called, otherwise unpredictable behavior may happen
        // When a sequence completes or errors, you should still dispose of your subscription.
        // It is best practice to always provide an OnError handler to prevent an exception being 
        // thrown in an otherwise difficult to handle manner.
        public override void Example7()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                i => System.Console.WriteLine(i),
                () => Console.WriteLine("Completed"));
            subject.OnCompleted();
            subject.OnNext(2); // This will not print
        }

        // IDisposable
        // By leveraging the common IDisposable interface, Rx offers the ability to have 
        // deterministic control over the lifetime of your subscriptions. Subscriptions are 
        // independent, so the disposable of one will not affect another. While some Subscribe 
        // extension methods utilize an automatically detaching observer, it is still considered 
        // best practice to explicitly manage your subscriptions, as you would with any other 
        // resource implementing IDisposable.
        public override void Example8()
        {
            // The Create method will ensure the standard Dispose semantics, so calling Dispose() 
            // multiple times will only invoke the delegate you provide once
            var disposable = Disposable.Create(() => Console.WriteLine("Being disposed."));
            Console.WriteLine("Calling dispose...");
            disposable.Dispose();
            Console.WriteLine("Calling again...");
            disposable.Dispose();
        }

        public override void Example9()
        {
            throw new NotImplementedException();
        }
    }
}
