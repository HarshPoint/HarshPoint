using System;

namespace HarshPoint
{
    public class HarshDependencyGraphCycleException : Exception
    {
        public HarshDependencyGraphCycleException() { }
        public HarshDependencyGraphCycleException(String message) : base(message) { }
        public HarshDependencyGraphCycleException(String message, Exception inner) : base(message, inner) { }
    }
}
