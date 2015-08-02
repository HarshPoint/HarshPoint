using System;

namespace HarshPoint
{
    internal struct HarshDisposableAction : IDisposable
    {
        private Action _action;

        public HarshDisposableAction(Action action)
        {
            _action = action;
        }
        public void Dispose()
        {
            _action?.Invoke();
            _action = null;
        }
    }
}
