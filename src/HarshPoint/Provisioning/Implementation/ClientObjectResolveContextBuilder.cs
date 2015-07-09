using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ClientObjectResolveContextBuilder
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext<ClientObjectResolveContextBuilder>();

        private readonly ClientObjectResolveContext _result = new ClientObjectResolveContext();

        public void Load<T>(IResolve2<T> resolver, params Expression<Func<T, Object>>[] retrievals)
            where T : ClientObject
        {
            if (resolver == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolver));
            }

            if (retrievals == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(retrievals));
            }

            if (!retrievals.Any())
            {
                return;
            }

            _result.Include(retrievals);
        }

        internal ClientObjectResolveContext ToResolveContext()
        {
            return _result;
        }
    }
}
