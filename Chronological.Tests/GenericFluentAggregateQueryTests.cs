using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
    'from': '2017-12-27T00:00:00.0000000Z',
    'to': '2017-12-28T00:00:00.0000000Z'
  },
  'predicate': {
    'predicateString': '([data.value] > 5)'
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
            var environment = new Environment("TestFqdn", "TestAccessToken");
            var from = new DateTime(2017, 12, 27,0,0,0,DateTimeKind.Utc);
            var to = new DateTime(2017, 12, 28,0,0,0, DateTimeKind.Utc);

            var queryString = environment.AggregateQuery<TestType1>("Test", Search.Span(from, to))
                .Select(builder => builder.UniqueValues(x => x.Value, Limit.Take(10),
                    builder.UniqueValues(x => x.Value, Limit.Take(10),
                        new
                        {
                            Maximum = builder.Maximum(x => x.Value)
                        })))
                .Where(x => x.Value > 5)
                .ToString();

            var expected = JToken.Parse("{" + ExpectedQuery() + "}")["content"];
            var actual = JToken.Parse(queryString)["content"];

            Assert.True(JToken.DeepEquals(expected, actual));
        }

        [Fact]
        public async void Test2()
        {
            var environment = new Environment("TestFqdn", "TestAccessToken");
            var from = new DateTime(2017, 12, 23, 12, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2017, 12, 30, 12, 0, 0, DateTimeKind.Utc);

            var result = await new GenericFluentAggregateQuery<TestType1>("Test", Search.Span(from, to), environment, new TestWebSocketRepository(_webSocketResult))
                .Select(builder => builder.UniqueValues(x => x.DataType, Limit.Take(10),
                                    builder.DateHistogram(x => x.Date, Breaks.InDays(1),                                        
                                    new
                                    {
                                        Maximum = builder.Maximum(x => x.Value),
                                        Minmum = builder.Minimum(x => x.Value)
                                    })))
                .Where(x => x.Value > 5)
                .Execute();

        }

        private string _webSocketResult = @"{
            'headers': {
            'x-ms-request-id': '67a05803-07cd-4efe-8281-bf53b01d9acc'
        },
        'content': [
    {
        'dimension': [
        'Basket Motor Phase1',
        'Heater Phase1',
        'Machine DeviceInputVoltage',
        'Machine LowLevel',
        'Machine OverTemperature',
        'Machine Running',
        'Machine SolutionTemperature',
        'Pump Phase1'
            ],
        'aggregate': {
            'dimension': [
            '2017-12-23T00:00:00Z',
            '2017-12-24T00:00:00Z',
            '2017-12-25T00:00:00Z',
            '2017-12-26T00:00:00Z',
            '2017-12-27T00:00:00Z',
            '2017-12-28T00:00:00Z',
            '2017-12-29T00:00:00Z',
            '2017-12-30T00:00:00Z'
                ],
            'measures': [
                [
                [
            0.002,
            0.002
                ],
                [
            0.002,
            0.002
                ],
                [
            0.002,
            0.002
                ],
                [
            0.002,
            0.002
                ],
                [
            0.002,
            0.002
                ],
                [
            0.002,
            0.002
                ],
                [
            0.002,
            0.002
                ],
                [
            0.002,
            0.002
                ]
                ],
                [
                [
            8.837,
            0.002
                ],
                [
            8.833,
            0.002
                ],
                [
            8.837,
            0.002
                ],
                [
            8.83,
            0.002
                ],
                [
            8.815,
            0.002
                ],
                [
            8.757,
            0.002
                ],
                [
            0.002,
            0.002
                ],
                [
            0.034,
            0.002
                ]
                ],
                [
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ]
                ],
                [
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            1.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ]
                ],
                [
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            1.0,
            0.0
                ],
                [
            1.0,
            1.0
                ],
                [
            1.0,
            1.0
                ]
                ],
                [
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ],
                [
            0.0,
            0.0
                ]
                ],
                [
                [
            62.6,
            59.0
                ],
                [
            63.0,
            58.8
                ],
                [
            63.7,
            58.2
                ],
                [
            63.0,
            56.5
                ],
                [
            61.1,
            54.0
                ],
                [
            59.1,
            19.3
                ],
                [
            19.5,
            15.0
                ],
                [
            15.0,
            14.6
                ]
                ],
                [
                [
            0.002,
            0.002
                ],
                [
            0.002,
            0.002
                ],
                [
            0.002,
            0.002
                ],
                [
            0.002,
            0.002
                ],
                [
            0.002,
            0.002
                ],
                [
            0.002,
            0.002
                ],
                [
            0.002,
            0.002
                ],
                [
            0.002,
            0.002
                ]
                ]
                ]
        }
    }
    ],
    'warnings': [],
    'percentCompleted': 100.0
}";

        private class TestWebSocketRepository : IWebSocketRepository
        {
            private readonly List<string> _results;

            public TestWebSocketRepository(string result) : this(new List<string> {result})
            {
            }

            public TestWebSocketRepository(List<string> results)
            {
                _results = results;
            }

            public async Task<List<string>> QueryWebSocket(string query, string resourcePath)
            {
                return _results;
            }
        }
    }
}
