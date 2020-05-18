using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Chronological.Tests
{
    public class HttpRepositoryTests
    {
        string applicationClientId = "testApplicationId";
        string applicationClientSecret = "testApplicationSecret";
        string tenant = "testTenant";

        [Fact]
        public async void GenericFluentEventQueryTest()
        {            
            var environment = new Environment("TestFqdn", "TestAccessToken");

            var from = new DateTime(2017, 12, 23, 12, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2018, 10, 23, 12, 0, 0, DateTimeKind.Utc);

            var result = await new GenericFluentEventQuery<EmailTest>("Test", Search.Span(from, to), Limit.CreateLimit(Limit.Take, 10), environment, new EventApiRepository(new MockHttpRepository(_eventsResult)))
            .Where(x => x.ip == "127.0.0.1")
                .ExecuteAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async void AggregateQueryTest()
        {
            var environment = new Environment("TestFqdn", "TestAccessToken");

            var from = new DateTime(2017, 12, 23, 12, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2018, 10, 23, 12, 0, 0, DateTimeKind.Utc);

            var result = await new GenericFluentAggregateQuery<EmailTest>("Test", Search.Span(from, to), environment, new AggregateApiRepository(new MockHttpRepository(_aggregateResults)))
            .Select(builder => builder.UniqueValues(x => x.ip, 10, new { Count = builder.Count() }))
            .Where(x => x.ip == "127.0.0.1")
            .ExecuteAsync();

            Assert.NotNull(result);
        }

        private string _eventsResult = @"{
  ""warnings"": [],
  ""events"": [
    {
      ""schema"": {
        ""rid"": 0,
        ""$esn"": ""source1"",
        ""properties"": [
          {
            ""name"": ""email"",
            ""type"": ""String""
          },
          {
            ""name"": ""ip"",
            ""type"": ""String""
          },
          {
            ""name"": ""useragent"",
            ""type"": ""String""
          }
        ]
      },
      ""$ts"": ""2018-10-18T11:30:36Z"",
      ""values"": [
        ""test1@mail.com"",
        ""127.0.0.1"",
        ""IE""
      ]
    },
    {
      ""schemaRid"": 0,
      ""$ts"": ""2018-10-18T11:30:36Z"",
      ""values"": [
        ""test2@mail.com"",
        ""127.0.0.1"",
        ""FF""
      ]
    },
    {
      ""schemaRid"": 0,
      ""$ts"": ""2018-10-18T11:30:36Z"",
      ""values"": [
        ""test3@mail.com"",
        ""127.0.0.1"",
        ""IE""
      ]
    }
  ]
}";

        private string _aggregateResults = @"{
  ""aggregates"": [
    {
      ""dimension"": [
        null,
        ""EmailOpened"",
        ""EmailSent""
      ],
      ""measures"": [
        [
          10.0
        ],
        [
          20.0
        ],
        [
          200.0
        ]
      ]
    }
  ],
  ""warnings"": []
}";

    }

    public class EmailTest
    {
        [ChronologicalEventField("email")]
        public string email { get; set; }
        [ChronologicalEventField("ip")]
        public string ip { get; set; }
        [ChronologicalEventField("useragent")]
        public string useragent { get; set; }
    }
}
