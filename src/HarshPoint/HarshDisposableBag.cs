using System;
using System.Collections.Immutable;

namespace HarshPoint
{
    public sealed class HarshDisposableBag : IDisposable
    {
        private ImmutableStack<IDisposable> _disposables = ImmutableStack<IDisposable>.Empty;

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
