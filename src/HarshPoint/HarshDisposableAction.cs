using System;

namespace HarshPoint
{
    public sealed class HarshDisposableAction : IDisposable
    {
        private Action _action;

        public HarshDisposableAction(Action action)
        {
            if (action == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(action));
            }

            _action = action;
        }

        public void Dispose()
        {
            _action?.Invoke();
            _action = null;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshDisposableAction>();
    }
}
