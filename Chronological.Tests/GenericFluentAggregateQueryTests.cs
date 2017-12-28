using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Chronological.Tests
{
    public class GenericFluentAggregateQueryTests
    {
        public string ExpectedQuery()
        {
            return @"'content': {
  'searchSpan': {
    'from': '2017-12-27T18:32:17.6395695Z',
    'to': '2017-12-28T18:32:17.6395695Z'
  },
  'predicate': {
    'predicateString': '[data.value] > 5'
  },
  'aggregates': [
    {
      'dimension': {
        'uniqueValues': {
          'input': {
            'property': 'data.value',
            'type': 'Double'
          },
          'take': 10
        }
      },
      'aggregate': {
        'dimension': {
          'uniqueValues': {
            'input': {
              'property': 'data.value',
              'type': 'Double'
            },
            'take': 10
          }
        },
        'measures': [
          {
            'max': {
              'input': {
                'property': 'data.value',
                'type': 'Double'
              }
            }
          }
        ]
      }
    }
  ]
}";
        }

        [Fact]
        public void Test1()
        {
            var environment = new Chronological.Environment("TestFqdn", "TestAccessToken");
            var from = DateTime.Now.AddDays(-1);
            var to = DateTime.Now;

            var queryString = environment.AggregateQuery<TestType1>("Test", Search.Span(from, to))
                .Select(builder => builder.UniqueValues(x => x.Value, Limit.Take(10),
                    builder.UniqueValues(x => x.Value, Limit.Take(10),
                        new
                        {
                            Maximum = builder.Maximum(x => x.Value)
                        })))
                .Where(x => x.Value > 5)
                .ToString();

            var expected = JToken.Parse("{" + ExpectedQuery() + "}");
            var actual = JToken.Parse("{" + queryString + "}");

            Assert.True(JToken.DeepEquals(expected, actual));
        }

    }
}
