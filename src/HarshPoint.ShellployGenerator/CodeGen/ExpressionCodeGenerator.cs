using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    class ExpressionCodeGenerator : ExpressionVisitor
    {
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.Assign)
            {

            }

            return base.VisitBinary(node);
        }
    }
}
