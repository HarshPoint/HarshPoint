using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    partial class ClientObjectQueryProcessor
    {
        private sealed class RetrievalExpressionComparer : IEqualityComparer<Expression>
        {
            public Boolean Equals(Expression x, Expression y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;

                if (x.Type != y.Type) return false;
                throw new NotImplementedException();

            }

            public Int32 GetHashCode(Expression obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
