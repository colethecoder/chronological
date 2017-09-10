# chronological #
Chronological is a library to simplify access to the Azure Time Series Insights API. Time Series Insights is still in preview and this project is a work in progress so there are likely to be breaking changes. Feedback and pull requests are welcomed.

## Download ##

The latest release of the Chronological is [available on NuGet](https://www.nuget.org/packages/Chronological/)

## Example ##

```cs
var connection = new Chronological.Connection(YourApplicationClientID,
                YourApplicationClientSecret, YourTenant);

var environments = await connection.GetEnvironmentsAsync();

var environment = environments.First();

var json = await environment.EventsQuery("Test")
    .WithSearch(Search.Span(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow)
    .WithLimit(Limit.Top(200, Sort.Ascending(Property.TimeSeries)))
    .Where(Filter.Equal(Property.Custom("id", DataType.String), "1234"))
    .ResultsToJObjectAsync();
```

## Getting Started ##

[Set up your Azure Time Series Insights Environment](Documentation/Environment-Config.md)

### Creating a connection ###

To use the API you first have to create a connection using the Active Directory details for your instance:

```cs
var connection = new Chronological.Connection(YourApplicationClientID,
                YourApplicationClientSecret, YourTenant);
```

### Getting Environments ###

Once you have a Connection object you can then retreive a list of available Environments:

```cs
var environments = await connection.GetEnvironmentsAsync();
```

You can also create an Environment object directly if you have the FQDN for the environment and an access token from another source:

```cs
var environment = new Environment(YourEnvironmentFqdn, YourAccessToken);
```

### Querying an Environment ###



