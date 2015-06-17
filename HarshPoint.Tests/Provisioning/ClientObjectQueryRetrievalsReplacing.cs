using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class ClientObjectQueryRetrievalsReplacing
    {
        [Fact]
        public void Throws_on_missing_Include()
        {
            var visitor = new ClientObjectResolveRetrievalTransformer<List>();
            var expression = GetExpression(w => w.Lists);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => visitor.Process(expression)
            );
        }

        [Fact]
        public void Throws_on_two_Includes()
        {

            var visitor = new ClientObjectResolveRetrievalTransformer<List>();
            var expression = GetExpression(w => w.Lists.Include().IncludeWithDefaultProperties());

            Assert.Throws<ArgumentOutOfRangeException>(
                () => visitor.Process(expression)
            );
        }

        [Fact]
        public void Empty_Include_is_replaced_with_retrievals()
        {
            var visitor = new ClientObjectResolveRetrievalTransformer<List>(l => l.Title);
            var expression = GetExpression(
                w => w.Lists.Include()
            );

            var visited = visitor.Process(expression);
            Assert.NotSame(expression, visited);

            var lambda = visited as LambdaExpression;
            Assert.NotNull(lambda);

            var call = lambda.Body as MethodCallExpression;
            Assert.NotNull(call);

            var arguments = call.Arguments;
            Assert.Equal(2, arguments.Count);

            var secondArg = arguments[1] as NewArrayExpression;
            Assert.NotNull(secondArg);

            var retrieval = Assert.Single(secondArg.Expressions);
            Assert.NotNull(retrieval);
            Assert.Equal("Title", retrieval.TryExtractSinglePropertyAccess().Name);
        }

        [Fact]
        public void Retrievals_are_added_to_existing_Include()
        {
            var visitor = new ClientObjectResolveRetrievalTransformer<List>(l => l.Title);
            var expression = GetExpression(
                w => w.Lists.Include(l => l.Description)
            );

            var visited = visitor.Process(expression);
            Assert.NotSame(expression, visited);

            var lambda = visited as LambdaExpression;
            Assert.NotNull(lambda);

            var call = lambda.Body as MethodCallExpression;
            Assert.NotNull(call);

            var arguments = call.Arguments;
            Assert.Equal(2, arguments.Count);

            var secondArg = arguments[1] as NewArrayExpression;
            Assert.NotNull(secondArg);

            Assert.Equal(2, secondArg.Expressions.Count);
            Assert.Equal("Description", secondArg.Expressions[0].TryExtractSinglePropertyAccess().Name);
            Assert.Equal("Title", secondArg.Expressions[1].TryExtractSinglePropertyAccess().Name);
        }

        [Fact]
        public void Empty_IncludeWitDefaultProperties_is_replaced_with_retrievals()
        {
            var visitor = new ClientObjectResolveRetrievalTransformer<List>(l => l.Title);
            var expression = GetExpression(
                w => w.Lists.IncludeWithDefaultProperties()
            );

            var visited = visitor.Process(expression);
            Assert.NotSame(expression, visited);

            var lambda = visited as LambdaExpression;
            Assert.NotNull(lambda);

            var call = lambda.Body as MethodCallExpression;
            Assert.NotNull(call);

            var arguments = call.Arguments;
            Assert.Equal(2, arguments.Count);

            var secondArg = arguments[1] as NewArrayExpression;
            Assert.NotNull(secondArg);

            var retrieval = Assert.Single(secondArg.Expressions);
            Assert.NotNull(retrieval);
            Assert.Equal("Title", retrieval.TryExtractSinglePropertyAccess().Name);
        }

        [Fact]
        public void Include_call_is_removed_when_no_retrievals()
        {
            var visitor = new ClientObjectResolveRetrievalTransformer<List>();
            var expression = GetExpression(
                w => w.Lists.Include()
            );

            var visited = visitor.Process(expression);
            Assert.NotSame(expression, visited);

            var lambda = visited as LambdaExpression;
            Assert.NotNull(lambda);

            var listsAccess = lambda.Body as MemberExpression;
            Assert.NotNull(listsAccess);
            Assert.Equal("Lists", listsAccess.Member.Name);
        }


        private static Expression GetExpression(Expression<Func<Web, Object>> expr)
        {
            return expr;
        }
    }
}
