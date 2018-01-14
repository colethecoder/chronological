using Newtonsoft.Json.Linq;
using System;

namespace Chronological
{
    internal interface IInternalAggregate
    {
        IAggregate GetPopulatedAggregate(JObject jObject, Func<JArray, JArray> measureAccessFunc);
    }

    
}
