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

public class BehaviorSubjectProperty
{
    // From https://stackoverflow.com/a/48345767/4499942
    private BehaviorSubject<int> _myNumberChanged = new BehaviorSubject<int>(0);
    private int _myNumber;
    public int MyNumber
    {
        get => _myNumber;
        set
        {
            if (_myNumber == value)
            {
                return;
            }

            _myNumber = value;
            _myNumberChanged.OnNext(_myNumber);
        }
    }
}