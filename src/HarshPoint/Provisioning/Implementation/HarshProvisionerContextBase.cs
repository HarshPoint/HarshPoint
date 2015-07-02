using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class HarshProvisionerContextBase
    {
        public abstract Boolean MayDeleteUserData
        {
            get;
        }

        public HarshProvisionerContextBase PushState(Object state)
        {
            return PushStateCore(state);
        }

        public IEnumerable<T> GetState<T>()
        {
            var results = new List<T>();

            foreach (var state in StateStack)
            {
                var typedCollection = (state as IEnumerable<T>);

                if (typedCollection != null)
                {
                    results.AddRange(typedCollection);
                }
                else if (state is T)
                {
                    results.Add((T)(state));
                }
            }

            return results;
        }
        
        public IEnumerable<Object> GetState(Type type)
        {
            if (type == null)
            {
                throw Error.ArgumentNull(nameof(type));
            }

            var info = type.GetTypeInfo();

            return GetState<Object>()
                .Where(state => 
                    info.IsAssignableFrom(state.GetType().GetTypeInfo())
                );
        }

        internal abstract IImmutableStack<Object> StateStack
        {
            get;
        }

        internal abstract HarshProvisionerContextBase PushStateCore(Object state);
    }
}
