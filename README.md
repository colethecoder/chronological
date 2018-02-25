[![Build status](https://ci.appveyor.com/api/projects/status/3x04uwu4bu4fy28s/branch/master?svg=true)](https://ci.appveyor.com/project/colethecoder/chronological/branch/master)

# chronological
Chronological is a library to simplify access to the Azure Time Series Insights API. This project is a work in progress, feedback and pull requests are welcomed.

## Download

The latest release of the Chronological is [available on NuGet](https://www.nuget.org/packages/Chronological/)

## Example

Entity:

```cs
public class TimeSeriesEntity
{
    [ChronologicalEventField("DeviceId")]
    public string Id { get; set; }

    [ChronologicalEventField(BuiltIn.EventTimeStamp)]
    public DateTime Date { get; set; }

    [ChronologicalEventField("EventType")]
    public string Type { get; set; }
    [ChronologicalEventField("MeasurementValue")]
    public double? Value { get; set; }
}
```

Get an environment:

```cs
var connection = new Chronological.Connection(YourApplicationClientID,
                YourApplicationClientSecret, YourTenant);

var environments = await connection.GetEnvironmentsAsync();

var environment = environments.First();
```

Query for events:

```cs
var events = await environment.EventQuery<TimeSeriesEntity>(FromDate, ToDate, Limit.Take, 200)
                    .Where(x => x.Value > 5)
                    .ExecuteAsync();
```

Query for aggregate data:

```cs
var aggregates = await environment.AggregateQuery<TestType1>(FromDate, ToDate)
                    .Select(builder => builder.UniqueValues(x => x.Type, 10,
                                builder.UniqueValues(x => x.Id, 10,
                                    builder.DateHistogram(x => x.Date, Breaks.InDays(1),
                                        new
                                        {
                                            Count = builder.Count(),
                                            Max = builder.Maximum(x => x.Value)
                                        }))))
                    .Where(x => x.Value > 5)
                    .ExecuteAsync();
```

## Setting Up Time Series Insights ready for use with Chronological

To access the Time Series Insights API you first have to setup an environment in Azure, details here:

[https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-get-started](https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-get-started)

You then need to configure authentication, instructions here:

[https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-authentication-and-authorization](https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-authentication-and-authorization)

Once you have completed these steps and have your authentication details you can begin to use Chronological to access data.

## Creating a Connection

To use the API you first have to create a connection using the Active Directory details for your instance:

```cs
var connection = new Chronological.Connection(YourApplicationClientID,
                YourApplicationClientSecret, YourTenant);
```

## Getting Environments 

Once you have a Connection object you can then retrieve a list of available Environments (IEnumerable):

var environments = await connection.GetEnvironmentsAsync();
This is using the Get Environments API

You can also create an Environment object directly if you have the FQDN for the environment and an access token from another source:

var environment = new Environment(YourEnvironmentFqdn, YourAccessToken);