using System;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResultSingleOrDefault<T> : 
        ResolveResultBase, IResolveSingleOrDefault<T>
    {
        private Status _status;
        private T _value;

        public Boolean HasValue
        {
            get
            {
                LoadResults();
                return _status == Status.ResultSingle;
            }
        }

        public T Value
        {
            get
            {
                LoadResults();
                return _value;
            }
        }

        private void LoadResults()
        {
            if (_status == Status.Uninitialized)
            {
                var array = EnumerateResults<T>(allowFailures: true);

                switch (array.Count)
                {
                    case 0:
                        _status = Status.ResultNone;
                        break;

                    case 1:
                        _value = array[0];
                        _status = Status.ResultSingle;
                        break;

                    default:
                        _status = Status.ResultsMany;
                        break;
                }
            }

            if (_status == Status.ResultsMany)
            {
                throw Logger.Fatal.InvalidOperationFormat(
                    SR.Resolvable_ManyResults, 
                    ResolveBuilder
                );
            }
        }

        private static readonly HarshLogger Logger 
            = HarshLog.ForContext(typeof(ResolveResultSingle<>));

        private enum Status
        {
            Uninitialized,
            ResultNone,
            ResultSingle,
            ResultsMany,
        }
    }
}
