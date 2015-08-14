using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint
{
    public sealed class HarshDisposableBag : IDisposable
    {
        private ImmutableStack<IDisposable> _disposables = ImmutableStack<IDisposable>.Empty;

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public void Add(Action action)
        {
            if (action == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(action));
            }

            _disposables = _disposables.Push(new HarshDisposableAction(action));
        }

        public void Add(IDisposable disposable)
        {
            if (disposable == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(disposable));
            }

            _disposables = _disposables.Push(disposable);
        }

        public void Dispose()
        {
            foreach (var item in _disposables)
            {
                item.Dispose();
            }

            _disposables = ImmutableStack<IDisposable>.Empty;
        }

        /// <summary>
        /// Not intended for production code.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void TryDispose()
        {
            foreach (var item in _disposables)
            {
                try
                {
                    item.Dispose();
                }
                catch (Exception exc)
                {
                    Logger.Fatal.Write(exc);
                }
            }
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshDisposableBag>();
    }
}
