using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Xunit;

namespace Chronological.Tests
{
    public class EventParsingTests
    {
        [Fact]
        public void NewParserIsSuccess()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Chronological.Tests.Data.events.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {                
                var result = FastEventParser.ParseEvents<TestType1>(reader);

                Assert.IsType<WebSocketResult<TestType1>.WebSocketSuccess>(result);
            }
        }

        [Fact]
        public void NewParserWorksAsOldOne()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Chronological.Tests.Data.events.json";

            WebSocketResult<TestType1> result1, result2;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result1 = new WebSocketReader<TestType1>(
                                new EventWebSocketRepository(null)
                                    .ParseEvents<TestType1>)
                                        .Read(reader);

            }

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            { 
                result2 = FastEventParser.ParseEvents<TestType1>(reader);
            }

            AssertAreEqualByJson(result1, result2);
        }

        public void AssertAreEqualByJson(object expected, object actual)
        {
            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);
            Assert.Equal(expectedJson, actualJson);
        }
    }
}
