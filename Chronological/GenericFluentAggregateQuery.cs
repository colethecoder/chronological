using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class GenericFluentAggregateQuery<T> where T : new()
    {        

        public GenericFluentAggregateQuery<T, Aggregate<T,TX,TY>> Select<TX,TY>(Func<AggregateBuilder<T>,Aggregate<T,TX,TY>> predicate)
        {
            var aggregate = predicate(new AggregateBuilder<T>());

            return new GenericFluentAggregateQuery<T, Aggregate<T,TX,TY>>();
        }
        
    }

    public class GenericFluentAggregateQuery<TX, TY> where TX : new()
    {
        public GenericFluentAggregateQuery<TX, TY> Where(Expression<Func<TX,bool>> predicate)
        {
            var binaryExpression = (BinaryExpression)predicate.Body;
            var leftSide = binaryExpression.Left;
            var rightSide = binaryExpression.Right;
            var comparison = binaryExpression.NodeType;


            throw new NotImplementedException();
        }

        public async Task<TY> Execute()
        {
            await Task.FromResult(0); //Just to stop warnings for now
            throw new NotImplementedException();
        }        
    }

    
}
