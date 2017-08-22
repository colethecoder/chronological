# chronological
Chronological is a library to simplify access to the Azure Time Series Insights API.

```
var conn = new Chronological.Connection(*ApplicationClientID*,
                *ApplicationClientSecret*, *Tenant*);

var environments = await conn.GetEnvironmentsAsync();

var environment = environments.First();

var json = await environment.EventsQuery("Test")
    .WithSearch(Search.Span(DateTime.Now.AddDays(-2), DateTime.Now)
    .WithLimit(Limit.Top(200, Sort.Ascending(Property.TimeSeries)))
    .Where(Filter.Equal(Property.Custom("id", DataType.String), "1234"))
    .ResultsToJObject();
```