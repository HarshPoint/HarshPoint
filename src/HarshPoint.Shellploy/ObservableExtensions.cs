using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace HarshPoint.Shellploy
{
    public static class ObservableExtensions
    {
        public static IObservable<T> MergeWithCompleteOnEither<T>(
            this IObservable<T> source,
            IObservable<T> right
        )
        {
            if (source == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(source));
            }

            if (right == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(right));
            }

            return Observable.Create<T>(observer =>
            {
                var compositeDisposable = new CompositeDisposable();
                var subject = new Subject<T>();

                compositeDisposable.Add(subject.Subscribe(observer));
                compositeDisposable.Add(source.Subscribe(subject));
                compositeDisposable.Add(right.Subscribe(subject));

                return compositeDisposable;
            });
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ObservableExtensions));
    }
}
