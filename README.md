# chronological
Chronological is a library to simplify access to the Azure Time Series Insights API. Azure Time Series Insights is still in preview and this project is a work in progress so there are likely to be breaking changes which we will try to stay on top of. Feedback and pull requests are welcomed.

## Example

```cs
var conn = new Chronological.Connection(YourApplicationClientID,
                YourApplicationClientSecret, YourTenant);

var environments = await conn.GetEnvironmentsAsync();

var environment = environments.First();

var json = await environment.EventsQuery("Test")
    .WithSearch(Search.Span(DateTime.Now.AddDays(-2), DateTime.Now)
    .WithLimit(Limit.Top(200, Sort.Ascending(Property.TimeSeries)))
    .Where(Filter.Equal(Property.Custom("id", DataType.String), "1234"))
    .ResultsToJObject();
```

## Getting Started

To access the Time Series Insights API you first have to setup an environment in Azure, details here:

https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-get-started

You then need to configure authentication, instructions here:

https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-authentication-and-authorization

Once you have completed these steps and have your authentication details you can begin to use Chronological to access data.

